using System;
using System.Collections.Generic;
using BallGame.Model;

namespace BallGame.Controller
{
	[Serializable]
	public class Unit
	{
		public UnitState State
		{
			get { return _state; }
		}
		
		UnitState _state;

		readonly List<IUnitListener> _listeners = new List<IUnitListener>();
		
		public Unit(UnitState state)
		{
			LoadState(state);
		}
		
		public Unit()
		{
			LoadState(new UnitState());
		}

		public void AddListener(IUnitListener listener)
		{
			if(_listeners.Contains(listener))
				return;
			_listeners.Add(listener);
		}
		
		public void LoadState(UnitState state)
		{
			_state = state;
		}

		void CheckBoundsCollision(Bounds bounds)
		{
			var rad = _state.Radius;
			var pos = _state.Position;
			var vel = _state.Velocity;
			var rightSide = bounds.Center.X + bounds.Extends.X;
			var leftSide = bounds.Center.X - bounds.Extends.X;
			var topSide = bounds.Center.Y + bounds.Extends.Y;
			var bottomSide = bounds.Center.Y - bounds.Extends.Y;

			if (pos.X + rad > rightSide)
			{
				pos.X = rightSide - rad;
				vel.X = vel.X *= -1;
			}

			if (pos.X - rad < leftSide)
			{
				pos.X = leftSide + rad;
				vel.X = vel.X *= -1;
			}

			if (pos.Y + rad > topSide)
			{
				pos.Y = topSide - rad;
				vel.Y = vel.Y *= -1;
			}

			if (pos.Y - rad < bottomSide)
			{
				pos.Y = bottomSide + rad;
				vel.Y = vel.Y *= -1;
			}

			_state.Position = pos;
			_state.Velocity = vel;
		}

		public bool IsOverlappingAABB(Vector otherPos, float otherRad)
		{
			var pos = _state.Position;
			var rad = _state.Radius;
			if (pos.X + rad + otherRad > otherPos.X 
			    && pos.X < otherPos.X + rad + otherRad
			    && pos.Y + rad + otherRad > otherPos.Y 
			    && otherPos.Y < otherPos.Y + rad + otherRad)
			{
				return true;
			}

			return false;
		}

		public void SetVelocity(Vector vel)
		{
			_state.Velocity = vel;
		}

		public void Destroy()
		{
			foreach (var unitListener in _listeners)
			{
				unitListener.OnUnitDestroyed();
			}
		}
		
		public void SetPosition(Vector pos)
		{
			_state.Position = pos;
		}
		
		public void SetRadius(float radius)
		{
			_state.Radius = radius;
		}
		
		public void UpdatePositions(Simulation simulation, float deltaTime)
		{
			_state.Position += _state.Velocity * deltaTime;
		}

		public bool IsOverlappingCircle(Vector point, float rad)
		{
			var distance = Vector.SquaredDistance(point, _state.Position);
			return distance <= (_state.Radius + rad) * (_state.Radius + rad);
		}
		
		public void UpdateCollisions(Simulation simulation, float deltaTime)
		{
			for (var i = 0; i <	simulation.ActiveUnits.Count; i++)
			{
				var otherUnit = simulation.ActiveUnits[i];
				if(otherUnit == this)
					continue;

				var otherUnitPos = otherUnit.State.Position;
				var otherUnitRad = otherUnit.State.Radius;
				if (IsOverlappingAABB(otherUnitPos, otherUnitRad))
				{
					if (IsOverlappingCircle(otherUnitPos, otherUnitRad))
					{
						simulation.ResolveCollision(this, otherUnit, deltaTime);
					}
				}
			}
			
			var bounds = simulation.Bounds;
			CheckBoundsCollision(bounds);

		}
	}

	public interface IUnitListener
	{
		void OnUnitDestroyed();
	}

}
