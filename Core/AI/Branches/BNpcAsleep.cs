using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
    public class BNpcAsleep : BNpcBase
    {
        // Singleton instance of this class
        public readonly static BNpcAsleep Instance = new BNpcAsleep();

        public override void Invoke(Npc a)
        {
            SetAnimBool(a, "isAsleep", true);

            if (a.Stats.IsDead)
            {
                ChangeBranch(a, BNpcDead.Instance);
                return;
            }

            if (!a.Stats.IsAsleep)
            {
                SetAnimBool(a, "isAsleep", false);
                ChangeBranch(a, BNpcIdle.Instance);
                return;
            }
        }
    }
}
