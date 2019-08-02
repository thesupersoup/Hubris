using System;
using UnityEngine;
using UnityEngine.AI;

namespace Hubris
{
	/// <summary>
	/// Npc is fleeing from a target
	/// </summary>
	public class BNpcFlee : BNpcBase
	{
		// Singleton instance of this class
		public readonly static BNpcFlee Instance = new BNpcFlee();

		public override BhvStatus Invoke( Npc a, BhvTree b )
		{
			if ( a.TargetObj == null )
			{
				b.SetStatus( BhvStatus.SUCCESS );
				return b.Status;
			}

			if ( a.TargetEnt?.Stats.IsDead ?? false )
			{
				StopMove( a );
				a.ResetTargetObj();
				b.SetStatus( BhvStatus.SUCCESS );
				return b.Status;
			}

			if ( !b.PathFailed && b.TimerCheck >= a.Params.ChkAlert )
			{
				b.TimerCheck = 0.0f;
				CheckEnv( a, b );
			}

			// Set Speed accordingly
			if ( a.NavAgent.speed != a.Params.MoveSpd )
				SetSpeed( a, a.Params.MoveSpd );

			if ( a.MovePos == Vector3.zero )
			{
				Vector3 nPos = FindFleePoint( a );

				if ( nPos == Vector3.zero )
				{
					// Debug.Log( "Unable to find flee point" );
					if ( !b.AnimInfo.IsName( AnimString.IDLE ) )
						SetAnimTrigger( a, AnimString.IDLE );
					return BhvStatus.RUNNING;
				}

				// Debug.Log( "Found flee point" );
				a.SetMovePos( nPos );
			}

			if ( a.NavAgent.pathPending )
				return BhvStatus.RUNNING;

			if ( !a.NavAgent.hasPath )
				StartMove( a, a.MovePos );
			else
			{
				if ( !b.AnimInfo.IsName( AnimString.RUN ) )
					SetAnimTrigger( a, AnimString.RUN );

				if ( a.MovePos != a.NavAgent.destination )
					a.SetMovePos( a.NavAgent.destination );

				// TurnToward( a, a.NavAgent.steeringTarget );
			}

			// Need to include NavAgent.radius or else the Npc won't ever reach the MovePos
			if ( b.DistMove <= Util.GetSquare( a.Params.StopDist + a.NavAgent.radius ) && a.NavAgent.hasPath )
			{
				if ( ( a.TargetDistSqr > Util.GetSquare( a.Params.AwareMax + 10.0f ) ) )
				{
					StopMove( a );
					// Debug.Log( "Safe distance from target, flight over" );
					a.ResetTargetObj();
					b.SetPathFailed( false );
					b.SetStatus( BhvStatus.SUCCESS );
					return b.Status;
				}
				else
				{
					StopMove( a );
					// Debug.Log( "Resetting move for recalculation" );
				}
			}

			return b.Status;
		}
	}
}