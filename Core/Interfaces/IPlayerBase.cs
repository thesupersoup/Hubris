using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
    /// <summary>
    /// Base Interface for all Player Entities
    /// </summary>
    interface IPlayerBase
    {
        void InteractA();
        void InteractB();
        void Move(InputManager.Axis ax, float val);
        void Rotate(InputManager.Axis ax, float val);
        void PhysMove(Vector3 dir, ForceMode force);
        Vector3 GetMoveAsVector(InputManager.Axis ax, float val, bool relative = false);
    }
}
