using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hubris
{
    /// <summary>
    /// Interface which provides functionality for use on each time interval (tick)
    /// </summary>
    interface ITick
    {
        void Tick();            // Use with Update() or FixedUpdate() on a given time interval
        void LateTick();        // Use with LateUpdate() on a given time interval
    }
}
