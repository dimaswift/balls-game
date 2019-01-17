using System;
using System.Collections.Generic;
using BallGame.Model;

namespace BallGame.Controllers
{
	public class SimulationController
	{
		public float Speed;
		
		public SimulationConfig Config
		{
			get { return _config; }
		}

		public Simulation Simulation
		{
			get { return _simulation; }
		}

		public List<UnitController> ActiveUnits
		{
			get { return _activeUnits; }
		}

		public event Action OnSimulationFinished;
		public event Action<UnitController> OnUnitDestroyed;
		
		readonly SimulationConfig _config;
		readonly Simulation _simulation;
		readonly List<int> _intValuesBuffer = new List<int>();
		readonly List<UnitController> _activeUnits = new List<UnitController>();
		readonly float _minRadius;
		readonly Bounds _bounds;
		
		public SimulationController(SimulationConfig config, Simulation simulation, float minRadius)
		{
			Speed = 1;
			_minRadius = minRadius;
			_config = config;
			_simulation = simulation;
			_bounds = new Bounds(config.gameAreaWidth / 2, config.gameAreaHeight / 2);
			foreach (var unit in _simulation.ActiveUnits)
			{
				var controller = new UnitController(unit);
				_activeUnits.Add(controller);
			}
		}
		
		public void Tick(float deltaTime)
		{
			if (Speed < 0)
				Speed = 0;

			UpdatePositions(deltaTime * Speed);
			UpdateCollisions(deltaTime * Speed);

			_simulation.TimePassed += deltaTime;
			_simulation.Frame++;
		}

		public bool IsOverlappingUnit(Vector point, float radius)
		{
			foreach (var unit in _activeUnits)
			{
				if (unit.IsOverlappingAABB(point, radius))
				{
					if (unit.IsOverlappingCircle(point, radius))
					{
						return true;
					}
				}
			}

			return false;
		}

		public UnitController CreateUnitController(Unit unit)
		{
			var controller = new UnitController(unit);
			_simulation.ActiveUnits.Add(unit);
			_activeUnits.Add(controller);
			return controller;
		}

		public float GetTotalRadius(int type)
		{
			var total = 0f;
			foreach (var unit in _activeUnits)
			{
				if (unit.Unit.Type == type)
				{
					total += unit.Unit.Radius;
				}
			}
			return total;
		}
		
		public float GetTotalRadius()
		{
			var total = 0f;
			foreach (var unit in _activeUnits)
			{
				total += unit.Unit.Radius;
			}
			return total;
		}
		
		bool CanCollide(UnitController a, UnitController b)
		{
			return a.Unit.Type == b.Unit.Type;
		}
		
		void OnUnitsCollision(UnitController a, UnitController b)
		{
			//TODO: add some  collision logic
		}

		void OnUnitsGoThrough(UnitController a, UnitController b)
		{
			var dist = GetOverlappingDistance(a, b) / 2;
			a.Unit.Radius -= dist;
			b.Unit.Radius -= dist;
		
			if (a.Unit.Radius < _minRadius || b.Unit.Radius < _minRadius)
			{
				DestroyUnit(a);
				DestroyUnit(b);
			}
		}

		void ResolveCollision(UnitController a, UnitController b, float deltaTime)
		{
			if (CanCollide(a, b) == false)
			{
				OnUnitsGoThrough(a, b);
				return;
			}
			
			var posA = a.Unit.Position;
			var posB = b.Unit.Position;

			var velA = a.Unit.Velocity;
			var velB = b.Unit.Velocity;

			var tangentVector = new Vector(posB.Y - posA.Y, -(posB.X - posA.X )).Normalized;
			var relativeVelocity = velA - velB;
			var length = Vector.Dot(relativeVelocity, tangentVector);
			var impulseVel = relativeVelocity - (tangentVector * length);

			b.Unit.Velocity += impulseVel;
			b.Unit.Position += b.Unit.Velocity * deltaTime;
			
			a.Unit.Velocity -= impulseVel;
			a.Unit.Position += a.Unit.Velocity * deltaTime;
			
			OnUnitsCollision(a, b);
		}

		float GetOverlappingDistance(UnitController a, UnitController b)
		{
			var posA = a.Unit.Position;
			var posB = b.Unit.Position;
			return (a.Unit.Radius + b.Unit.Radius) - Vector.Distance(posA, posB);
		}

		void DestroyUnit(UnitController unit)
		{
			_simulation.ActiveUnits.RemoveAll(u => u == unit.Unit);
			_activeUnits.RemoveAll(u => u == unit);
			if (OnUnitDestroyed != null)
				OnUnitDestroyed(unit);
			if (IsSimulationOver())
			{
				if(OnSimulationFinished != null)
					OnSimulationFinished.Invoke();
			}
		}

		void UpdatePositions(float deltaTime)
		{
			for (var i = 0; i <	_activeUnits.Count; i++)
			{
				_activeUnits[i].UpdatePositions(deltaTime * Speed);
			}
		}
		
		void UpdateCollisions(float deltaTime)
		{
			for (var i = 0; i <	_activeUnits.Count; i++)
			{
				var unitA = _activeUnits[i];
				unitA.CheckBoundsCollision(_bounds);
				for (var j = 0; j <	_activeUnits.Count; j++)
				{
					if(i == j)
						continue;
					var unitB = _activeUnits[j];
				
					var unitAPos = unitA.Unit.Position;
					var unitARad = unitA.Unit.Radius;
					if (unitB.IsOverlappingAABB(unitAPos, unitARad))
					{
						if (unitB.IsOverlappingCircle(unitAPos, unitARad))
						{
							ResolveCollision(unitA, unitB, deltaTime);
						}
					}
				}
			}
		}

		bool IsSimulationOver()
		{
			_intValuesBuffer.Clear();
			foreach (var activeUnit in _simulation.ActiveUnits)
			{
				if(_intValuesBuffer.FindIndex(t => activeUnit.Type == t) < 0)
					_intValuesBuffer.Add(activeUnit.Type);
			}
			return _intValuesBuffer.Count <= 1;
		}
	}
}
