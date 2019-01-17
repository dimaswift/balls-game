using System.Collections;
using System.Collections.Generic;
using BallGame.Controllers;
using BallGame.Model;
using BallGame.Utils;
using UnityEngine;

namespace BallGame.Views
{
	public class UnitView : View, IPoolable
	{
		public UnitController Controller
		{
			get { return _controller; }
		}
		
		[SerializeField] Transform _meshTransform;
		[SerializeField] MeshRenderer _meshRenderer;
		
		UnitController _controller;
		Transform _transform;

		public void SetUp(UnitController controller, SimulationView simulationView)
		{
			_controller = controller;
			_meshRenderer.sharedMaterial = simulationView.GetColorMaterial(_controller.Unit.Type);
			Render();
		}

		public override void Render()
		{
			if(_controller == null)
				return;
			_transform.position = new Vector3(_controller.Unit.Position.X, _controller.Unit.Position.Y);
			_meshTransform.localScale = Vector3.one * _controller.Unit.Radius * 2;
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
	}
}
