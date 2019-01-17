using System;


namespace BallGame.Model
{
    [Serializable]
    public struct Vector
    {
        public static Vector Zero
        {
            get
            {
                return new Vector();
            }
        }

        public Vector Normalized
        {
            get
            {
                var l = Length;
                return l > 0 ? new Vector(X / l, Y / l) : Zero;
            }
        }

        public float Length
        {
            get
            {
                return (float)Math.Sqrt(X * X + Y * Y);
            }
        }
        
        public float X;
        public float Y;

        public Vector(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector operator *(Vector a, float v)
        {
            return new Vector(a.X * v, a.Y * v);
        }
        
        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }
        
        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }
        
        public static bool operator == (Vector a, Vector b)
        {
            return a.X == b.X && a.Y == b.Y;
        }
        
        public static bool operator != (Vector a, Vector b)
        {
            return a.X != b.X || a.Y != b.Y;
        }
        
        public static float Distance(Vector a, Vector b)
        {
            return (a - b).Length;
        }

        public static float Dot(Vector a, Vector b)
        {
            return (a.X * b.X) + (a.Y * b.Y);
        }
        
        public static float SquaredDistance(Vector a, Vector b)
        {
            return (b.X - a.X) * (b.X - a.X) +  (b.Y - a.Y) * (b.Y - a.Y);
        }
    }
}