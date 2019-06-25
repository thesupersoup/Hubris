namespace Hubris
{
	/// <summary>
	/// Represents a single magazine for a weapon
	/// </summary>
	public struct Magazine : ICounter
	{
		private int _amt;   // Current ammo in mag
		private int _max;   // Max ammo in mag

		public int Amt => _amt;
		public int Max => _max;

		public Magazine(int nAmt, int nMax)
		{
			_amt = nAmt;
			_max = nMax;
		}

		/// <summary>
		/// Return true if arguement added to ammo total is less than or equal to max, else set total to max and return false
		/// </summary>
		public bool Inc(int nAmt)
		{
			return ((_amt += nAmt) <= _max) ? true : ((_amt = _max) > _max);
		}

		/// <summary>
		/// Return true if arguement subtracted from ammo total is >= 0, else set total to 0 and return false
		/// </summary>		
		public bool Dec(int nAmt)
		{
			return ((_amt -= nAmt) >= 0) ? true : ((_amt = 0) < 0);
		}
	}
}
