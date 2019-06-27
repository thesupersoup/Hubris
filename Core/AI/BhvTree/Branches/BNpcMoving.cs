using System;
using UnityEngine;

namespace Hubris
{
	[Serializable]
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

			if ( a.TargetObj != null )
			{
				b.SetStatus( BhvStatus.FAILURE );
				a.SetMovePos( Vector3.zero );
				return b.Status;
			}

			if ( Util.CheckDistSqr( a.transform.position, a.MovePos ) <= a.Params.StopDist * a.Params.StopDist )
			{
				a.NavAgent.ResetPath();
				a.SetMovePos( Vector3.zero );
				b.SetStatus( BhvStatus.SUCCESS );
				return b.Status;
			}

			SetAnimTrigger(a, "Walk");

			if (b.TimerCheck >= a.Params.ChkIdle)
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
