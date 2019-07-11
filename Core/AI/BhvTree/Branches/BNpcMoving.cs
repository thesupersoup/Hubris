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

		public override BhvStatus Invoke( BhvTree b, Npc a )
		{
			if ( a.MovePos == Vector3.zero )
			{
				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			if ( a.TargetObj != null && b.SeeTarget )
			{
				b.SetStatus( BhvStatus.FAILURE );
				a.SetMovePos( Vector3.zero );
				return b.Status;
			}

			float moveDist = a.MoveDistSqr;

			// Need to include NavAgent.radius or else the Npc won't ever reach the MovePos
			if ( moveDist <= Util.GetSquare( a.Params.StopDist + a.NavAgent.radius ) )
			{
				a.NavAgent.ResetPath();
				a.SetMovePos( Vector3.zero );
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
				CheckEnv(a);
			}

			if( a.NavAgent.destination != a.MovePos )
				a.NavAgent.SetDestination( a.MovePos );

			return b.Status;
		}
	}
}
