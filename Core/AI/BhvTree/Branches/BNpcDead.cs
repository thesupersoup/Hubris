using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	public class BNpcDead : BNpcBase
	{
		// Singleton instance of this class
		public readonly static BNpcDead Instance = new BNpcDead();

		public override BhvStatus Invoke( BhvTree b, Npc a )
		{
			if (!a.Stats.IsDead)
			{
				SetAnimBool(a, "isDead", false);
				b.SetStatus( BhvStatus.FAILURE );
				return b.Status;
			}

			SetAnimBool( a, "isDead", true );

			return b.Status;
		}
	}
}
