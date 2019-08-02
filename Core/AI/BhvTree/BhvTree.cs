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
		private float _timerRoot = 0.0f;
		private float _timerAct = 0.0f;
		private float _timerChk = 0.0f;
		private bool _actReady = false;
		private float _distTarget = 0.0f;
		private float _distMove = 0.0f;
		private Vector3 _prevPos = Vector3.zero;
		private bool _pathFailed = false;
		private AnimatorStateInfo _animInfo;
		private bool _seeTarget = false;
		private bool _ignoreSightCheck = false;
		private bool _patient = true;

		// temporary for testing
		private UnityEngine.AI.NavMeshPathStatus _pathStatus = UnityEngine.AI.NavMeshPathStatus.PathInvalid;

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
		/// Timer for certain root behaviors
		/// </summary>
		public float TimerRoot { get { return _timerRoot; } protected set { _timerRoot = value; } }

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
		/// Set true when Npc can't reach its target
		/// </summary>
		public bool PathFailed => _pathFailed;

		/// <summary>
		/// Current Animator state info for the Npc
		/// </summary>
		public AnimatorStateInfo AnimInfo => _animInfo;

		/// <summary>
		/// Can the Npc see the target, if there is any? Will also return true if IgnoreSightCheck is true.
		/// </summary>
		public bool SeeTarget { get { return _seeTarget || _ignoreSightCheck; } protected set { _seeTarget = value; } }

		/// <summary>
		/// Are we temporarily ignoring the sight check?
		/// </summary>
		public bool IgnoreSightCheck { get { return _ignoreSightCheck; } protected set { _ignoreSightCheck = value; } }

		/// <summary>
		/// Will we wait to act? False forces action
		/// </summary>
		public bool Patient { get { return _patient; } protected set { _patient = value; } }

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
			TimerRoot += Time.deltaTime;
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
		/// Set whether the Npc's pathfinding failed
		/// </summary>
		public void SetPathFailed( bool fail )
		{
			_pathFailed = fail;
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
			SeeTarget = a.SightCheck( a.TargetObj, a.TargetDistSqr );

			// If we can see the target, reset ignore sight check
			if ( SeeTarget )
				_ignoreSightCheck = false;
		}

		/// <summary>
		/// Public method to set ignore sight check
		/// </summary>
		public void SetIgnoreSightCheck( bool check )
		{
			_ignoreSightCheck = check;
		}

		/// <summary>
		/// Public method to set patience
		/// </summary>
		public void SetPatient( bool p )
		{
			_patient = p;
		}

		/// <summary>
		/// Resets NavMeshAgent flags that may have been changed by behaviors
		/// </summary>
		public void ResetNavAgent( Npc a )
		{
			if ( !a.NavAgent.updateRotation )
				a.NavAgent.updateRotation = true;
		}

		/// <summary>
		/// Change the active behavior branch
		/// </summary>
		public void ChangeBranch( IBhvNode n, Npc a )
		{
			ResetTimers();
			ResetNavAgent( a );
			SetActionReady( true );

			// if ( HubrisCore.Instance.Debug )
				// LocalConsole.Instance.Log( $"{a.Name}: {nameof( ActiveBranch )} {ActiveBranch.GetType().Name} requested change due to status: {(int)Status}", true );

			ActiveBranch = n;
			a.BhvBranch = n.GetType().Name;

			// if ( HubrisCore.Instance.Debug )
				// LocalConsole.Instance.Log( $"{a.Name}: {nameof( ActiveBranch )} is now {ActiveBranch.GetType().Name}", true );

			SetStatus( BhvStatus.RUNNING );
		}

		/// <summary>
		/// Universal behavior tree checks for all behavior branches
		/// </summary>
		internal bool RootChecks( Npc a )
		{
			if ( a.Stats.IsDead )
			{
				if ( ActiveBranch != BNpcDead.Instance )
				{
					a.PlaySound( SndT.DIE );
					// Whatever you were doing, something failed if you died ...
					SetStatus( BhvStatus.FAILURE );
					ChangeBranch( BNpcDead.Instance, a );
					return false;
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
						return false;
					}
				}

				// Reset patient if no target
				if ( a.TargetObj == null )
					SetPatient( true );
			}

			return true;
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

			// Root methods which should be invoked each time
			UpdateTimers();
			UpdateDistances( a );
			UpdateAnimInfo( a, 0 );

			if( !RootChecks( a ) )
				return;

			// Root methods which should be invoked on a timer
			if ( TimerRoot >= a.Params.ChkAlert )
			{
				UpdateSeeTarget( a );
				TimerRoot = 0.0f;
			}

			// What should we do if the active behavior branch succeeds or fails
			if ( ActiveBranch.Invoke( a, this ) != BhvStatus.RUNNING )
			{
				// If we were fleeing but we successfully escaped
				if ( ActiveBranch == BNpcFlee.Instance && Status == BhvStatus.SUCCESS )
				{
					// Replenish stats
					a.Stats.AddHealth( a.Stats.HealthMax - a.Stats.Health );
					a.Stats.AddArmor( a.Stats.ArmorMax - a.Stats.Armor );
					a.Stats.AddStamina( a.Stats.StaminaMax - a.Stats.Stamina );
				}

				// Unable to reach target, pathfinding failed
				if ( PathFailed )
				{
					// Debug.Log( "Pathfinding failed, fleeing..." );
					ChangeBranch( BNpcFlee.Instance, a );
					return;
				}

				if ( a.Stats.Wounded )
				{
					if ( a.TargetObj == null || !SeeTarget )
					{
						ChangeBranch( BNpcFlee.Instance, a );
						return;
					}
					else
					{
						if ( DistTarget > Util.GetSquare( a.Params.AtkDist ) )
						{
							if ( !a.Params.Territorial && (!a.Params.Predator || a.Params.Flighty) )
								ChangeBranch( BNpcFlee.Instance, a );
							else
								ChangeBranch( BNpcAggro.Instance, a );
						}
						else
						{
							// Only attack live entities
							if ( a.TargetEnt != null )
								ChangeBranch( BNpcAtk.Instance, a );
						}

						return;
					}
				}

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
					// Check how close the Npc is
					if ( DistTarget > Util.GetSquare( a.Params.AwareMed ) && Patient ) // If target is further than the medium awareness distance and Npc is still patient
						ChangeBranch( BNpcAlert.Instance, a );  // We become alert
					else if ( DistTarget > Util.GetSquare( a.Params.AwareClose ) )  // If target is nearer than the medium distance but further than the close distance
					{
						if ( a.Params.Predator || a.Params.Territorial )	// If we're a predatory Npc or territorial...
							ChangeBranch( BNpcHunt.Instance, a );	// ... we go hunting
						else	// Otherwise...
							ChangeBranch( BNpcWary.Instance, a );	// ... we become wary
					}
					else if ( DistTarget > Util.GetSquare( a.Params.AtkDist ) )// If target is too close for comfort
					{
						if ( a.Params.Predator || a.Params.Territorial )	// If we're a predatory Npc...
							ChangeBranch( BNpcAggro.Instance, a );	// ... we get angry
						else	// Otherwise...
							ChangeBranch( BNpcFlee.Instance, a );	// ... we run away
					}
					else	// And then the fight started
					{
						// Only attack live entities
						if( a.TargetEnt != null )
							ChangeBranch( BNpcAtk.Instance, a );
					}
				}
			}
		}
	}
}