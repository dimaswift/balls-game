using BallGame.Model;

namespace BallGame.Controllers
{
    public class UnitController
    {
        public Unit Unit { get; private set; }

        public UnitController(Unit unit)
        {
            Unit = unit;
        }
        
        public void UpdatePositions(float deltaTime)
        {
            Unit.Position += Unit.Velocity * deltaTime;
        }

        public bool IsOverlappingCircle(Vector point, float rad)
        {
            var distance = Vector.SquaredDistance(point, Unit.Position);
            return distance <= (Unit.Radius + rad) * (Unit.Radius + rad);
        }

        public void CheckBoundsCollision(Bounds bounds)
        {
            var rad = Unit.Radius;
            var pos = Unit.Position;
            var vel = Unit.Velocity;
            var rightSide = bounds.Center.X + bounds.Extends.X;
            var leftSide = bounds.Center.X - bounds.Extends.X;
            var topSide = bounds.Center.Y + bounds.Extends.Y;
            var bottomSide = bounds.Center.Y - bounds.Extends.Y;

            if (pos.X + rad > rightSide)
            {
                pos.X = rightSide - rad;
                vel.X = vel.X *= -1;
            }

            if (pos.X - rad < leftSide)
            {
                pos.X = leftSide + rad;
                vel.X = vel.X *= -1;
            }

            if (pos.Y + rad > topSide)
            {
                pos.Y = topSide - rad;
                vel.Y = vel.Y *= -1;
            }

            if (pos.Y - rad < bottomSide)
            {
                pos.Y = bottomSide + rad;
                vel.Y = vel.Y *= -1;
            }

            Unit.Position = pos;
            Unit.Velocity = vel;
        }

        public bool IsOverlappingAABB(Vector otherPos, float otherRad)
        {
            var pos = Unit.Position;
            var rad = Unit.Radius;
            if (pos.X + rad + otherRad > otherPos.X 
                && pos.X < otherPos.X + rad + otherRad
                && pos.Y + rad + otherRad > otherPos.Y 
                && otherPos.Y < otherPos.Y + rad + otherRad)
            {
                return true;
            }

            return false;
        }

	
    }

}