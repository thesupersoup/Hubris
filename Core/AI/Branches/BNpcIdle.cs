using System;
using UnityEngine;
using UnityEngine.AI;

namespace Hubris
{
    [Serializable]
    public class BNpcIdle : BNpcBase
    {
        // Singleton instance of this state
        public readonly static BNpcIdle Instance = new BNpcIdle();

        public override void Invoke(Npc a)
        {
            if (a.Stats.IsDead)
            {
                ChangeBranch(a, BNpcDead.Instance);
                return;
            }

            SetAnimTrigger(a, "Idle");

            // Set Speed accordingly
            SetSpeed(a, a.Params.MoveSpd * a.Params.MoveWalk);

            if (a.TimerCheck >= a.Params.ChkIdle)
            {
                a.TimerCheck = 0.0f;
                CheckEnv(a);
            }

            if(a.TargetObj != null)
            {
                ChangeBranch(a, BNpcAlert.Instance);
                return;
            }

            if (a.TimerMove >= a.Params.RoamTime)
            {
                a.TimerMove = 0.0f;

                // Set speed appropriately for walking

                // if (Random.Range(1, 20) > 10)
                   // RpcTriggerSound(Apex.SndT.IDLE, Random.Range(0.0f, Apex.SND_MAX_DELAY));

                Debug.Log("Checking if " + a.Name + " should roam...");
                if(UnityEngine.Random.Range(1, 20) > 10)
                {
                    Debug.Log(a.Name + " is attempting to roam");
                    Vector3 nPos = FindRoamPoint(a);

                    if (nPos != Vector3.zero)
                    {
                        a.SetMovePos(nPos);
                        Debug.Log(a.Name + " is roaming");
                        ChangeBranch(a, BNpcMoving.Instance);
                    }
                }
            }
        }

        public Vector3 FindRoamPoint(Npc a)
        {
            bool invalid = false;

            Vector3 roamPoint = UnityEngine.Random.insideUnitSphere * a.Params.RoamDist;

            roamPoint += a.transform.position;

            NavMesh.SamplePosition(roamPoint, out NavMeshHit point, a.Params.RoamDist, NavMesh.AllAreas);

            if (point.position.x == Mathf.Infinity || point.position.x == Mathf.NegativeInfinity)
                invalid = true;

            if (point.position.y == Mathf.Infinity || point.position.y == Mathf.NegativeInfinity)
                invalid = true;

            if (point.position.z == Mathf.Infinity || point.position.z == Mathf.NegativeInfinity)
                invalid = true;

            if (!invalid)
                return point.position;
            else
                return Vector3.zero;
        }
    }
}
