using System;
using UnityEngine;
using UnityEngine.AI;

namespace Hubris
{
	/// <summary>
	/// Npc is hunting a target
	/// </summary>
	public class BNpcHunt : BNpcBase
	{
		// Singleton instance of this state
		public readonly static BNpcHunt Instance = new BNpcHunt();

		public override BhvStatus Invoke( Npc a, BhvTree b )
		{
			if ( a.TargetObj == null )
			{
				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			if ( a.TargetEnt?.Stats.IsDead ?? false )
			{
				// We don't reset the move here because we still want to investigate what we were hunting
				a.ResetTargetObj();
				b.SetStatus( BhvStatus.SUCCESS );
				return b.Status;
			}

			if ( !b.SeeTarget )
			{
				// Set MovePos to last known TargetPos
				a.SetMovePos( a.TargetPos );
				a.ResetTargetObj();
				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			if ( b.DistTarget > Util.GetSquare( a.Params.AwareMax ) )
			{
				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			if ( b.DistTarget <= Util.GetSquare( a.Params.AwareClose ) )
			{
				StopMove( a );
				b.SetStatus( BhvStatus.SUCCESS );
				return b.Status;
			}

			if ( a.MovePos == Vector3.zero )
				a.SetMovePos( a.TargetPos );

			if ( a.NavAgent.hasPath )
			{
				// Sometimes the calculated destination on the nav mesh isn't exactly where we wanted to move
				if ( a.MovePos != a.NavAgent.destination )
					a.SetMovePos( a.NavAgent.destination );
			}

			float nSpd = a.Params.MoveSpd * a.Params.MoveWalk;

			// Set Speed accordingly
			if ( a.NavAgent.speed != nSpd )
				SetSpeed( a, nSpd );

			if( !b.AnimInfo.IsName( AnimString.WALK ) )
				SetAnimTrigger( a, AnimString.WALK );

			if ( b.TimerCheck >= a.Params.ChkAlert )
			{
				if ( a.TargetPos != a.MovePos )
					a.SetMovePos( a.TargetPos );

				b.TimerCheck = 0.0f;
				CheckEnv( a, b );
			}

			if ( a.NavAgent.destination != a.MovePos )
				a.NavAgent.SetDestination( a.MovePos );

			return b.Status;
		}
	}
}
