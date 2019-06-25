using System;
using UnityEngine;

namespace Hubris
{
	[Serializable]
	public class BNpcMoving : BNpcBase
	{
		// Singleton instance of this class
		public readonly static BNpcMoving Instance = new BNpcMoving();

		public override void Invoke( BhvTree b, Npc a )
		{
			if ( a.MovePos == Vector3.zero )
			{
				Status = BhvStatus.FAILURE;
				return;
			}

			if ( a.TargetObj != null )
			{
				Status = BhvStatus.FAILURE;
				a.SetMovePos( Vector3.zero );
				return;
			}

			SetAnimTrigger(a, "Walk");

			if (b.TimerCheck >= a.Params.ChkIdle)
			{
				b.TimerCheck = 0.0f;
				CheckEnv(a);
			}

			if( Util.CheckDistSqr( a.transform.position, a.MovePos ) <= a.Params.StopDist * a.Params.StopDist )
			{
				a.NavAgent.ResetPath();
				a.SetMovePos(Vector3.zero);
				Status = BhvStatus.FAILURE;
				return;
			}

			a.NavAgent.SetDestination( a.MovePos );

			Status = BhvStatus.SUCCESS;
		}
	}
}
