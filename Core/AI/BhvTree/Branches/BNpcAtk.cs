using System;
using UnityEngine;
using UnityEngine.AI;

namespace Hubris
{
	public class BNpcAtk : BNpcBase
	{
		// Singleton instance of this state
		public readonly static BNpcAtk Instance = new BNpcAtk();

		public override BhvStatus Invoke( Npc a, BhvTree b )
		{
			if ( a.TargetObj == null )
			{
				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			if ( a.TargetEnt?.Stats.IsDead ?? false )
			{
				StopMove( a );
				a.ResetTargetObj();
				b.SetStatus( BhvStatus.SUCCESS );
				return b.Status;
			}

			if ( b.DistTarget > Util.GetSquare( a.Params.AtkDist ) )
			{
				if ( b.AnimInfo.IsName( AnimString.ATK ) )
					ResetAnimTrigger( a, AnimString.ATK );

				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			if ( a.NavAgent.pathStatus == NavMeshPathStatus.PathPartial || a.NavAgent.pathStatus == NavMeshPathStatus.PathInvalid )
			{
				StopMove( a );
				b.SetPathFailed( true );
				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			GetFlatVectors( a, out Vector3 targetPos, out Vector3 thisPos, out Vector3 fwd );

			float angle = Vector3.Angle( fwd, (targetPos - thisPos) );

			if ( angle > a.Params.RotAngle )
			{
				TurnToward( a, targetPos );
			}

			a.SetMovePos( targetPos );

			float nSpd = a.Params.MoveSpd;

			if ( b.AnimInfo.IsName( AnimString.ATK ) )
				nSpd *= a.Params.AtkSlow;

			// Set Speed accordingly
			if ( a.NavAgent.speed != nSpd )
				SetSpeed( a, nSpd );

			if ( b.ActionReady )
			{
				if ( b.TimerAct >= a.Params.AtkInit )
				{
					if ( a.TargetEnt != null )
					{
						if ( b.ActionReady )
						{
							a.PlaySound( SndT.ATK );
							a.TargetEnt.TakeDmg( a, (int)a.Params.DamageStats.CommonType, a.Params.DamageStats.CommonAmount, false );
						}
					}

					b.SetActionReady( false );
					SetAnimTrigger( a, AnimString.ATK );
				}
			}
			else	// ActionReady is false
			{
				if ( a.NavAgent.velocity != Vector3.zero )
				{
					if ( !b.AnimInfo.IsName( AnimString.WALK ) )
						SetAnimTrigger( a, AnimString.WALK );
				}
				else
				{
					if ( !b.AnimInfo.IsName( AnimString.IDLE ) )
						SetAnimTrigger( a, AnimString.IDLE );
				}

				if ( b.TimerAct >= a.Params.AtkEnd )
				{
					b.SetActionReady( true );
					b.TimerAct = 0.0f;
				}
			}

			if ( b.TimerCheck >= a.Params.ChkAlert )
			{ 
				b.TimerCheck = 0.0f;
				CheckEnv( a, b );
			}

			// if ( a.NavAgent.destination != a.MovePos )
			StartMove( a, a.MovePos );

			b.SetPrevPos( a.transform.position );

			return b.Status;
		}
	}
}
