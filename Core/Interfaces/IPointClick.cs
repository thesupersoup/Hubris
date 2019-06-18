using UnityEngine;

namespace Hubris
{
    /// <summary>
    /// Interface for Entities which can be moved by the player via point-and-click
    /// </summary>
    interface IPointClick
    {
        void SetMovePos(Vector3 nPos);
        void CheckMove();
        void CancelMove();
    }
}
