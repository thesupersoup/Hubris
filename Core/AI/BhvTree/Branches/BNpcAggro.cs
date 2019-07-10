using System;
using UnityEngine;
using UnityEngine.AI;

namespace Hubris
{
	/// <summary>
	/// Npc is aggressively moving toward a target
	/// </summary>
	public class BNpcAggro : BNpcBase
	{
		// Singleton instance of this state
		public readonly static BNpcAggro Instance = new BNpcAggro();

		public override BhvStatus Invoke( BhvTree b, Npc a )
		{
			if ( a.TargetObj == null )
			{
				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			if ( b.DistTarget <= Util.GetSquare( a.Params.AtkDist ) )
			{
				b.SetStatus( BhvStatus.SUCCESS );
				return b.Status;
			}

			a.SetMovePos( a.TargetPos );

			float nSpd = a.Params.MoveSpd;

			AnimatorStateInfo animInfo = a.Anim.GetCurrentAnimatorStateInfo( 0 );

			if ( animInfo.IsName( "Attack" ) )
				nSpd *= a.Params.AtkSlow;
			else
				SetAnimTrigger( a, "Run" );

			// Set Speed accordingly
			if ( a.NavAgent.speed != nSpd )
				SetSpeed( a, nSpd );

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
