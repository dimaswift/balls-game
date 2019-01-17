using System;
using System.Collections;
using System.Collections.Generic;
using BallGame.Controllers;
using BallGame.Model;
using BallGame.Utils;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace BallGame.Views
{
	public class SimulationView : View
	{
		public Material[] Colors
		{
			get { return _colors; }
		}
		
		[SerializeField] UnitView _unitViewPrefab;
		[SerializeField] Transform _fieldTransform;
		[SerializeField] Material[] _colors;
		
		SimulationController _controller;
		
		readonly List<UnitView> _units = new List<UnitView>();
		Camera _cam;
		Pool<UnitView> _unitsPool;

		public void Init(SimulationConfig config)
		{
			_unitsPool = new Pool<UnitView>(() => Instantiate(_unitViewPrefab), config.numUnitsToSpawn);
		}
		
		public void SetUp(SimulationController controller)
		{
			_cam = Camera.main;
			_controller = controller;
			foreach (var activeUnit in _controller.ActiveUnits)
			{
				SpawnUnitView(activeUnit);
			}
			_controller.OnUnitDestroyed += OnUnitDestroyed;
			SetFieldSize(_controller.Config.gameAreaWidth, _controller.Config.gameAreaHeight);
		}

		void OnUnitDestroyed(UnitController unit)
		{
			var unitView = _units.Find(u => u.Controller == unit);
			if(unitView != null)
				unitView.Return();
		}

		public void StartSpawningAnimation()
		{
			StartCoroutine(UnitSpawningRoutine());
		}
				
		public Material GetWinnerMaterial()
		{
			if (_controller == null || _controller.Simulation == null || _controller.Simulation.ActiveUnits.Count == 0)
				return null;
			if (_controller.Simulation.ActiveUnits.Count > 0)
			{
				return GetColorMaterial(_controller.Simulation.ActiveUnits[0].Type);
			}
			return null;
		}

		public Material GetColorMaterial(int type)
		{
			if (type < 0 || type >= _colors.Length)
			{
				Debug.LogError("Invalid color index: " + type);
				return null;
			}
			return _colors[type];
		}
		
		public override void Render()
		{
			foreach (var unit in _units)
			{
				unit.Render();
			}
		}

		public void Dispose()
		{
			if(_controller != null)
				_controller.OnUnitDestroyed -= OnUnitDestroyed;
			if(_unitsPool != null)
				_unitsPool.Reset();
			_units.Clear();
		}
		
		public IEnumerator UnitSpawningRoutine()
		{
			var config = _controller.Config;
			var unitsLeft = config.numUnitsToSpawn;
			var simulation = _controller.Simulation;
			while (unitsLeft > 0)
			{
				Vector point;
				float rad;
				do
				{
					rad = Random.Range(config.minUnitRadius, config.maxUnitRadius);
					var x = Random.Range(-.5f, .5f) * (config.gameAreaWidth - (rad * 2));
					var y = Random.Range(-.5f, .5f) * (config.gameAreaHeight - (rad * 2));
					point = new Vector(x, y);
				} 
				while (_controller.IsOverlappingUnit(point, rad));
				
				var unit = new Unit(point, Vector.Zero, rad, Random.Range(0, _colors.Length));
				
				SpawnUnitView(_controller.CreateUnitController(unit));
				
				unitsLeft--;
				
				yield return new WaitForSeconds(TimeSpan.FromMilliseconds(config.unitSpawnDelay).Seconds);
			}

			foreach (var unit in simulation.ActiveUnits)
			{
				var speed = Random.Range(config.minUnitSpeed, config.maxUnitSpeed);
				unit.Velocity = new Vector(Random.Range(-1f, 1f), Random.Range(-1f,1f)).Normalized * speed;
			}
		}

		public void SetFieldSize(float width, float height)
		{
			_fieldTransform.localScale = new Vector3(width, height, 1);
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

		void SpawnUnitView(UnitController controller)
		{
			var unitView = _unitsPool.Pick();
			unitView.SetUp(controller, this);
			_units.Add(unitView);
		}

	}
}
