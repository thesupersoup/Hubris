using System;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Npc is moving toward a destination without a target
	/// </summary>
	public class BNpcMoving : BNpcBase
	{
		// Singleton instance of this class
		public readonly static BNpcMoving Instance = new BNpcMoving();

		public override BhvStatus Invoke( Npc a, BhvTree b )
		{
			if ( a.MovePos == Vector3.zero )
			{
				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			if ( a.TargetObj != null && b.SeeTarget )
			{
				StopMove( a );
				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			if ( a.TargetObj == null && a.NavAgent.hasPath )
			{
				// Sometimes the calculated destination on the nav mesh isn't exactly where we wanted to move
				if ( a.MovePos != a.NavAgent.destination )
					a.SetMovePos( a.NavAgent.destination );

				// Something caught our attention midmove
				if ( a.TargetPos != Vector3.zero )
				{
					// Debug.Log( "Something caught our attention midmove" );
					StopMove( a );
					b.SetStatus( BhvStatus.FAILURE );
					return b.Status;
				}
			}

			float moveDist = a.MoveDistSqr;

			// Need to include NavAgent.radius or else the Npc won't ever reach the MovePos
			if ( moveDist <= Util.GetSquare( a.Params.StopDist + a.NavAgent.radius ) )
			{
				StopMove( a );
				b.SetStatus( BhvStatus.SUCCESS );
				return b.Status;
			}

			float nSpd = a.Params.MoveSpd * a.Params.MoveWalk;

			// Set Speed accordingly
			if ( a.NavAgent.speed != nSpd )
				SetSpeed( a, nSpd );

			if( !b.AnimInfo.IsName( AnimString.WALK ) )
				SetAnimTrigger( a, AnimString.WALK );

			if ( b.TimerCheck >= a.Params.ChkIdle )
			{
				b.TimerCheck = 0.0f;
				CheckEnv( a, b );
			}

			if ( a.NavAgent.destination != a.MovePos )
				StartMove( a, a.MovePos );

			return b.Status;
		}
	}
}
