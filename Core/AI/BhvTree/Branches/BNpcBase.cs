using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Hubris
{
	/// <summary>
	/// Base class for behavior branches, includes common methods
	/// </summary>
	public abstract class BNpcBase : IBhvNode
	{
		public virtual BhvStatus Invoke( Npc a, BhvTree b )
		{
			return b.Status;
			// Override in derived state
		}

		public void SetSpeed( Npc a, float nSpd )
		{
			if(a.NavAgent != null)
			{
				if (a.NavAgent.speed != nSpd)
					a.NavAgent.speed = nSpd;
			}
		}

		/// <summary>
		/// Initiates a move to the indicated position
		/// </summary>
		public void StartMove( Npc a, Vector3 pos )
		{
			if ( a.NavAgent == null )
				return;

			// Something is wrong with the NavAgent's NavMesh binding
			if ( !a.NavAgent.isOnNavMesh && !a.NavAgent.isOnOffMeshLink )
				return;

			// Safeguard against spamming new destinations while a path is pending
			if ( a.NavAgent.pathPending )
				return;

			// Debug.Log( $"Starting new move to {pos}" );
			a.NavAgent.SetDestination( pos );
		}


		/// <summary>
		/// Resets NavMeshAgent path and sets MovePos to Vector3.zero
		/// </summary>
		/// <param name="a"></param>
		public void StopMove( Npc a )
		{
			a.SetMovePos( Vector3.zero );

			if ( a.NavAgent == null )
				return;

			// Something is wrong with the NavAgent's NavMesh binding
			if ( !a.NavAgent.isOnNavMesh && !a.NavAgent.isOnOffMeshLink )
				return;

			a.NavAgent.ResetPath();
		}

		public void SetAnimTrigger( Npc a, AnimT e )
		{
			if( a.Anim == null )
			{
				LocalConsole.Instance.LogWarning( "Behavior SetAnimTrigger(): Npc Animator is null", true );
				return;
			}

			switch ( e )
			{
				case AnimT.IDLE:
					a.Anim.SetTrigger( AnimString.IDLE );
					break;
				case AnimT.WALK:
					a.Anim.SetTrigger( AnimString.WALK );
					break;
				case AnimT.WALKBACK:
					a.Anim.SetTrigger( AnimString.WALKBACK );
					break;
				case AnimT.RUN:
					a.Anim.SetTrigger( AnimString.RUN );
					break;
				case AnimT.ATK:
					a.Anim.SetTrigger( AnimString.ATK );
					break;
				case AnimT.HURT:
					a.Anim.SetTrigger( AnimString.HURT );
					break;
				case AnimT.DIE:
					a.Anim.SetTrigger( AnimString.DIE );
					break;
				case AnimT.SLEEP:
					a.Anim.SetTrigger( AnimString.SLEEP );
					break;
				default:
					LocalConsole.Instance.LogWarning( "Behavior SetAnimTrigger(): Invalid animation trigger enum value specified", true );
					break;
			}
		}

		public void SetAnimTrigger( Npc a, string nAnim )
		{
			if ( a.Anim == null )
			{
				LocalConsole.Instance.LogWarning( "Behavior SetAnimTrigger(): Npc Animator is null", true );
				return;
			}

			if (nAnim != null && nAnim.Length > 0)
					a.Anim.SetTrigger(nAnim);
		}

		public void SetAnimBool( Npc a, string nName, bool nValue )
		{
			if ( a.Anim == null )
			{
				LocalConsole.Instance.LogWarning( "Behavior SetAnimTrigger(): Npc Animator is null", true );
				return;
			}

			if (nName != null && nName.Length > 0)
					a.Anim.SetBool(nName, nValue);
		}

		public void ResetAnimTrigger( Npc a, AnimT e )
		{
			if ( a.Anim == null )
			{
				LocalConsole.Instance.LogWarning( "Behavior ResetAnimTrigger(): Npc Animator is null", true );
				return;
			}

			switch ( e )
			{
				case AnimT.IDLE:
					a.Anim.ResetTrigger( AnimString.IDLE );
					break;
				case AnimT.WALK:
					a.Anim.ResetTrigger( AnimString.WALK );
					break;
				case AnimT.WALKBACK:
					a.Anim.ResetTrigger( AnimString.WALKBACK );
					break;
				case AnimT.RUN:
					a.Anim.ResetTrigger( AnimString.RUN );
					break;
				case AnimT.ATK:
					a.Anim.ResetTrigger( AnimString.ATK );
					break;
				case AnimT.HURT:
					a.Anim.ResetTrigger( AnimString.HURT );
					break;
				case AnimT.DIE:
					a.Anim.ResetTrigger( AnimString.DIE );
					break;
				case AnimT.SLEEP:
					a.Anim.ResetTrigger( AnimString.SLEEP );
					break;
				default:
					LocalConsole.Instance.LogWarning( "Behavior ResetAnimTrigger(): Invalid animation trigger enum value specified", true );
					break;
			}
		}

		public void ResetAnimTrigger( Npc a, string nAnim )
		{
			if ( a.Anim == null )
			{
				LocalConsole.Instance.LogWarning( "Behavior SetAnimTrigger(): Npc Animator is null", true );
				return;
			}

			if ( nAnim != null && nAnim.Length > 0 )
				a.Anim.ResetTrigger( nAnim );
		}

		/// <summary>
		/// Turn toward the specified Vector3 target
		/// </summary>
		public void TurnToward( Npc a, Vector3 target )
		{
			if ( float.IsNaN( target.sqrMagnitude ) )
				return;

			if ( target == Vector3.zero )
				return;

			if ( target == Vector3.negativeInfinity || target == Vector3.positiveInfinity )
				return;

			a.transform.rotation = Quaternion.Slerp( a.transform.rotation,
					Quaternion.LookRotation( target - a.transform.position ), a.Params.RotSpd );
		}

		/// <summary>
		/// Tries to find a random point up to RoamDist away from the Npc
		/// </summary>
		public Vector3 FindRoamPoint( Npc a )
		{
			bool invalid = false;

			Vector3 roamPoint = UnityEngine.Random.insideUnitSphere * a.Params.RoamDist;

			roamPoint += a.transform.position;

			NavMesh.SamplePosition( roamPoint, out NavMeshHit point, a.Params.RoamDist, NavMesh.AllAreas );

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

		/// <summary>
		/// Tries to find a point in the opposite direction of the target object
		/// </summary>
		public Vector3 FindFleePoint( Npc a )
		{
			bool invalid = false;

			Vector3 fleePoint = a.transform.position + (a.transform.position - a.TargetPos) + (UnityEngine.Random.insideUnitSphere * (a.Params.RoamDist / AIParams.FLEE_DIVISOR));

			// fleePoint += a.transform.position;

			// Debug.Log( $"Flee Point: {fleePoint}" );

			NavMesh.SamplePosition( fleePoint, out NavMeshHit point, a.Params.RoamDist / AIParams.FLEE_DIVISOR, NavMesh.AllAreas );

			if ( point.position.x == Mathf.Infinity || point.position.x == Mathf.NegativeInfinity )
				invalid = true;

			if ( point.position.y == Mathf.Infinity || point.position.y == Mathf.NegativeInfinity )
				invalid = true;

			if ( point.position.z == Mathf.Infinity || point.position.z == Mathf.NegativeInfinity )
				invalid = true;

			// Debug.Log( $"Invalid is {invalid}" );

			if ( !invalid )
				return point.position;
			else
				return Vector3.zero;
		}

		/// <summary>
		/// Provides the TargetObj position, Npc position, and Npc forward vectors via the out parameters with the y-value zeroed out
		/// </summary>
		public void GetFlatVectors( Npc a, out Vector3 targetPos, out Vector3 thisPos, out Vector3 fwd )
		{
			if ( a.TargetObj != null )
			{
				targetPos = a.TargetPos;
				targetPos.y = 0.0f;
			}
			else
				targetPos = Vector3.zero;

			thisPos = a.transform.position;
			fwd = a.transform.forward;
			thisPos.y = 0.0f;
			fwd.y = 0.0f;
		}

		/// <summary>
		/// Provides the Npc position and forward vectors via the out parameters with the y-value zeroed out
		/// </summary>
		public void GetFlatVectors( Npc a, out Vector3 thisPos, out Vector3 fwd )
		{
			thisPos = a.transform.position;
			fwd = a.transform.forward;
			thisPos.y = 0.0f;
			fwd.y = 0.0f;
		}

		/// <summary>
		/// Scan the environment for entities, then process tracked entities
		/// </summary>
		public void CheckEnv( Npc a, BhvTree b )
		{
			if ( a.TrackDict != null )
				ProcessTrackDict( a, b );
		}

		/// <summary>
		/// Process the entities in the TrackDict, finds closest and removes ents that are beyond MAX_DIST or dead
		/// </summary>
		public void ProcessTrackDict( Npc a, BhvTree b )
		{
			if ( a.TrackDict != null && a.TrackDict.Count > 0 )
			{
				float closestEntDistSqr = 0.0f;
				LiveEntity closestEnt = null;
				bool fresh = true;

				foreach ( ulong id in a.TrackDict.Keys )
				{
					LiveEntity ent;
					a.TrackDict.TryGetValue( id, out ent );

					// If the entity dies and is cleaned up before the trackBook is processed, it will be null
					if ( ent == null ) 
					{
						a.RemoveList.Add( id );
						continue;
					}

					// Check if the entity is invisible
					if ( ent.Stats.Invisible )
					{
						continue;
					}

					// Check if the entity is dead
					if ( ent.Stats.IsDead )
					{
						// If the entity is dead and the same type as this one, set behavior to aggro or hunt or something
						continue;
					}

					if ( ent is Npc npc )
					{
						// Check if we want to make a squad with other entities of the same type
						//
						//

						// If we're prey
						if ( !a.Params.Predator )
						{
							// Prey should ignore other prey
							if ( !npc.Params.Predator )
								continue;
						}
					}

					float maxDist = a.Params.AwareMax;
					float entDist = Util.CheckDistSqr( a.transform.position, ent.transform.position );

					// Check if it's beyond our max awareness distance
					if ( entDist > Util.GetSquare( maxDist ) )
					{
						a.RemoveList.Add( id );
						continue;
					}

					// Field-of-view and Line-of-sight check
					if ( !a.SightCheck( ent.gameObject, Util.GetSquare( maxDist ) ) )
						continue;
					
					// If we haven't set our closest entity info yet
					if ( fresh )
					{
						closestEntDistSqr = entDist;
						closestEnt = ent;
						fresh = false;
					}
					else
					{
						if ( entDist < closestEntDistSqr )
						{
							closestEntDistSqr = maxDist;
							closestEnt = ent;
						}
					}
				}

				if ( closestEnt != null )
				{
					// If we don't currently have a target
					if ( a.TargetObj == null )
						a.SetTargetObj( closestEnt.gameObject, closestEnt );
					else
					{
						if ( a.TargetObj != closestEnt.gameObject && closestEntDistSqr < ( a.TargetDistSqr * a.Params.ProxPct ) )
							a.SetTargetObj( closestEnt.gameObject, closestEnt );
					}
				}
			}
			else
			{
				// There's no entities within range, so reset our target
				if( a.TargetObj != null )
					a.ResetTargetObj();
			}

			if ( a.RemoveList != null && a.RemoveList.Count > 0 )
			{
				for ( int i = 0; i < a.RemoveList.Count; i++ )
				{
					LiveEntity ent = null;
					a.TrackDict.TryGetValue( a.RemoveList[i], out ent );
					Untrack( a, a.RemoveList[i]);
				}

				a.RemoveList.Clear();
			}
		}

		/// <summary>
		/// Attempts to remove the specified entity from the TrackList
		/// </summary>
		public void Untrack( Npc a, ulong id )
		{
			a.TrackDict.Remove( id );
		}
	}
}
