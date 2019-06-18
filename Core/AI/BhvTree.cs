using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
    /// <summary>
    /// The root object of an AI behavior tree; contains a reference to the active branch, which can be changed by the children
    /// </summary>
    public class BhvTree : IBhvTree
    {
        ///--------------------------------------------------------------------
        /// BhvTree instance vars
        ///--------------------------------------------------------------------

        private IBhvBranch _active;

        ///--------------------------------------------------------------------
        /// BhvTree properties
        ///--------------------------------------------------------------------

        public IBhvBranch ActiveBranch { get { return _active; } protected set { _active = value; } }

        ///--------------------------------------------------------------------
        /// BhvTree methods
        ///--------------------------------------------------------------------

        public BhvTree(IBhvBranch nActive = null)
        {
            _active = nActive;
        }

        public void Invoke(Npc a)
        {
            if (a.Stats.IsDead)
            {
                if (ActiveBranch != BNpcDead.Instance)
                    ActiveBranch = BNpcDead.Instance;
            }
            else
            {
                if (a.Stats.IsAsleep)
                {
                    if (ActiveBranch != BNpcAsleep.Instance)
                        ActiveBranch = BNpcAsleep.Instance;
                }
            }

            if (ActiveBranch != null)
                ActiveBranch.Invoke(a);
        }
    }
}
