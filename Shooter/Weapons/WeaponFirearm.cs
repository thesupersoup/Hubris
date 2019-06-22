using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Class for projectile weapons derived from Weapon : InvItem
	/// </summary>
	[Serializable]
	[CreateAssetMenu(fileName = "NewWeaponFirearm", menuName = "Hubris/WeaponFirearm", order = 2)]
	public class WeaponFirearm : WeaponBase
	{
		///--------------------------------------------------------------------
		///	WeaponFirearm instance vars
		///--------------------------------------------------------------------

		[SerializeField]
		private bool _isMag;			// Is the weapon magazine-fed (true), or not? (false, one shell/cartridge at a time)
		[SerializeField]
		private AmmoType _ammoType;		// What type of ammunition does the firearm use
		[SerializeField]
		private int _ammo;				// Current amount of ammunition in the weapon
		[SerializeField]
		private int _ammoMax;			// Maximum amount of ammo the weapon can hold

		///--------------------------------------------------------------------
		///	WeaponFirearm properties
		///--------------------------------------------------------------------	

		public bool IsMag
		{ get { return _isMag; } protected set { _isMag = value; } }

		public AmmoType AmmoType
		{ get { return _ammoType; } protected set { _ammoType = value; } }

		public int Ammo
		{ get { return _ammo; } protected set { _ammo = value; } }

		public int AmmoMax
		{ get { return _ammoMax; } protected set { _ammoMax = value; } }

		///--------------------------------------------------------------------
		///	WeaponFirearm methods
		///--------------------------------------------------------------------

		public WeaponFirearm(string nName, DmgType nType, int nDmg, int nRange) : 
			base(nName, nType, nDmg, nRange)
		{
			_weaponSfx = new AudioClip[(int)FirearmSfx.NUM_SFX];
		}

		public void LoadAmmo(int nAmmo)
		{
			_ammo = nAmmo;
		}

		public override void Interact0()
		{

		}

		public override void Interact1()
		{
			
		}
	}
}
