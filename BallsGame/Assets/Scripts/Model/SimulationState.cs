using System;
using System.Collections.Generic;

namespace BallGame.Model
{
	[Serializable]
	public class SimulationState
	{
		public float TimePassed;
		public int Frame;
		public List<UnitState> Units = new List<UnitState>();
		
	}

}
