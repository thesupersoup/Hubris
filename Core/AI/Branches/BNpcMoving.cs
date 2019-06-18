using System;
using UnityEngine;

namespace Hubris
{
    [Serializable]
    public class BNpcMoving : BNpcBase
    {
        // Singleton instance of this class
        public readonly static BNpcMoving Instance = new BNpcMoving();

        public override void Invoke(Npc a)
        {
            SetAnimTrigger(a, "Walk");

            if (a.TimerCheck >= a.Params.ChkIdle)
            {
                a.TimerCheck = 0.0f;
                CheckEnv(a);
            }

            if (a.TargetObj != null)
            {
                ChangeBranch(a, BNpcAlert.Instance);
                return;
            }

            if (a.MovePos != Vector3.zero)
            {
                if((a.transform.position - a.MovePos).sqrMagnitude > a.Params.StopDist * a.Params.StopDist)
                    a.NavAgent.SetDestination(a.MovePos);
                else
                {
                    a.NavAgent.ResetPath();
                    a.SetMovePos(Vector3.zero);
                    a.ChangeBranch(BNpcIdle.Instance);
                }
            }
            else
                a.ChangeBranch(BNpcIdle.Instance);
        }
    }
}
