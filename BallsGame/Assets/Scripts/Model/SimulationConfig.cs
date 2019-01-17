using System;

namespace BallGame.Model
{
	[Serializable]
	public class SimulationConfig
	{
		public float gameAreaWidth;
		public float gameAreaHeight;
		public float unitSpawnDelay;
		public int numUnitsToSpawn;
		public float minUnitRadius;
		public float maxUnitRadius;
		public float minUnitSpeed;
		public float maxUnitSpeed;
	}
}
