using System;
using System.Collections;
using System.Collections.Generic;
using BallGame.View;
using UnityEngine;
using UnityEngine.UI;

namespace BallGame.HUD
{
	public class EndPopUp : MonoBehaviour
	{
		[SerializeField] GameObject _controlPanel;
		[SerializeField] Image _winnerColorImage;
		[SerializeField] Text _durationText;

		void Start()
		{
			SimulationView.Instance.OnSimulationFinished.AddListener(OnSimFinished);
			gameObject.SetActive(false);
		}
		
		public void OnAgainClick()
		{
			SimulationView.Instance.CreateNewSimulation();
			gameObject.SetActive(false);
			_controlPanel.SetActive(true);
		}

		void OnSimFinished()
		{
			var winnerMat = SimulationView.Instance.GetWinnerMaterial();
			_winnerColorImage.color = winnerMat ? winnerMat.color : Color.black;
			var simTime = TimeSpan.FromSeconds(SimulationView.Instance.Simulation.State.TimePassed);
			_durationText.text = string.Format(@"{0:00}:{1:00}", simTime.Minutes, simTime.Seconds);
			gameObject.SetActive(true);
			_controlPanel.SetActive(false);
		}
	}

}
