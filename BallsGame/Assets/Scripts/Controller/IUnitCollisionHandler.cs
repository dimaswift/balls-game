namespace BallGame.Controller
{
    public interface IUnitCollisionHandler
    {
        bool CanCollide(Unit a, Unit b);
        void OnUnitsGoThrough(Unit a, Unit b);
        void OnUnitsCollision(Unit a, Unit b);
    }
}