using UnityEngine;
using UnityEngine.UI;


namespace BallGame.Hud
{
    public class ButtonsPanel : HudElement
    {
        public float Speed
        {
            get { return Controller.App.SimulationController.Speed; }
            set { Controller.App.SimulationController.Speed = value; }
        }
        
        [SerializeField] Slider _simulationSpeedSlider;

        protected override void OnInit(HudController hudController)
        {
            
        }

        public override void UpdateInfo()
        {
            Controller.App.SimulationController.Speed = _simulationSpeedSlider.value;
        }
    }
}