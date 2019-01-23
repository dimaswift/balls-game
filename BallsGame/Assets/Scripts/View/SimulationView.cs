using System.Collections.Generic;
using BallGame.Controllers;
using BallGame.Model;
using BallGame.Utils;
using UnityEngine;

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
			_controller.OnUnitSpawned += SpawnUnitView;
			SetFieldSize(_controller.Config.gameAreaWidth, _controller.Config.gameAreaHeight);
		}

		void OnUnitDestroyed(UnitController unit)
		{
			var unitView = _units.Find(u => u.Controller == unit);
			if(unitView != null)
				unitView.Return();
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
