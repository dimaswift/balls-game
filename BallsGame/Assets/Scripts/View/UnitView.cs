using System.Collections;
using System.Collections.Generic;
using BallGame.Controller;
using BallGame.Utils;
using UnityEngine;

namespace BallGame.View
{
	public class UnitView : MonoBehaviour, IPoolable, IUnitListener
	{
		[SerializeField] Transform _meshTransform;
		[SerializeField] MeshRenderer _meshRenderer;
		
		Unit _unit;
		Transform _transform;
		SimulationView _simulationView;
		
		public void SetUp(Unit unit, SimulationView view)
		{
			_simulationView = view;
			_unit = unit;
			_unit.AddListener(this);
			_meshRenderer.sharedMaterial = view.GetMaterialType(_unit.State.Type);
			UpdateTransform();
		}

		public void UpdateTransform()
		{
			if(_unit == null)
				return;
			_transform.position = new Vector3(_unit.State.Position.X, _unit.State.Position.Y);
			_meshTransform.localScale = Vector3.one * _unit.State.Radius * 2;
		}

		public void Init()
		{
			_transform = transform;
		}

		public void Pick()
		{
			gameObject.SetActive(true);
		}

		public bool IsBeingUsed()
		{
			return gameObject.activeSelf;
		}

		public void Return()
		{
			gameObject.SetActive(false);
		}

		public void OnUnitDestroyed()
		{
			Return();
			_simulationView.OnUnitDestroyed(this);
		}
	}

}
