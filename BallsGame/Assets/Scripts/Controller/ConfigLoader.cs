using System;
using BallGame.Model;
using UnityEngine;

namespace BallGame.Controllers
{
	public class ConfigLoader
	{
		readonly string _jsonData;

		public ConfigLoader(string data)
		{
			_jsonData = data;
		}
		
		public SimulationConfig LoadConfig()
		{
			if (_jsonData == null)
			{
				Debug.LogError("Can't load game config! Specify json data field in GameConfigContainer!");
				return null;
			}
			try
			{
				return JsonUtility.FromJson<GameConfigData>(_jsonData).GameConfig;
			}
			catch (Exception e)
			{
				Debug.LogError("Cannot deserialize game config: " + e.Message);
			}

			return null;
		}

		[Serializable]
		public class GameConfigData
		{
			public SimulationConfig GameConfig;
		}
	}

}
