using System;

namespace BallGame.Model
{
	[Serializable]
	public struct UnitState
	{
		public Vector Position;
		public Vector Velocity;
		public float Radius;
		public int Type;

		public UnitState(Vector position, Vector velocity, float radius, int type)
		{
			Position = position;
			Velocity = velocity;
			Radius = radius;
			Type = type;
		}
	}
}
