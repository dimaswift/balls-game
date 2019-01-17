using System.Collections;
using System.Collections.Generic;
using BallGame.Views;
using UnityEngine;
using UnityEngine.UI;

namespace BallGame.Hud
{
	public class HudController : MonoBehaviour
	{
		public App App
		{
			get { return _app; }
		}

		[SerializeField] App _app;
		
		HudElement[] _elements;

		void Awake()
		{
			_elements = GetComponentsInChildren<HudElement>(true);
			_app.OnSimulationFinished += UpdateInfo;
			_app.OnSimulationStarted += UpdateInfo;
			foreach (var element in _elements)
			{
				element.Init(this);
			}
		}

		void UpdateInfo()
		{
			foreach (var element in _elements)
			{
				element.UpdateInfo();
			}
		}

		public void OnLoadClick()
		{
			_app.LoadSimulationFromJson();
		}

		public void OnSaveClick()
		{
			_app.SaveSimulation();
		}

		public void OnNewClick()
		{
			_app.CreateNewSimulation();
		}

		public void OnAgainClick()
		{
			_app.CreateNewSimulation();
		}

		public T GetElement<T>() where T : HudElement
		{
			foreach (var hudElement in _elements)
			{
				if (hudElement is T)
					return hudElement as T;
			}

			return null;
		}
	}

}
