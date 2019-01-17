using System;
using System.Collections;
using System.Collections.Generic;
using BallGame.Controller;
using BallGame.Model;
using BallGame.Utils;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace BallGame.View
{
	public class SimulationView : MonoBehaviour, IUnitCollisionHandler
	{
		public UnityEvent OnSimulationFinished
		{
			get { return _onSimulationFinished; }
		}

		public Simulation Simulation
		{
			get { return _simulation; }
		}

		public Material[] Colors
		{
			get { return _colors; }
		}
		
		public float Speed
		{
			get
			{
				return _simSpeed;
			}
			set
			{
				value = Mathf.Clamp(value, 0, 25);
				_simSpeed = value;
			}
		}

		static SimulationView _instance;

		public static SimulationView Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<SimulationView>();
				}
				return _instance;
			}
		}

		[SerializeField, Range(0f, 5f)] float _simSpeed = 1;
		[SerializeField] bool _createSimulationOnAwake;
		[SerializeField] TextAsset _configData;
		[SerializeField] Material[] _colors;
		[SerializeField] float _minRadius = .2f;
		[SerializeField] UnitView _unitViewPrefab;
		[SerializeField] Transform _fieldTransform;
		[SerializeField] UnityEvent _onSimulationFinished;

		SimulationConfig _config;
		Simulation _simulation;
		readonly List<UnitView> _units = new List<UnitView>();
		Camera _cam;
		Pool<UnitView> _unitsPool;
		readonly List<int> _intValuesBuffer = new List<int>();
		
		const string SIM_SAVE_KEY = "sim";

		void Awake()
		{
			if (_instance != null && _instance != this)
			{
				return;
			}
			_instance = this;
			Init();
			if(_createSimulationOnAwake)
				CreateNewSimulation();
		}

		void Update()
		{
			if(_simulation == null)
				return;
			_simulation.Speed = _simSpeed;
			_simulation.Tick(Time.deltaTime);
			foreach (var unit in _units)
			{
				unit.UpdateTransform();
			}
		}

		public void Init()
		{
			_config = new ConfigLoader(_configData).LoadConfig();
			_cam = Camera.main;
			_unitsPool = new Pool<UnitView>(() => Instantiate(_unitViewPrefab), _config.numUnitsToSpawn);
			SetConfig(_config);
		}

		public bool CanCollide(Unit a, Unit b)
		{
			return a.State.Type == b.State.Type;
		}

		public void OnUnitsGoThrough(Unit a, Unit b)
		{
			var dist = _simulation.GetOverlappingDistance(a, b) / 2;
			a.SetRadius(a.State.Radius - dist);
			b.SetRadius(b.State.Radius - dist);
		
			if (a.State.Radius < _minRadius || b.State.Radius < _minRadius)
			{
				_simulation.DestroyUnit(a);
				_simulation.DestroyUnit(b);
			}
		}
		
		public void OnUnitDestroyed(UnitView unitView)
		{
			if (IsSimulationOver())
			{
				_onSimulationFinished.Invoke();
			}
		}

		bool IsSimulationOver()
		{
			_intValuesBuffer.Clear();
			foreach (var activeUnit in _simulation.ActiveUnits)
			{
				if(_intValuesBuffer.FindIndex(t => activeUnit.State.Type == t) < 0)
					_intValuesBuffer.Add(activeUnit.State.Type);
			}
			return _intValuesBuffer.Count <= 1;
		}

		public void OnUnitsCollision(Unit a, Unit b)
		{
			//TODO: add some  collision logic
		}

		public Material GetWinnerMaterial()
		{
			if (_simulation == null || _simulation.ActiveUnits.Count == 0)
				return null;
			if (_simulation.ActiveUnits.Count > 0)
			{
				return GetMaterialType(_simulation.ActiveUnits[0].State.Type);
			}
			return null;
		}
		
		public void LoadSimulationFromJson()
		{
			var json = PlayerPrefs.GetString(SIM_SAVE_KEY);
			var state = JsonUtility.FromJson<SimulationState>(json);
			LoadSimulation(state);
		}

		public void SaveSimulation()
		{
			if(_simulation == null)
				return;
			var state = _simulation.SaveState();
			var json = JsonUtility.ToJson(state);
			PlayerPrefs.SetString(SIM_SAVE_KEY, json);
			PlayerPrefs.Save();
		}

		public void CreateNewSimulation()
		{
			LoadSimulation(new SimulationState());
			StartCoroutine(UnitSpawningRoutine());
		}

		public void LoadSimulation(SimulationState state)
		{
			_unitsPool.Reset();
			_simulation = new Simulation(state, _config, this);
			foreach (var activeUnit in _simulation.ActiveUnits)
			{
				CreateUnitView(activeUnit);
			}
		}
		
		public void SetConfig(SimulationConfig config)
		{
			_fieldTransform.localScale = new Vector3(config.gameAreaWidth, config.gameAreaHeight, 1);
			SetCameraSize(config.gameAreaWidth, config.gameAreaHeight);
		}
		
		public Material GetMaterialType(int typeIndex)
		{
			if (typeIndex < 0 || typeIndex >= _colors.Length)
			{
				Debug.LogError("Invalid type index: " + typeIndex + ". Should be between 0 and " + (_colors.Length - 1));
				return _colors[0];
			}
			return _colors[typeIndex];
		}

		void SetCameraSize(float width, float height)
		{
			var aspect = width / height;
			if (aspect > _cam.aspect)
			{
				_cam.orthographicSize = (width / _cam.aspect) / 2;
			}
			else
			{
				_cam.orthographicSize = height / 2;
			}
		}

		IEnumerator UnitSpawningRoutine()
		{
			var unitsLeft = _config.numUnitsToSpawn;
			while (unitsLeft > 0)
			{
				Vector point;
				float rad;
				do
				{
					rad = Random.Range(_config.minUnitRadius, _config.maxUnitRadius);
					var x = Random.Range(-.5f, .5f) * (_config.gameAreaWidth - (rad * 2));
					var y = Random.Range(-.5f, .5f) * (_config.gameAreaHeight - (rad * 2));
					point = new Vector(x, y);
				} 
				while (_simulation.IsOverlappingUnit(point, rad));

				var unitState = new UnitState(point, Vector.Zero, rad, Random.Range(0, _colors.Length));
				
				var unit = new Unit(unitState);
				
				_simulation.AddUnit(unit);
				
				CreateUnitView(unit);
				
				unitsLeft--;
				
				yield return new WaitForSeconds(TimeSpan.FromMilliseconds(_config.unitSpawnDelay).Seconds);
			}

			foreach (var unit in _simulation.ActiveUnits)
			{
				var speed = Random.Range(_config.minUnitSpeed, _config.maxUnitSpeed);
				unit.SetVelocity(new Vector(Random.Range(-1f, 1f), Random.Range(-1f,1f)).Normalized * speed);
			}
		}
		
		void CreateUnitView(Unit unit)
		{
			var unitView = _unitsPool.Pick();
			unitView.SetUp(unit, this);
			_units.Add(unitView);
		}
	}
}
