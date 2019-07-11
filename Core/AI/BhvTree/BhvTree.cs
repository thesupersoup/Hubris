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
		private bool _actReady = false;
		private float _distTarget = 0.0f;
		private float _distMove = 0.0f;
		private Vector3 _prevPos = Vector3.zero;
		private AnimatorStateInfo _animInfo;
		private bool _seeTarget = false;

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

		/// <summary>
		/// Timer for certain actions in behaviors
		/// </summary>
		public float TimerAct { get { return _timerAct; } set { _timerAct = value; } }

		/// <summary>
		/// Timer for checking the environment
		/// </summary>
		public float TimerCheck { get { return _timerChk; } set { _timerChk = value; } }

		/// <summary>
		/// Flag for certain Npc behaviors
		/// </summary>
		public bool ActionReady => _actReady;

		/// <summary>
		/// Square of the distance between the Npc and the target
		/// </summary>
		public float DistTarget => _distTarget;

		/// <summary>
		/// Square of the distance between the Npc and desired movement position
		/// </summary>
		public float DistMove => _distMove;

		/// <summary>
		/// Position of the Npc as of the previous behavior invocation
		/// </summary>
		public Vector3 PrevPos => _prevPos;

		/// <summary>
		/// Current Animator state info for the Npc
		/// </summary>
		public AnimatorStateInfo AnimInfo => _animInfo;

		/// <summary>
		/// Can the Npc see the target, if there is any?
		/// </summary>
		public bool SeeTarget { get { return _seeTarget; } protected set { _seeTarget = value; } }

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
		private void UpdateTimers()
		{
			TimerAct += Time.deltaTime;
			TimerCheck += Time.deltaTime;
		}

				/// <summary>
		/// Sets the ActionReady flag
		/// </summary>
		public void SetActionReady( bool r )
		{
			_actReady = r;
		}

		/// <summary>
		/// Update distance to target object and distance to move point for use by behavior branches
		/// </summary>
		private void UpdateDistances( Npc a )
		{
			if( a.TargetObj != null )
				_distTarget = a.TargetDistSqr;

			if ( a.MovePos != Vector3.zero )
				_distMove = a.MoveDistSqr;
		}

		/// <summary>
		/// Set the previous position
		/// </summary>
		public void SetPrevPos( Vector3 pos )
		{
			_prevPos = pos;
		}

		/// <summary>
		/// Update the stored animator state info
		/// </summary>
		private void UpdateAnimInfo( Npc a, int layer )
		{
			_animInfo = a.Anim.GetCurrentAnimatorStateInfo( layer );
		}

		/// <summary>
		/// Update whether the Npc can see the target, if there is any
		/// </summary>
		private void UpdateSeeTarget( Npc a )
		{
			SeeTarget = a.SightCheck();
		}

		/// <summary>
		/// Change the active behavior branch
		/// </summary>
		public void ChangeBranch( IBhvNode n, Npc a )
		{
			ResetTimers();
			SetActionReady( true );

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
			UpdateDistances( a );
			UpdateAnimInfo( a, 0 );
			UpdateSeeTarget( a );
			RootChecks( a );

			// What should we do if the active behavior branch succeeds or fails
			if( ActiveBranch.Invoke( this, a ) != BhvStatus.RUNNING )
			{				
				if ( a.TargetObj == null || !SeeTarget )
				{
					// If we have a destination
					if ( a.MovePos != Vector3.zero )
						ChangeBranch( BNpcMoving.Instance, a );
					else
						ChangeBranch( BNpcIdle.Instance, a );
				}
				else // If the parent NPC has a target (that can be seen); we're not in idletown anymore
				{
					/* if ( TOOK_DAMAGE )
					 *	ChangeBranch( BNpcAggro, a ); // We go straight to angry (but there needs to be certain caveats)
					 * else
					 */

					// Check how close the Npc is
					if ( DistTarget > Util.GetSquare( a.Params.AwareMed ) )	// If target is further than the medium awareness distance
						ChangeBranch( BNpcAlert.Instance, a );	// We become alert
					else if ( DistTarget > Util.GetSquare( a.Params.AwareClose ) )	// If target is nearer than the medium distance but further than the close distance
					{
						if ( a.Params.Predator )	// If we're a predatory Npc...
							ChangeBranch( BNpcHunt.Instance, a );	// ... we go hunting
						else	// Otherwise...
							ChangeBranch( BNpcWary.Instance, a );	// ... we become wary
					}
					else if ( DistTarget > Util.GetSquare( a.Params.AtkDist ) )// If target is too close for comfort
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