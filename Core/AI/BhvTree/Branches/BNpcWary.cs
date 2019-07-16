using UnityEngine;

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

			// Set Speed accordingly
			SetSpeed( a, a.Params.MoveSpd * a.Params.MoveWalk );

			float waryDist = ((a.Params.AwareMax - a.Params.AwareMed) / 2.0f) + a.Params.AwareMed;

			if ( b.DistTarget <= Util.GetSquare( waryDist ) && a.MovePos == Vector3.zero )
			{
				Vector3 nPos = FindFleePoint( a );

				if ( nPos == Vector3.zero )
				{
					Debug.Log( "Unable to find flee point" );
					if ( !b.AnimInfo.IsName( AnimString.IDLE ) )
						SetAnimTrigger( a, AnimString.IDLE );
					return BhvStatus.RUNNING;
				}

				Debug.Log( "Found flee point" );
				b.TimerAct = 0.0f;
				a.SetMovePos( nPos );
			}

			if ( b.TimerCheck >= a.Params.ChkAlert )
			{
				b.TimerCheck = 0.0f;
				CheckEnv( a, b );
			}

			if ( a.NavAgent.pathPending )
				return BhvStatus.RUNNING;

			if ( !a.NavAgent.hasPath && a.MovePos != Vector3.zero )
				StartMove( a, a.MovePos );
			else
			{
				if ( a.MovePos != a.NavAgent.destination )
					a.SetMovePos( a.NavAgent.destination );
			}

			if( a.MovePos != Vector3.zero )
			{
				TurnToward( a, a.MovePos );

				if ( a.NavAgent.hasPath)
				{
					if ( !b.AnimInfo.IsName( AnimString.WALK ) )
						SetAnimTrigger( a, AnimString.WALK );
				}
				else
				{
					if ( !b.AnimInfo.IsName( AnimString.IDLE ) )
						SetAnimTrigger( a, AnimString.IDLE );
				}
			}

			if ( b.DistMove <= Util.GetSquare( a.Params.StopDist + a.NavAgent.radius ) && a.NavAgent.hasPath )
			{
				if ( a.TargetDistSqr > Util.GetSquare( a.Params.AwareMax + 10.0f ) )
				{
					StopMove( a );
					Debug.Log( "Safe distance from target, flight over" );
					a.ResetTargetObj();
					b.SetPathFailed( false );
					b.SetStatus( BhvStatus.SUCCESS );
					return b.Status;
				}
				else
				{
					StopMove( a );
					Debug.Log( "Resetting move for recalculation" );
				}
			}

			return b.Status;
		}
	}
}