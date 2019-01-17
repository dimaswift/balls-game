using System.Collections;
using System.Collections.Generic;
using BallGame.Model;

namespace BallGame.Controller
{
	public class Simulation
	{
		public float Speed;
		
		public List<Unit> ActiveUnits
		{
			get { return _activeUnits; }
		}

		public Bounds Bounds
		{
			get { return _bounds; }
		}
		
		public SimulationState State
		{
			get
			{
				if (_state == null)
					_state = CreateDefaultState();
				return _state;
			}
		}
		
		readonly List<Unit> _activeUnits = new List<Unit>();

		readonly SimulationConfig _config;
		readonly Bounds _bounds;
		readonly IUnitCollisionHandler _collisionHandler;
		SimulationState _state;
		
		public Simulation(SimulationState state, SimulationConfig config, IUnitCollisionHandler collisionHandler)
		{
			Speed = 1;
			_collisionHandler = collisionHandler;
			_config = config;
			_bounds = new Bounds()
			{
				Extends = new Vector(config.gameAreaWidth, config.gameAreaHeight) * .5f,
				Center = Vector.Zero
			};
			LoadState(state);
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

		public void LoadState(SimulationState state)
		{
			if (state == null)
				state = CreateDefaultState();
			_state = state;
			_activeUnits.Clear();

			foreach (var unitState in state.Units)
			{
				var unit = new Unit(unitState);
				_activeUnits.Add(unit);
			}
		}

		public void AddUnit(Unit unit)
		{
			_activeUnits.Add(unit);
		}

		public void ResolveCollision(Unit a, Unit b, float deltaTime)
		{
			if (_collisionHandler.CanCollide(a, b) == false)
			{
				_collisionHandler.OnUnitsGoThrough(a, b);
				return;
			}
			
			var posA = a.State.Position;
			var posB = b.State.Position;

			var velA = a.State.Velocity;
			var velB = b.State.Velocity;

			var tangentVector = new Vector(posB.Y - posA.Y, -(posB.X - posA.X )).Normalized;
			var relativeVelocity = velA - velB;
			var length = Vector.Dot(relativeVelocity, tangentVector);
			var impulseVel = relativeVelocity - (tangentVector * length);

			b.SetVelocity(velB + impulseVel);
			b.SetPosition(posB + (b.State.Velocity * deltaTime));
			
			a.SetVelocity(velA - impulseVel);
			a.SetPosition(posA + (a.State.Velocity * deltaTime));
			_collisionHandler.OnUnitsCollision(a, b);
		}

		public float GetOverlappingDistance(Unit a, Unit b)
		{
			var posA = a.State.Position;
			var posB = b.State.Position;
			return (a.State.Radius + b.State.Radius) - Vector.Distance(posA, posB);
		}

		public float GetTotalRadius(int type)
		{
			var total = 0f;
			foreach (var unit in _activeUnits)
			{
				if (unit.State.Type == type)
				{
					total += unit.State.Radius;
				}
			}
			return total;
		}
		
		public float GetTotalRadius()
		{
			var total = 0f;
			foreach (var unit in _activeUnits)
			{
				total += unit.State.Radius;
			}
			return total;
		}

		public void DestroyUnit(Unit unit)
		{
		
			_activeUnits.RemoveAll(u => u == unit);
			unit.Destroy();
		}
		
		public void Tick(float deltaTime)
		{
			if (Speed < 0)
				Speed = 0;
			for (var i = 0; i <	_activeUnits.Count; i++)
			{
				_activeUnits[i].UpdatePositions(this, deltaTime * Speed);
			}

			for (var i = 0; i <	_activeUnits.Count; i++)
			{
				_activeUnits[i].UpdateCollisions(this, deltaTime * Speed);
			}

			_state.TimePassed += deltaTime * Speed;
			_state.Frame++;
		}

		public SimulationState SaveState()
		{
			_state.Units.Clear();

			foreach (var activeUnit in _activeUnits)
			{
				_state.Units.Add(activeUnit.State);
			}
			return _state;
		}
		
		SimulationState CreateDefaultState()
		{
			var state = new SimulationState();
			state.Units = new List<UnitState>();
			state.Frame = 0;
			state.TimePassed = 0;
			return state;
		}
	}
}
