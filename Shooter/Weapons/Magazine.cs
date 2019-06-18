namespace Hubris
{
    /// <summary>
    /// Represents a single magazine for a weapon
    /// </summary>
    public struct Magazine : ICounter
    {
        private int _amt;   // Current ammo in mag
        private int _max;   // Max ammo in mag

        public int Amt
        {
            get { return _amt; }
        }

        public int Max
        {
            get { return _max; }
        }

        public Magazine(int nAmt, int nMax)
        {
            _amt = nAmt;
            _max = nMax;
        }

        /*
         * Return true if arguement added to ammo is <= max, else set ammo to max and return false
         */
        public bool Inc(int nAmt)
        {
            return ((_amt += nAmt) <= _max) ? true : ((_amt = _max) > _max);
        }

        /*
         * Return true if arguement subtracted from ammo is >= 0, else set ammo to 0 and return false
         */
        public bool Dec(int nAmt)
        {
            return ((_amt -= nAmt) >= 0) ? true : ((_amt = 0) < 0);
        }
    }
}
