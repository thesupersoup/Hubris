namespace Hubris
{
	/// <summary>
	/// Interface which provides Entities with Hubris driven time interval (tick) functionality
	/// </summary>
	interface ITickable
	{
		void Tick();            // Invoked with FixedUpdate() on a time interval specified in the GameManager
		void LateTick();        // Invoked with the LateUpdate() following a time interval specified in the GameManager
		void FixedTick();       // Invoked with every FixedUpdate()
	}
}
