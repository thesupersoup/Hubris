using System;
using UnityEngine;

namespace Hubris
{
    [Serializable]
    public class BNpcAlert : BNpcBase
    {
        // Singleton instance of this state
        public readonly static BNpcAlert Instance = new BNpcAlert();

        public override void Invoke(Npc a)
        {
            // Set Speed accordingly
            SetSpeed(a, a.Params.MoveSpd);

            if (a.TimerCheck >= a.Params.ChkAlert)
            {
                a.TimerCheck = 0.0f;
                CheckEnv(a);
            }

            if (a.TargetObj != null)
            {
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
                    a.transform.rotation = Quaternion.Slerp(a.transform.rotation,
                                        Quaternion.LookRotation(targetPos - thisPos), a.Params.RotSpd);
                }
                else
                {
                    if (!a.Anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                        SetAnimTrigger(a, "Idle");
                }
            }
            else
            {
                ChangeBranch(a, BNpcIdle.Instance);
                return;
            }
        }
    }
}
