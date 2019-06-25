using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	public class AmmoManager
	{
		// Default maximum in the event of an unrecognized ammo type
		public const int MAX_DEF = 100;

		// Maximum amounts of each ammo type
		public const int MAX_DARTS = 20, MAX_9MM = 120, MAX_40 = 80,
							MAX_556 = 120, MAX_12G = 48;

		private int[] ammoCount = new int[(int)Ammo.Type.NUM_TYPES];

		public AmmoManager()
		{
			for (int i = 0; i < ammoCount.Length; i++)
			{
				switch (i)
				{
					case (int)Ammo.Type._DARTS:     // Darts
						ammoCount[i] = MAX_DARTS;
						break;
					case (int)Ammo.Type._9MM: // 9mm
						ammoCount[i] = MAX_9MM;
						break;
					case (int)Ammo.Type._40: // .40
						ammoCount[i] = MAX_40;
						break;
					case (int)Ammo.Type._556: // 5.56
						ammoCount[i] = MAX_556;
						break;
					case (int)Ammo.Type._12G: // 12 gauge ammo
						ammoCount[i] = MAX_12G;
						break;
					default:
						ammoCount[i] = MAX_DEF;
						break;
				}
			}
			StatusReport();
		}

		public int GetAmmoMax(int index) // Get the maximum amount of ammo for the specified type
		{
			int max = 0;
			switch (index)
			{
				case (int)Ammo.Type._DARTS: // Darts
					max = MAX_DARTS;
					break;
				case (int)Ammo.Type._9MM: // 9mm
					max = MAX_9MM;
					break;
				case (int)Ammo.Type._40: // .40
					max = MAX_40;
					break;
				case (int)Ammo.Type._556: // 5.56
					max = MAX_556;
					break;
				case (int)Ammo.Type._12G: // 12 Gauge
					max = MAX_12G;
					break;
				default:
					max = MAX_DEF;
					break;
			}
			return max;
		}

		public int GetAmmoCount(int index) // Get the current amount of ammo for the specified type
		{
			if (index >= 0 && index < ammoCount.Length)
			{
				return ammoCount[index];
			}
			else
				return 0;
		}

		public bool ReduceAmmoCount(int index, int amt)
		{
			if (index >= 0 && index < ammoCount.Length && (ammoCount[index] - amt) >= 0)
			{
				ammoCount[index] -= amt;
				return true;
			}
			else
				return false;
		}

		public bool IncreaseAmmoCount(int index, int amt)
		{
			if (index >= 0 && index < ammoCount.Length)
			{
				int max = GetAmmoMax(index);
				if (ammoCount[index] + amt <= max)
					ammoCount[index] += amt;
				else
					ammoCount[index] = max;
				return true;
			}
			else
				return false;
		}

		public void DeductAmmo(int index, int amt)
		{
			Debug.Log("Ammo count before deduction: " + ammoCount[index]);
			ReduceAmmoCount(index, amt);
			Debug.Log("Ammo count after deduction: " + ammoCount[index]);
		}

		public void StatusReport()
		{
			Debug.Log("Current Ammo Status:\nDarts (" + (int)Ammo.Type._DARTS + "): " + ammoCount[(int)Ammo.Type._DARTS] + "/" + MAX_DARTS + "\n9mm (" + (int)Ammo.Type._9MM + "): "
						+ ammoCount[(int)Ammo.Type._9MM] + "/" + MAX_9MM + "\n.40 (" + (int)Ammo.Type._40 + "): " + ammoCount[(int)Ammo.Type._40] + "/" + MAX_40 + "\n5.56 ("
						+ (int)Ammo.Type._556 + "): " + ammoCount[(int)Ammo.Type._556] + "/" + MAX_556);
		}
	}
}