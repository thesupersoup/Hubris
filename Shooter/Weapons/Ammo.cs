namespace Hubris
{
	/// <summary>
	/// Represents one particular type of ammo and implements Counter interface
	/// </summary>
	public struct Ammo : ICounter
	{
		public enum Type { _DARTS = 0, _9MM, _40, _556, _12G, NUM_TYPES }

		private Type _type;     // Ammo type
		private int _total;     // Total of type currently held
		private int _max;       // Max ammo of type that can be held

		public Type AmmoType => _type;
		public int Total => _total;
		public int Max => _max;

		public Ammo(Type nType, int nAmt, int nMax)
		{
			_type = nType;
			_total = nAmt;
			_max = nMax;
		}

		/// <summary>
		/// Return true if arguement added to ammo total is less than or equal to max, else set total to max and return false
		/// </summary>
		public bool Inc(int nAmt)
		{
			return ((_total += nAmt) <= _max) ? true : ((_total = _max) > _max);
		}

		/// <summary>
		/// Return true if arguement subtracted from ammo total is >= 0, else set total to 0 and return false
		/// </summary>		
		public bool Dec(int nAmt)
		{
			return ((_total -= nAmt) >= 0) ? true : ((_total = 0) < 0);
		}
	}
}
