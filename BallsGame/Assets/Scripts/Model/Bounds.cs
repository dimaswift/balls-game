using System;

namespace BallGame.Model
{
    [Serializable]
    public struct Bounds
    {
        public Vector Center;
        public Vector Extends;

        public Bounds(float width, float height)
        {
            Extends = new Vector(width, height);
            Center = Vector.Zero;
        }
    }
}