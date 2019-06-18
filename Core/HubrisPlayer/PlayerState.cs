namespace Hubris
{
    // Player State class
    public class PlayerState
    {
        // Player state enum
        public enum State { STAND = 0, MOVE, AIR, SWIM, NUM_STATES };
        // Flag enum
        public enum Flag { DEAD = 0, CROUCHED, DEMIGOD, GRAV, LOCKED, NUM_FLAGS };

        // Player's current state
        public State Player { get; protected set; }

        // Flag array
        public bool[] Flags { get; protected set; }     // Array of the flags a player can have


        public PlayerState(bool[] nFlags = null)
        {
            Init(nFlags);
        }

        public void Init(bool[] nFlags = null)
        {
            Player = State.STAND;
            if (nFlags == null)
            {
                Flags = new bool[(int)Flag.NUM_FLAGS];

                Flags[(int)Flag.DEAD] = false;
                Flags[(int)Flag.CROUCHED] = false;
                Flags[(int)Flag.DEMIGOD] = false;
                Flags[(int)Flag.GRAV] = true;
                Flags[(int)Flag.LOCKED] = false;
            }
            else
            {
                Flags = nFlags;
            }
        }

        /*
         * Attempt to change player state to specified value; return false if state could not be changed 
         * (if player is dead and they attempt to jump, for instance)
         */
        public bool SetState(State nVal)
        {
            bool success = false;

            if (nVal >= State.STAND && nVal < State.NUM_STATES)
            {
                Player = nVal;
                success = true;
            }
            else
            {
                LocalConsole.Instance.LogError("Attempted to change to invalid state " + (int)nVal, true);
            }

            return success;
        }

        /*
         * Attempt to set a flag to specified value; log error to console on failure
         */
        public void SetFlag(Flag nFlag, bool nVal)
        {
            if (nFlag >= Flag.DEAD && nFlag < Flag.NUM_FLAGS)
            {
                Flags[(int)nFlag] = nVal;
            }
            else
            {
                LocalConsole.Instance.LogError("Attempted to set invalid flag " + (int)nFlag, true);
            }
        }
    }
}