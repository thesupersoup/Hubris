using System;
using UnityEngine;
using UnityEngine.AI;

namespace Hubris
{
	/// <summary>
	/// Npc is afraid of a target, and will try to keep its distance
	/// </summary>
	public class BNpcWary : BNpcBase
	{
		// Singleton instance of this class
		public readonly static BNpcWary Instance = new BNpcWary();

		public override BhvStatus Invoke( Npc a, BhvTree b )
		{
			if ( a.TargetObj == null )
			{
				b.SetStatus( BhvStatus.SUCCESS );
				return b.Status;
			}

			if ( b.DistTarget <= Util.GetSquare( a.Params.AwareClose ) )
			{
				if ( a.Params.Flighty )
				{
					// Too close, should flee
					b.SetStatus( BhvStatus.FAILURE );
					return b.Status;
				}
			}

			if ( b.DistTarget <= Util.GetSquare( a.Params.AwareMed ) )
			{
				if ( a.MovePos == Vector3.zero )
				{
					Vector3 nPos = FindFleePoint( a );

					if ( nPos == Vector3.zero )
					{
						b.SetStatus( BhvStatus.FAILURE );
						return b.Status;
					}

					// Set Speed accordingly
					SetSpeed( a, a.Params.MoveSpd * a.Params.MoveWalk );
					a.SetMovePos( nPos );
				}
			}

			if ( b.DistMove <= Util.GetSquare( a.Params.StopDist ) )
			{
				StopMove( a );
				b.SetStatus( BhvStatus.SUCCESS );
				return b.Status;
			}

			if( !b.AnimInfo.IsName( AnimString.WALKBACK ) )
				SetAnimTrigger( a, AnimString.WALKBACK );

			if ( b.TimerCheck >= a.Params.ChkAlert )
			{
				b.TimerCheck = 0.0f;
				CheckEnv( a, b );
			}

			if ( a.NavAgent.destination != a.MovePos )
				a.NavAgent.SetDestination( a.MovePos );

			return b.Status;
		}

		public Vector3 FindFleePoint( Npc a )
		{
			bool invalid = false;

			// Tighten up the search distance at target location
			float searchDist = a.Params.RoamDist / AIParams.WARY_DIVISOR;

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