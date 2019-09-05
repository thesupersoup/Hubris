namespace Hubris
{
	// Player State class
	/*public class PlayerState
	{
		// Flag enum
		public enum Flag { CROUCHED, GRAV, LOCKED, NUM_FLAGS };

		// Player's current state
		public int Current { get; protected set; }

		// Flag array
		public bool[] Flags { get; protected set; }     // Array of the flags a player can have


		public PlayerState(bool[] nFlags = null)
		{
			Init(nFlags);
		}

		public void Init(bool[] nFlags = null)
		{
			Current = 0;
			if (nFlags == null)
			{
				Flags = new bool[(int)Flag.NUM_FLAGS];

				Flags[(int)Flag.CROUCHED] = false;
				Flags[(int)Flag.GRAV] = true;
				Flags[(int)Flag.LOCKED] = false;
			}
			else
				Flags = nFlags;
		}

		/// <summary>
		///	Attempt to change player state to specified value; return false if state could not be changed 
		///	(if player is dead and they attempt to jump, for instance)
		/// </summary>	
		public bool SetState( int val )
		{
			bool success = false;

			if (nVal >= 0 && nVal < State.NUM_STATES)
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

		/// <summary>
		/// Attempt to set a flag to specified value; log error to console on failure
		/// </summary>
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
	}*/
}
 