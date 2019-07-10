using System;
using UnityEngine;
using UnityEngine.AI;

namespace Hubris
{
	public class BNpcAtk : BNpcBase
	{
		// Singleton instance of this state
		public readonly static BNpcAtk Instance = new BNpcAtk();

		public override BhvStatus Invoke( BhvTree b, Npc a )
		{
			if ( a.TargetObj == null )
			{
				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			AnimatorStateInfo animInfo = a.Anim.GetCurrentAnimatorStateInfo( 0 );

			if ( b.DistTarget > Util.GetSquare( a.Params.AtkDist ) )
			{
				if ( animInfo.IsName( "Attack" ) )
					a.Anim.ResetTrigger( "Attack" );

				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			Vector3 targetPos = a.TargetPos;
			Vector3 thisPos = a.transform.position;
			Vector3 fwd = a.transform.forward;
			fwd.y = 0.0f;
			targetPos.y = 0.0f;
			thisPos.y = 0.0f;

			float angle = Vector3.Angle( fwd, (targetPos - thisPos) );

			if ( angle > a.Params.RotAngle )
			{
				TurnToward( a, targetPos );
			}

			a.SetMovePos( targetPos );

			float nSpd = a.Params.MoveSpd;

			if ( animInfo.IsName( "Attack" ) )
				nSpd *= a.Params.AtkSlow;

			// Set Speed accordingly
			if ( a.NavAgent.speed != nSpd )
				SetSpeed( a, nSpd );

			SetAnimTrigger( a, "Attack" );

			if ( b.TimerCheck >= a.Params.ChkAlert )
			{ 
				b.TimerCheck = 0.0f;
				CheckEnv( a );
			}

			if ( a.NavAgent.destination != a.MovePos )
				a.NavAgent.SetDestination( a.MovePos );

			return b.Status;
		}
	}
}
