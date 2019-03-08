using UnityEngine;

namespace Hubris
{
    /// <summary>
    /// Interface for Entities which can be moved by the player via point-and-click
    /// </summary>
    interface IMovable
    {
        void SetTargetPos(Vector3 nPos);
        void CheckMove();
        void CancelMove();
    }
}
