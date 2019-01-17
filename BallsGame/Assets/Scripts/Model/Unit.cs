using System;

namespace BallGame.Model
{
	[Serializable]
	public class Unit
	{
		public Vector Position;
		public Vector Velocity;
		public float Radius;
		public int Type;
		
		public  Unit(Vector position, Vector velocity, float radius, int type)
		{
			Position = position;
			Velocity = velocity;
			Radius = radius;
			Type = type;
		}
	}
}
