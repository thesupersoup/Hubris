using System;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Npc is aware of a target but hasn't taken action yet
	/// </summary>
	public class BNpcAlert : BNpcBase
	{
		// Singleton instance of this state
		public readonly static BNpcAlert Instance = new BNpcAlert();

		public override BhvStatus Invoke( BhvTree b, Npc a )
		{
			if ( a.TargetObj == null )
			{
				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			if (b.TimerCheck >= a.Params.ChkAlert)
			{
				b.TimerCheck = 0.0f;
				CheckEnv(a);
			}

			if (a.NavAgent.hasPath)
				a.NavAgent.ResetPath();

			Vector3 targetPos = a.TargetObj.transform.position;
			Vector3 thisPos = a.transform.position;
			Vector3 fwd = a.transform.forward;
			fwd.y = 0.0f;
			targetPos.y = 0.0f;
			thisPos.y = 0.0f;

			float angle = Vector3.Angle(fwd, (targetPos - thisPos));

			if (angle > a.Params.RotAngle)
			{
				// Reverse walk anim to differentiate from regular walking
				SetAnimTrigger(a, "WalkBack");
				TurnToward( a, targetPos );
			}
			else
			{
				if (!a.Anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
					SetAnimTrigger(a, "Idle");
			}

			return b.Status;
		}
	}
}
