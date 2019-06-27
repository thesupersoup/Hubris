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
		private float _timerMove = 0.0f;
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

		public float TimerMove { get { return _timerMove; } set { _timerMove = value; } }

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
			TimerMove = 0.0f;
			TimerCheck = 0.0f;
		}

		/// <summary>
		/// Increment logic timers by time since the last frame
		/// </summary>
		public void UpdateTimers()
		{
			TimerMove += Time.deltaTime;
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
					/// BNpcAggro not implemented yet
					/* If the target is closer than our awareness distance near point or we took damage 
					if( Util.CheckDistSqr( a.transform.position, a.TargetObj.transform.position ) < a.Params.AwareClose * a.Params.AwareClose || TOOK_DAMAGE )
					{ 
						ChangeBranch( BNpcAggro.Instance );
						return;
					}*/

					/// BNpcHunt not implemented yet
					/* If the target is closer than our awareness distance midpoint
					if ( Util.CheckDistSqr( a.transform.position, a.TargetObj.transform.position ) < a.Params.AwareMed * a.Params.AwareMed )
						ChangeBranch( BNpcHunt.Instance ); // We become alert
					else */

					// If the target is further than the midpoint of our awareness distance
					// if ( Util.CheckDistSqr( a.transform.position, a.TargetObj.transform.position ) >= a.Params.AwareMed * a.Params.AwareMed )
					ChangeBranch( BNpcAlert.Instance, a ); // We become alert
				}
			}
		}
	}
}