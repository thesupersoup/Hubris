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

		private IBhvBranch _active;
		private float _timerMove = 0.0f;
		private float _timerChk = 0.0f;

		///--------------------------------------------------------------------
		/// BhvTree properties
		///--------------------------------------------------------------------

		public IBhvBranch ActiveBranch { get { return _active; } protected set { _active = value; } }

		public float TimerMove { get { return _timerMove; } set { _timerMove = value; } }

		public float TimerCheck { get { return _timerChk; } set { _timerChk = value; } }

		///--------------------------------------------------------------------
		/// BhvTree methods
		///--------------------------------------------------------------------

		public BhvTree(IBhvBranch nActive)
		{
			_active = nActive;
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

		public void ChangeBranch( IBhvBranch b )
		{
			ResetTimers();
			ActiveBranch = b;
			LocalConsole.Instance.Log( "New branch is " + nameof( ActiveBranch ), true );
			ActiveBranch.SetStatus( BhvStatus.RUNNING );
		}

		public void Invoke( Npc a )
		{
			if ( ActiveBranch == null )
				return;

			UpdateTimers();

			if ( a.Stats.IsDead )
			{
				if ( ActiveBranch != BNpcDead.Instance )
					ChangeBranch( BNpcDead.Instance );
			}
			else
			{
				if ( a.Stats.IsAsleep )
				{
					if ( ActiveBranch != BNpcAsleep.Instance )
						ChangeBranch( BNpcAsleep.Instance );
				}
			}

			if ( ActiveBranch != null )
				ActiveBranch.Invoke( this, a );

			// What should we do if the active behavior branch fails?
			if( ActiveBranch.Status == BhvStatus.FAILURE )
			{
				LocalConsole.Instance.Log( nameof( ActiveBranch ) + " reported failure", true);

				
				if ( a.TargetObj == null )
				{
					// If we have a destination
					if ( a.MovePos != Vector3.zero )
						ChangeBranch( BNpcMoving.Instance );
					else
						ChangeBranch( BNpcIdle.Instance );
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
					ChangeBranch( BNpcAlert.Instance ); // We become alert
				}
			}
		}
	}
}