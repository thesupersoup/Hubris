using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Npc is idle, and may choose to roam on a given interval
	/// </summary>
	public class BNpcIdle : BNpcBase
	{
		// Singleton instance of this state
		public readonly static BNpcIdle Instance = new BNpcIdle();

		public override BhvStatus Invoke( Npc a, BhvTree b )
		{
			if ( a.TargetObj != null && b.SeeTarget)
			{
				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}
				
			if (a.MovePos != Vector3.zero)
			{
				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			float nSpd = a.Params.MoveSpd * a.Params.MoveWalk;

			// Set Speed accordingly
			if ( a.NavAgent.speed != nSpd )
			SetSpeed( a, nSpd );

			if ( b.TimerAct >= a.Params.RoamTime )
			{
				b.TimerAct = 0.0f;

				// if (Random.Range(1, 20) > 10)
				// RpcTriggerSound(Apex.SndT.IDLE, Random.Range(0.0f, Apex.SND_MAX_DELAY));

				if ( a.Params.Roam )
				{
					bool doRoam = false;
					Debug.Log( "Checking if " + a.Name + " should roam..." );

					if ( a.TargetPos == Vector3.zero )
					{
						if ( UnityEngine.Random.Range( 1, 20 ) > 10 )
						{
							Debug.Log( a.Name + " is attempting to roam" );

							Vector3 nPos = FindRoamPoint( a );

							if ( nPos != Vector3.zero )
							{
								a.SetMovePos( nPos );
								doRoam = true;
							}
						}
					}
					else	// We have something that's piqued our interest
					{					
						a.SetMovePos( a.TargetPos );
						a.SetTargetPos( Vector3.zero );
						doRoam = true;
					}

					if ( doRoam )
					{
						Debug.Log( a.Name + " is roaming" );
						b.SetStatus( BhvStatus.SUCCESS );
						return b.Status;
					}
				}
			}

			GetFlatVectors( a, out Vector3 thisPos, out Vector3 fwd );

			if ( a.TargetPos != Vector3.zero )
			{
				float angle = Vector3.Angle( fwd, (a.TargetPos - thisPos) );

				if ( angle > a.Params.RotAngle )
				{
					// Reverse walk anim to differentiate from regular walking
					if ( !b.AnimInfo.IsName( AnimString.WALKBACK ) )
						SetAnimTrigger( a, AnimString.WALKBACK );

					TurnToward( a, a.TargetPos );
				}
				else
				{
					if ( !b.AnimInfo.IsName( AnimString.IDLE ) )
						SetAnimTrigger( a, AnimString.IDLE );
				}
			}
			else
			{
				if ( !b.AnimInfo.IsName( AnimString.IDLE ) )
					SetAnimTrigger( a, AnimString.IDLE );
			}

			if ( b.TimerCheck >= a.Params.ChkIdle )
			{
				b.TimerCheck = 0.0f;
				CheckEnv( a, b );
			}

			return b.Status;
		}
	}
}
