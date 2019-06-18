using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
    /// <summary>
    /// Interface for the root objects in Behavior Trees
    /// </summary>
    public interface IBhvTree
    {
        void Invoke(Npc a);
    }
}
