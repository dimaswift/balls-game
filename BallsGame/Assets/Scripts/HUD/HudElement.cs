using UnityEngine;

namespace BallGame.Hud
{
    public abstract class HudElement : MonoBehaviour
    {
        protected HudController Controller;

        public void Init(HudController hudController)
        {
            Controller = hudController;
            OnInit(hudController);
        }

        public virtual void UpdateInfo()
        {
            
        }
        
        protected abstract void OnInit(HudController hudController);
        
    }
}