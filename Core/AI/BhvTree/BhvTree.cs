using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// The root object of an AI behavior tree; contains a reference to the active branch, which can be changed by the children
	/// </summary>
	public class BhvTree : IBhvTree
	{
		///--------------------------------------------------------------------
		/// BhvTree instance vars
		///--------------------------------------------------------------------

		private IBhvNode _active;
		private BhvStatus _activeStatus;
		private float _timerAct = 0.0f;
		private float _timerChk = 0.0f;

		///--------------------------------------------------------------------
		/// BhvTree properties
		///--------------------------------------------------------------------

		/// <summary>
		/// Currrently active behavior branch
		/// </summary>
		public IBhvNode ActiveBranch { get { return _active; } protected set { _active = value; } }

		/// <summary>
		/// ActiveBranch current status
		/// </summary>
		public BhvStatus Status { get { return _activeStatus; } protected set { _activeStatus = value; } }

		public float TimerAct { get { return _timerAct; } set { _timerAct = value; } }

		public float TimerCheck { get { return _timerChk; } set { _timerChk = value; } }

		///--------------------------------------------------------------------
		/// BhvTree methods
		///--------------------------------------------------------------------

		public BhvTree( IBhvNode n )
		{
			_active = n;
		}

		/// <summary>
		/// Set the status of the active behavior branch
		/// </summary>
		public void SetStatus( BhvStatus s )
		{
			Status = s;
		}

		/// <summary>
		/// Reset logic timers
		/// </summary>
		public void ResetTimers()
		{
			TimerAct = 0.0f;
			TimerCheck = 0.0f;
		}

		/// <summary>
		/// Increment logic timers by time since the last frame
		/// </summary>
		public void UpdateTimers()
		{
			TimerAct += Time.deltaTime;
			TimerCheck += Time.deltaTime;
		}

		public void ChangeBranch( IBhvNode n, Npc a )
		{
			ResetTimers();

			if ( HubrisCore.Instance.Debug )
				LocalConsole.Instance.Log( $"{a.Name}: {nameof( ActiveBranch )} requested change due to status: {(int)Status}", true );

			ActiveBranch = n;
			SetStatus( BhvStatus.RUNNING );
		}

		/// <summary>
		/// Universal behavior tree checks for all behavior branches
		/// </summary>
		internal void RootChecks( Npc a )
		{
			if ( a.Stats.IsDead )
			{
				if ( ActiveBranch != BNpcDead.Instance )
				{
					// Whatever you were doing, something failed if you died ...
					SetStatus( BhvStatus.FAILURE );
					ChangeBranch( BNpcDead.Instance, a );
				}
			}
			else
			{
				if ( a.Stats.IsAsleep )
				{
					if ( ActiveBranch != BNpcAsleep.Instance )
					{
						// ... same for falling asleep
						SetStatus( BhvStatus.FAILURE );
						ChangeBranch( BNpcAsleep.Instance, a );
					}
				}
			}
		}

		/// <summary>
		/// Returns a behavior branch, accepts a BhvBranch enum value
		/// </summary>
		/// <returns></returns>
		public IBhvNode GetBranch( BhvEnum v )
		{
			IBhvNode node;

			switch( v )
			{
				case BhvEnum.IDLE:
					node = BNpcIdle.Instance;
					break;
				case BhvEnum.MOVING:
					node = BNpcMoving.Instance;
					break;
				case BhvEnum.ALERT:
					node = BNpcAlert.Instance;
					break;
				case BhvEnum.WARY:
					node = BNpcWary.Instance;
					break;
				case BhvEnum.HUNT:
					node = BNpcHunt.Instance;
					break;
				case BhvEnum.AGGRO:
					node = BNpcAggro.Instance;
					break;
				case BhvEnum.FLEE:
					node = BNpcFlee.Instance;
					break;
				case BhvEnum.ASLEEP:
					node = BNpcAsleep.Instance;
					break;
				case BhvEnum.DEAD:
					node = BNpcDead.Instance;
					break;
				default:
					node = BNpcNone.Instance;
					break;
			}

			return node;
		}

		public void Invoke( Npc a )
		{
			if ( ActiveBranch == null )
				return;

			UpdateTimers();
			RootChecks( a );

			// What should we do if the active behavior branch succeeds or fails
			if( ActiveBranch.Invoke( this, a ) != BhvStatus.RUNNING )
			{				
				if ( a.TargetObj == null )
				{
					// If we have a destination
					if ( a.MovePos != Vector3.zero )
						ChangeBranch( BNpcMoving.Instance, a );
					else
						ChangeBranch( BNpcIdle.Instance, a );
				}
				else // If the parent NPC has a target; we're not in idletown anymore
				{
					/* if ( TOOK_DAMAGE )
					 *	ChangeBranch( BNpcAggro, a ); // We go straight to angry (but there needs to be certain caveats)
					 * else
					 */

					// Check how close the Npc is
					float distSqr = Util.CheckDistSqr( a.transform.position, a.TargetObj.transform.position );

					if ( distSqr > a.Params.AwareMed * a.Params.AwareMed )	// If target is further than the medium awareness distance
						ChangeBranch( BNpcAlert.Instance, a );	// We become alert
					else if ( distSqr > a.Params.AwareClose * a.Params.AwareClose )	// If target is nearer than the medium distance but further than the close distance
					{
						if ( a.Params.Predator )	// If we're a predatory Npc...
							ChangeBranch( BNpcHunt.Instance, a );	// ... we go hunting
						else	// Otherwise...
							ChangeBranch( BNpcWary.Instance, a );	// ... we become wary
					}
					else if ( distSqr > a.Params.AtkDist * a.Params.AtkDist )// If target is too close for comfort
					{
						if ( a.Params.Predator )	// If we're a predatory Npc...
							ChangeBranch( BNpcAggro.Instance, a );	// ... we get angry
						else	// Otherwise...
							ChangeBranch( BNpcFlee.Instance, a );	// ... we run away
					}
					else	// And then the fight started
						ChangeBranch( BNpcAtk.Instance, a );
				}
			}
		}
	}
}