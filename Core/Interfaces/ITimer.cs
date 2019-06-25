namespace Hubris
{
	/// <summary>
	/// Interface for Entities that represent timers
	/// </summary>
	interface ITimer
	{
		bool Start(int nAmt);
		bool Stop(bool nReset);
	}
}
