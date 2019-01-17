using System.Collections;
using System.Collections.Generic;
using BallGame.View;
using UnityEngine;
using UnityEngine.UI;

namespace BallGame.HUD
{
	public class ControlPanel : MonoBehaviour
	{
		public float Speed
		{
			get { return SimulationView.Instance.Speed; }
			set { SimulationView.Instance.Speed = value; }
		}
		
		[SerializeField] Slider _simulationSpeedSlider;

		void Start()
		{
			_simulationSpeedSlider.value = SimulationView.Instance.Speed;
		}

		public void OnLoadClick()
		{
			SimulationView.Instance.LoadSimulationFromJson();
		}

		public void OnSaveClick()
		{
			SimulationView.Instance.SaveSimulation();
		}

		public void OnNewClick()
		{
			SimulationView.Instance.CreateNewSimulation();
		}
	}

}
