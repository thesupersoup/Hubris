using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
    interface IUsable
    {
        // IUsable methods
        bool TryUse();
        void OnUse();
    }
}
