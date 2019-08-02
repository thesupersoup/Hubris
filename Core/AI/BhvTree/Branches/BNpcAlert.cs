using System;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Npc is aware of a target but hasn't taken action yet
	/// </summary>
	public class BNpcAlert : BNpcBase
	{
		// Singleton instance of this state
		public readonly static BNpcAlert Instance = new BNpcAlert();

		public override BhvStatus Invoke( Npc a, BhvTree b )
		{
			if ( a.TargetObj == null )
			{
				b.SetStatus( BhvStatus.FAILURE );
				b.SetPatient( true );
				return b.Status;
			}

			if ( a.Stats.Wounded )
			{
				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			if ( !b.SeeTarget && b.DistTarget > Util.GetSquare( a.Params.AwareMed ) )
			{
				// Debug.Log( "SightCheck failed" );
				a.ResetTargetObj();
				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			if ( b.DistTarget <= Util.GetSquare( a.Params.AwareMed ) )
			{
				if( a.Params.Predator )
				{
					b.SetStatus( BhvStatus.SUCCESS );
					return b.Status;
				}
				else
				{
					b.SetStatus( BhvStatus.FAILURE );
					return b.Status;
				}
			}

			if( b.TimerAct >= a.Params.PatienceTime )
			{
				b.TimerAct = 0.0f;

				if ( UnityEngine.Random.Range( 1, 20 ) >= 10 )
				{
					b.SetPatient( false );
					a.PlaySound( SndT.ALERT );
					b.SetStatus( a.Params.Predator ? BhvStatus.SUCCESS : BhvStatus.FAILURE );
					return b.Status;
				}
			}

			if (b.TimerCheck >= a.Params.ChkAlert)
			{
				b.TimerCheck = 0.0f;
				CheckEnv( a, b );
			}

			if (a.NavAgent.hasPath)
				a.NavAgent.ResetPath();

			GetFlatVectors( a, out Vector3 targetPos, out Vector3 thisPos, out Vector3 fwd );

			float angle = Vector3.Angle(fwd, (targetPos - thisPos));

			if (angle > a.Params.RotAngle)
			{
				// Reverse walk anim to differentiate from regular walking
				if ( !b.AnimInfo.IsName( AnimString.WALKBACK ) )
					SetAnimTrigger( a, AnimString.WALKBACK );

				TurnToward( a, targetPos );
			}
			else
			{
				if ( !b.AnimInfo.IsName( AnimString.IDLE ) )
					SetAnimTrigger( a, AnimString.IDLE );
			}

			return b.Status;
		}
	}
}
