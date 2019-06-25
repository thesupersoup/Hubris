namespace Hubris
{
	/// <summary>
	/// Implementation of a BaseTickable as Cooldown
	/// </summary>
	public class Cooldown : BaseTickable
	{
		// Methods
		public Cooldown(int nVal, int nMax, int nMin, int nAmt, bool nStart)
		{
			Name = "Cooldown";
			Value = nVal;
			Max = nMax;
			Min = nMin;
			Mod = nAmt;
			Suspended = false;
			Decay = true;

			if(nStart)
			{
				Start(Value);
			}

			SubTick();  // UnsubTick handled in default Dispose() implementation
		}
	}
}
