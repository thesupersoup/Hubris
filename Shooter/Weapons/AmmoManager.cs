using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Stores and manages all of the different ammo types available to the player
	/// </summary>
	public class AmmoManager
	{
		///--------------------------------------------------------------------
		///	AmmoManager constants
		///--------------------------------------------------------------------
		
			// Default maximum
		public const int MAX_DEF = 100;

		// Maximum amounts of each ammo type
		public const int MAX_DARTS = 20, MAX_9MM = 120, MAX_40 = 80,
							MAX_556 = 120, MAX_12G = 48;

		///--------------------------------------------------------------------
		///	AmmoManager instance vars
		///--------------------------------------------------------------------

		private int[] _ammoCount = new int[(int)AmmoType.NUM_TYPES];

		///--------------------------------------------------------------------
		///	AmmoManager properties
		///--------------------------------------------------------------------

		public int[] AmmoArr { get { return _ammoCount; } protected set { _ammoCount = value; } }

		///--------------------------------------------------------------------
		///	AmmoManager methods
		///--------------------------------------------------------------------

		public AmmoManager()
		{
			for (int i = 0; i < _ammoCount.Length; i++)
			{
				switch (i)
				{
					case (int)AmmoType._DARTS:	// Darts
						_ammoCount[i] = MAX_DARTS;
						break;
					case (int)AmmoType._9MM:	// 9mm
						_ammoCount[i] = MAX_9MM;
						break;
					case (int)AmmoType._40:		// .40
						_ammoCount[i] = MAX_40;
						break;
					case (int)AmmoType._556:	// 5.56
						_ammoCount[i] = MAX_556;
						break;
					case (int)AmmoType._12G:	// 12 gauge ammo
						_ammoCount[i] = MAX_12G;
						break;
					default:
						_ammoCount[i] = MAX_DEF;
						break;
				}
			}

			Report();
		}

		public int GetAmmoMax(int index) // Get the maximum amount of ammo for the specified type
		{
			int max = 0;
			switch (index)
			{
				case (int)AmmoType._DARTS:	// Darts
					max = MAX_DARTS;
					break;
				case (int)AmmoType._9MM:	// 9mm
					max = MAX_9MM;
					break;
				case (int)AmmoType._40:		// .40
					max = MAX_40;
					break;
				case (int)AmmoType._556:	// 5.56
					max = MAX_556;
					break;
				case (int)AmmoType._12G:	// 12 Gauge
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
			if (index >= 0 && index < _ammoCount.Length)
			{
				return _ammoCount[index];
			}
			else
				return 0;
		}

		public bool DeductAmmo(int index, int amt)
		{
			if (index >= 0 && index < _ammoCount.Length && (_ammoCount[index] - amt) >= 0)
			{
				_ammoCount[index] -= amt;
				return true;
			}
			else
				return false;
		}

		public bool AddAmmo(int index, int amt)
		{
			if (index >= 0 && index < _ammoCount.Length)
			{
				int max = GetAmmoMax(index);
				if (_ammoCount[index] + amt <= max)
					_ammoCount[index] += amt;
				else
					_ammoCount[index] = max;
				return true;
			}
			else
				return false;
		}

		public void DeductAmmoReport(int index, int amt)
		{
			Debug.Log("Ammo count before deduction: " + _ammoCount[index]);
			DeductAmmo(index, amt);
			Debug.Log("Ammo count after deduction: " + _ammoCount[index]);
		}

		public void Report()
		{
			Debug.Log("Current Ammo Status:\nDarts (" + (int)AmmoType._DARTS + "): " + _ammoCount[(int)AmmoType._DARTS] + "/" + MAX_DARTS + "\n9mm (" + (int)AmmoType._9MM + "): "
						+ _ammoCount[(int)AmmoType._9MM] + "/" + MAX_9MM + "\n.40 (" + (int)AmmoType._40 + "): " + _ammoCount[(int)AmmoType._40] + "/" + MAX_40 + "\n5.56 ("
						+ (int)AmmoType._556 + "): " + _ammoCount[(int)AmmoType._556] + "/" + MAX_556);
		}
	}
}