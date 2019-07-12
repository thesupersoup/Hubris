using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	public class BNpcAsleep : BNpcBase
	{
		// Singleton instance of this class
		public readonly static BNpcAsleep Instance = new BNpcAsleep();

		public override BhvStatus Invoke( Npc a, BhvTree b )
		{
			if ( a.NavAgent.hasPath )
				StopMove( a );

			if (a.Stats.IsDead)
			{
				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			if (!a.Stats.IsAsleep)
			{
				SetAnimBool(a, "isAsleep", false);
				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			SetAnimBool( a, "isAsleep", true );

			return b.Status;
		}
	}
}
