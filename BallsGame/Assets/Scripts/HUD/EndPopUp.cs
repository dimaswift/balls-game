using System;
using UnityEngine;
using UnityEngine.UI;

namespace BallGame.Hud
{
	public class EndPopUp : HudElement
	{
		[SerializeField] Image _winnerColorImage;
		[SerializeField] Text _durationText;

		protected override void OnInit(HudController hudController)
		{
			hudController.App.OnSimulationFinished += OnSimFinished;
			gameObject.SetActive(false);
		}
		
		public void OnAgainClick()
		{
			Controller.OnAgainClick();
			gameObject.SetActive(false);
			Controller.GetElement<ButtonsPanel>().gameObject.SetActive(true);
		}

		void OnSimFinished()
		{
			var winnerMat = Controller.App.SimulationView.GetWinnerMaterial();
			_winnerColorImage.color = winnerMat ? winnerMat.color : Color.black;
			var simTime = TimeSpan.FromSeconds(Controller.App.SimulationController.Simulation.TimePassed);
			_durationText.text = string.Format(@"{0:00}:{1:00}", simTime.Minutes, simTime.Seconds);
			gameObject.SetActive(true);
			Controller.GetElement<ButtonsPanel>().gameObject.SetActive(false);
		}

	}

}
