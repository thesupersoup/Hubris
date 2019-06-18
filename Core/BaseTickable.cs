using UnityEngine;
using System.Collections;


namespace Hubris
{
    /// <summary>
    /// Abstract class for deriving Tickable objects
    /// </summary>
    public abstract class BaseTickable : LogicalEntity, ITickable, ICounter, ITimer
    {
        // BaseTickable constants
        public static int DEF_VAL = 5000;   // Default value
        public static int MAX_VAL = 10000;  // Default maximum value
        public static int MIN_VAL = 0;      // Default minimum value
        public static int DEF_MOD = 5;      // Default mod 
        public static int MIN_MOD = 1;      // Minimum increment per timestep

        // BaseTickable instance vars
        private bool _dec;      // Is this BaseTickable decaying?
        private bool _sus;      // Is this BaseTickable suspended (prevented from changing on tick)?
        private int _val;       // Current value of BaseTickable, never lower than min or higher than max
        private int _max;       // Maximum value of BaseTickable, never higher than 100.0f
        private int _min;       // Minimum value of BaseTickable, never lower than 0.0f
        private int _mod;       // Amount to modify Value per timestep. Improvement or Decay of a need is a function of the Essential's mod * Interest mod.

        // BaseTickable properties
        public bool Decay
        {
            get { return _dec; }
            set { _dec = value; }
        }
        public bool Suspended
        {
            get { return _sus; }
            set { _sus = value; }
        }

        public int Value
        {
            get { return _val; }
            set
            {
                if (value >= _min && value <= _max)
                {
                    _val = value;
                }
                else
                {
                    if (value < _min)
                    {
                        _val = MIN_VAL;
                    }
                    else
                    {
                        _val = MAX_VAL;
                    }
                }
            }
        }

        public int Max
        {
            get { return _max; }
            set { if (value <= MAX_VAL) { _max = value; } else { _max = MAX_VAL; } }
        }

        public int Min
        {
            get { return _min; }
            set { if (value >= MIN_VAL) { _min = value; } else { _min = MIN_VAL; } }
        }

        public int Mod
        {
            get { return _mod; }
            set { if (value >= MIN_MOD) { _mod = value; } else { _mod = MIN_MOD; } }
        }

        // BaseTickable methods
        public BaseTickable()
        {
            Max = MAX_VAL;
            Min = MIN_VAL;
            Mod = MIN_MOD;
            Suspended = false;
            Decay = true;
        }

        public BaseTickable(int nMax, int nMin, int nMod, bool nSus = false, bool nDecay = true)
        {
            Max = nMax;
            Min = nMin;
            Mod = nMod;
            Suspended = nSus;
            Decay = nDecay;
        }

        public bool Start(int nAmt = 1)
        {
            bool success = false;

            if (Suspended)
            {
                Suspended = false;
                success = true;
                if (!Inc(nAmt))
                {
                    if (HubrisCore.Instance.Debug)
                    {
                        LocalConsole.Instance.Log(Name + " set to max", true);
                    }
                }
            }
            else
            {
                if (HubrisCore.Instance.Debug)
                {
                    LocalConsole.Instance.Log("Tried to start a " + Name + " that has already started", true);
                }
            }

            return success;
        }

        public bool Stop(bool nReset = true)
        {
            bool success = false;

            if (!Suspended)
            {
                Suspended = true;
                success = true;

                if (nReset)
                {
                    Value = Min;
                }
            }
            else
            {
                if (HubrisCore.Instance.Debug)
                {
                    LocalConsole.Instance.Log("Tried to stop a " + Name + " that is not started", true);
                }
            }

            return success;
        }

        public bool Inc(int nAmt)
        {
            bool success = true;

            if (Value + nAmt <= MAX_VAL)
            {
                Value += nAmt;
            }
            else
            {
                Value = MAX_VAL;
                success = false;
            }

            return success;
        }

        public bool Dec(int nAmt)
        {
            bool success = true;

            if (Value - nAmt >= MIN_VAL)
            {
                Value -= nAmt;
            }
            else
            {
                Value = MIN_VAL;
                success = false;
            }

            return success;
        }

        public override void Tick()
        {
            if (!Suspended)
            {
                if (Decay)
                {
                    if(!Dec(Mod))
                    {
                        Suspended = true;   // Value is at Min, suspend until restarted
                    }
                }
            }
        }

        public override void LateTick()
        {

        }

        public override void FixedTick()
        {

        }
    }
}
