using UnityEngine;
using System.Collections;

namespace Hubris
{
    /// <summary>
    /// Interface for things that are to be incremented and decremented
    /// </summary>
    interface ICounter
    {
        bool Inc(int nAmt);   // Increment method
        bool Dec(int nAmt);   // Decrement method
    }
}
