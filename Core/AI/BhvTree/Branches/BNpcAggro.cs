using System;
using UnityEngine;
using UnityEngine.AI;

namespace Hubris
{
	/// <summary>
	/// Npc is aggressively moving toward a target
	/// </summary>
	public class BNpcAggro : BNpcBase
	{
		// Singleton instance of this state
		public readonly static BNpcAggro Instance = new BNpcAggro();

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

			if ( b.DistTarget <= Util.GetSquare( a.Params.AtkDist ) )
			{
				b.SetStatus( BhvStatus.SUCCESS );
				return b.Status;
			}

			if ( a.NavAgent.pathStatus == NavMeshPathStatus.PathPartial || a.NavAgent.pathStatus == NavMeshPathStatus.PathInvalid )
			{
				bool failure = false;

				if ( a.NavAgent.pathStatus == NavMeshPathStatus.PathInvalid )
					failure = true;

				if ( a.NavAgent.pathStatus == NavMeshPathStatus.PathPartial )
				{
					if ( Util.CheckDistSqr( a.NavAgent.destination, a.TargetPos ) > Util.GetSquare( a.Params.AtkDist ) )
						failure = true;
				}

				if ( failure )
				{
					StopMove( a );
					b.SetPathFailed( true );
					b.SetStatus( BhvStatus.FAILURE );
					return b.Status;
				}
			}

			a.SetMovePos( a.TargetPos );

			float nSpd = a.Params.MoveSpd;

			if ( b.AnimInfo.IsName( AnimString.ATK ) )
				nSpd *= a.Params.AtkSlow;
			else
			{
				if( !b.AnimInfo.IsName( AnimString.RUN ) )
					SetAnimTrigger( a, AnimString.RUN );
			}

			// Set Speed accordingly
			if ( a.NavAgent.speed != nSpd )
				SetSpeed( a, nSpd );

			if ( b.TimerCheck >= a.Params.ChkAlert )
			{
				if ( a.TargetPos != a.MovePos )
					a.SetMovePos( a.TargetPos );

				b.TimerCheck = 0.0f;
				CheckEnv( a, b );
			}

			if ( a.NavAgent.destination != a.MovePos )
				StartMove( a, a.MovePos );

			return b.Status;
		}
	}
}
