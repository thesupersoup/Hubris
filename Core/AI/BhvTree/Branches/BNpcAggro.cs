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

			float tarDist = a.TargetDistSqr;

			if ( tarDist <= Util.GetSquare( a.Params.AtkDist ) )
			{
				b.SetStatus( BhvStatus.SUCCESS );
				return b.Status;
			}

			if ( a.MovePos == Vector3.zero )
				a.SetMovePos( a.TargetPos );

			float nSpd = a.Params.MoveSpd;

			// Set Speed accordingly
			if ( a.NavAgent.speed != nSpd )
				SetSpeed( a, nSpd );

			SetAnimTrigger( a, "Run" );

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
