using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallGame.Utils
{
	public class Pool<T> where T : IPoolable
	{
		readonly List<T> _instances;
		readonly Func<T> _cloneHandler;
		
		public Pool(Func<T> cloneHandler, int capacity)
		{
			_instances = new List<T>(capacity);
			_cloneHandler = cloneHandler;
			for (var i = 0; i < capacity; i++)
			{
				CreateInstance();
			}
		}

		T CreateInstance()
		{
			var instance = _cloneHandler();
			instance.Init();
			instance.Return();
			_instances.Add(instance);
			return instance;
		}

		public void Reset()
		{
			foreach (var instance in _instances)
			{
				instance.Return();
			}
		}

		public T Pick()
		{
			foreach (var instance in _instances)
			{
				if (instance.IsBeingUsed() == false)
				{
					instance.Pick();
					return instance;
				}
			}
			var newInstance = CreateInstance();
			newInstance.Pick();
			return newInstance;
		}
		
	}
}
