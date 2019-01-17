using System;
using BallGame.Model;
using UnityEngine;

namespace BallGame.Controller
{
	public class ConfigLoader
	{
		readonly TextAsset _jsonData;

		public ConfigLoader(TextAsset data)
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
				return JsonUtility.FromJson<GameConfigData>(_jsonData.text).GameConfig;
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
