﻿using System;
using UnityEngine;
using UnityEngine.AI;

namespace Hubris
{
	/// <summary>
	/// Npc is fleeing from a target
	/// </summary>
	public class BNpcFlee : BNpcBase
	{
		// Singleton instance of this class
		public readonly static BNpcFlee Instance = new BNpcFlee();

		public override BhvStatus Invoke( BhvTree b, Npc a )
		{
			if ( a.TargetObj == null )
			{
				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			if ( a.MovePos == Vector3.zero )
			{
				Vector3 nPos = FindFleePoint( a );

				if ( nPos == Vector3.zero )
				{
					b.SetStatus( BhvStatus.FAILURE );
					return b.Status;
				}

				// Set Speed accordingly
				SetSpeed( a, a.Params.MoveSpd );
				a.SetMovePos( nPos );
			}

			float moveDist = a.MoveDistSqr;

			if ( moveDist <= Util.GetSquare(a.Params.StopDist) )
			{
				a.NavAgent.ResetPath();
				a.SetMovePos( Vector3.zero );
				b.SetStatus( BhvStatus.SUCCESS );
				return b.Status;
			}

			SetAnimTrigger( a, "Run" );

			if ( b.TimerCheck >= a.Params.ChkAlert )
			{
				b.TimerCheck = 0.0f;
				CheckEnv( a );
			}

			if ( a.NavAgent.destination != a.MovePos )
				a.NavAgent.SetDestination( a.MovePos );

			return b.Status;
		}

		public Vector3 FindFleePoint( Npc a )
		{
			bool invalid = false;

			// Tighten up the search distance at target location
			float searchDist = a.Params.RoamDist / AIParams.FLEE_DIVISOR;

			Vector3 roamPoint = UnityEngine.Random.insideUnitSphere * searchDist;

			// Search for a position away from the target, or at least behind the Npc
			if ( a.TargetObj != null )
				roamPoint += (a.TargetObj.transform.position - a.transform.position).normalized * searchDist;
			else
				roamPoint += -(a.transform.forward).normalized * searchDist;

			NavMesh.SamplePosition( roamPoint, out NavMeshHit point, searchDist, NavMesh.AllAreas );

			if ( point.position.x == Mathf.Infinity || point.position.x == Mathf.NegativeInfinity )
				invalid = true;

			if ( point.position.y == Mathf.Infinity || point.position.y == Mathf.NegativeInfinity )
				invalid = true;

			if ( point.position.z == Mathf.Infinity || point.position.z == Mathf.NegativeInfinity )
				invalid = true;

			if ( !invalid )
				return point.position;
			else
				return Vector3.zero;
		}
	}
}