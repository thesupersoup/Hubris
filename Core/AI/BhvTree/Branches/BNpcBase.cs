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
		/// Resets NavMeshAgent path and sets MovePos to Vector3.zero
		/// </summary>
		/// <param name="a"></param>
		public void StopMove( Npc a )
		{
			if ( a.NavAgent != null )
				a.NavAgent.ResetPath();

			a.SetMovePos( Vector3.zero );
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

			Debug.Log( $"Flee Point: {fleePoint}" );

			NavMesh.SamplePosition( fleePoint, out NavMeshHit point, a.Params.RoamDist / AIParams.FLEE_DIVISOR, NavMesh.AllAreas );

			if ( point.position.x == Mathf.Infinity || point.position.x == Mathf.NegativeInfinity )
				invalid = true;

			if ( point.position.y == Mathf.Infinity || point.position.y == Mathf.NegativeInfinity )
				invalid = true;

			if ( point.position.z == Mathf.Infinity || point.position.z == Mathf.NegativeInfinity )
				invalid = true;

			Debug.Log( $"Invalid is {invalid}" );

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
			AreaScan( a );
			if ( a.TrackList != null )
				ProcessTrackList( a, b );
		}

		/// <summary>
		/// Uses a Physics.OverlapSphere to scan the area up to this entity's Awareness Max Distance and log any GameObjects on Layers specified in the EntMask
		/// </summary>
		public void AreaScan( Npc a ) 
		{
			Collider[] localEnts = Physics.OverlapSphere(a.transform.position, a.Params.AwareMax, a.EntMask);

			if (localEnts != null && localEnts.Length > 0)
			{
				foreach (Collider col in localEnts)
				{
					GameObject obj = col.transform.root.gameObject;
					LiveEntity ent = obj.GetComponent<LiveEntity>();

					if ( ent == null )
					{
						Debug.LogWarning( a.gameObject.name + " couldn't find a LiveEntity script for a detected object (" + obj.name + "), can't add to trackList" );
						continue;
					}

					// Check if we found ourselves
					if ( ent == a )
						continue;

					// Ignore if the entity is dead or invisible
					if ( ent.Stats.IsDead || ent.Stats.Invisible )
						continue;

					// Check if we're already tracking this entity
					if ( a.TrackList.Contains( ent ) )
						continue;
							
					Debug.Log( $"{a.gameObject.name} found {ent.gameObject.name}" );
					a.TrackList.Add(ent);
				}
			}
		}

		/// <summary>
		/// Process the entities in the TrackList, finds closest and removes ents that are beyond MAX_DIST or dead
		/// </summary>
		public void ProcessTrackList( Npc a, BhvTree b )
		{
			bool resetTrackList = false;

			if (a.TrackList != null && a.TrackList.Count > 0)
			{
				float closestEntDistSqr = 0.0f;
				LiveEntity closestEnt = null;
				bool fresh = true;

				for (int i = 0; i < a.TrackList.Count; i++)
				{
					LiveEntity ent = a.TrackList[i];

					// If the entity dies and is cleaned up before the trackBook is processed, it will be null
					if ( ent == null ) 
					{
						resetTrackList = true;
						continue;
					}

					// Check if the entity is dead or invisible
					if ( ent.Stats.IsDead || ent.Stats.Invisible )
					{
						// If the entity is dead and the same type as this one, set behavior to aggro or hunt or something

						a.RemoveList.Add( ent );
						continue;
					}

					// Check if we want to make a squad with other entities of the same type
					//
					//

					float distSqr = Util.CheckDistSqr( a.transform.position, ent.transform.position );

					// Check if it's beyond our max awareness distance
					if ( distSqr > Util.GetSquare( a.Params.AwareMax ) )
					{
						a.RemoveList.Add( ent );
						continue;
					}

					// Field-of-view and Line-of-sight check
					if ( !a.SightCheck( ent.gameObject, distSqr ) )
						continue;
					
					// If we haven't set our closest entity info yet
					if ( fresh )
					{
						closestEntDistSqr = distSqr;
						closestEnt = ent;
						fresh = false;
					}
					else
					{
						if ( distSqr < closestEntDistSqr )
						{
							closestEntDistSqr = distSqr;
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
						if ( a.TargetObj != closestEnt.gameObject )
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
					LiveEntity ent = a.RemoveList[i];
					if (ent != null) // If the entity dies and is cleaned up before the removeBook is processed, it will be null
						Untrack(a, ent, ref resetTrackList);
				}

				a.RemoveList.Clear();
			}

			if ( resetTrackList )
				a.TrackList.Clear();
		}

		/// <summary>
		/// Attempts to remove the specified entity from the TrackList, checks for null entity and sets TrackList reset flag if one is found
		/// </summary>
		public void Untrack( Npc a, LiveEntity ent, ref bool resetTrackList )
		{
			// If the entity dies and is cleaned up before it can be untracked, it will be null
			if ( ent == null )
			{
				resetTrackList = true;
				return;
			}

			a.TrackList.Remove(ent);
		}
	}
}
