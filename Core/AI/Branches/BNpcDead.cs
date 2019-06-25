using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	public class BNpcDead : BNpcBase
	{
		// Singleton instance of this class
		public readonly static BNpcDead Instance = new BNpcDead();

		public override void Invoke( BhvTree b, Npc a )
		{
			SetAnimBool(a, "isDead", true);

			if (!a.Stats.IsDead)
			{
				SetAnimBool(a, "isDead", false);
				Status = BhvStatus.FAILURE;
				return;
			}

			Status = BhvStatus.SUCCESS;
		}
	}
}
