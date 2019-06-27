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

		public override BhvStatus Invoke( BhvTree b, Npc a )
		{
			if ( a.TargetObj != null || a.MovePos != Vector3.zero)
			{
				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			if ( b.TimerMove >= a.Params.RoamTime )
			{
				b.TimerMove = 0.0f;

				// Set speed appropriately for walking

				// if (Random.Range(1, 20) > 10)
				// RpcTriggerSound(Apex.SndT.IDLE, Random.Range(0.0f, Apex.SND_MAX_DELAY));

				Debug.Log( "Checking if " + a.Name + " should roam..." );
				if ( UnityEngine.Random.Range( 1, 20 ) > 10 )
				{
					Debug.Log( a.Name + " is attempting to roam" );
					Vector3 nPos = FindRoamPoint( a );

					if ( nPos != Vector3.zero )
					{
						// Set Speed accordingly
						SetSpeed( a, a.Params.MoveSpd * a.Params.MoveWalk );

						a.SetMovePos( nPos );
						Debug.Log( a.Name + " is roaming" );
						b.SetStatus( BhvStatus.SUCCESS );
						return b.Status;
					}
				}
			}

			SetAnimTrigger(a, "Idle");

			if (b.TimerCheck >= a.Params.ChkIdle)
			{
				b.TimerCheck = 0.0f;
				CheckEnv(a);
			}

			return b.Status;
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
