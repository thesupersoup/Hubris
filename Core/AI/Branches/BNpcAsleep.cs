using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	public class BNpcAsleep : BNpcBase
	{
		// Singleton instance of this class
		public readonly static BNpcAsleep Instance = new BNpcAsleep();

		public override void Invoke( BhvTree b, Npc a )
		{
			SetAnimBool(a, "isAsleep", true);

			if (a.Stats.IsDead)
			{
				Status = BhvStatus.FAILURE;
				return;
			}

			if (!a.Stats.IsAsleep)
			{
				SetAnimBool(a, "isAsleep", false);
				Status = BhvStatus.FAILURE;
				return;
			}

			Status = BhvStatus.SUCCESS;
		}
	}
}
