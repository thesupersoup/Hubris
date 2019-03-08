namespace Hubris
{
    /// <summary>
    /// Interface which provides Entities with time interval (tick) functinoality
    /// </summary>
    interface ITickable
    {
        void Tick();            // Use with Update() or FixedUpdate() on a given time interval
        void LateTick();        // Use with LateUpdate() on a given time interval
    }
}
