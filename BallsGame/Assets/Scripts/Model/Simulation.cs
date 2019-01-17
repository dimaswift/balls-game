using System;
using System.Collections.Generic;

namespace BallGame.Model
{
	[Serializable]
	public class Simulation
	{
		public List<Unit> ActiveUnits;
		public float TimePassed;
		public int Frame;

		public Simulation()
		{
			ActiveUnits = new List<Unit>();
		}
	}
}
