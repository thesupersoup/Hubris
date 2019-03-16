using System;

namespace Hubris
{
    public class Cooldown : LogicalEntity, IDisposable, ICounter, ITickable
    {
        int _val, _max, _min, _tickAmt;

        public Cooldown()
        {
            _val = 0;
            _max = 100;
            _min = 0;
            _tickAmt = 1;

            SubTick();  // UnsubTick handled in default Dispose() implementation
        }

        public Cooldown(int nVal, int nMax, int nMin, int nAmt)
        {
            _val = nVal;
            _max = nMax;
            _min = nMin;
            _tickAmt = nAmt;

            SubTick();  // UnsubTick handled in default Dispose() implementation
        }

        public int Value
        {
            get { return _val; }
            set { _val = value; }
        }

        public int Max
        {
            get { return _max; }
            set { _max = value; }
        }

        public int Min
        {
            get { return _min; }
            set { _min = value; }
        }

        public int TickAmt
        {
            get { return _tickAmt; }
            set { _tickAmt = value; }
        }

        public bool Inc(int nAmt = 1)
        {
            if ((Value + nAmt) < Max)
            {
                Value += nAmt;
                return true;
            }
            else
            {
                Value = Max;
                return false;
            }
        }

        public bool Dec(int nAmt = 1)
        {
            if ((Value - nAmt) >= 0)
            {
                Value -= nAmt;
                return true;
            }
            else
            {
                Value = 0;
                return false;
            }
        }

        public override void Tick()
        {
            if (Value > Min)
                Dec(TickAmt);
        }

        public override void LateTick()
        {

        }
    }
}
