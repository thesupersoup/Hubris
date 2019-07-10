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

		public override BhvStatus Invoke( BhvTree b, Npc a )
		{
			if ( a.TargetObj == null )
			{
				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			if ( b.DistTarget <= Util.GetSquare( a.Params.AwareClose ) )
			{
				a.NavAgent.ResetPath();
				a.SetMovePos( Vector3.zero );
				b.SetStatus( BhvStatus.SUCCESS );
				return b.Status;
			}

			if ( a.MovePos == Vector3.zero )
				a.SetMovePos( a.TargetPos );

			float nSpd = a.Params.MoveSpd * a.Params.MoveWalk;

			// Set Speed accordingly
			if ( a.NavAgent.speed != nSpd )
				SetSpeed( a, nSpd );

			SetAnimTrigger( a, "Walk" );

			if ( b.TimerCheck >= a.Params.ChkAlert )
			{
				if ( a.TargetPos != a.MovePos )
					a.SetMovePos( a.TargetPos );

				b.TimerCheck = 0.0f;
				CheckEnv( a );
			}

			if ( a.NavAgent.destination != a.MovePos )
				a.NavAgent.SetDestination( a.MovePos );

			return b.Status;
		}
	}
}
