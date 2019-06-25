using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	[Serializable]
	public class WeaponManager
	{
		///--------------------------------------------------------------------
		///	WeaponManager instance vars
		///--------------------------------------------------------------------

		[SerializeField]
		private GameObject _weaponDock; // GameObject for storing visible first person weapon models

		private AmmoManager _ammoMgr = new AmmoManager();
		private GameObject _wObj = null;                // The GameObject attached to weaponDock which matches the active weapon
		private Animator _activeAnim = null;            // Holding variable for main weapon Animator
		private ParticleSystem[] _activeEffects = null; // Holding variable for weapon ParticleSystem effects
		private Light _activeLight = null;              // Holding variable for active weapon Light effect
		private AudioSource _activeAudio = null;        // Holding variable for active weapon AudioSource
		private bool _canFire;

		[SerializeField]
		private int _activeIndex;       // Index of active WeaponSlot
		[SerializeField]
		private IItem _activeWeapon;

		///--------------------------------------------------------------------
		///	WeaponManager properties
		///--------------------------------------------------------------------

		public AmmoManager AmmoMgr
		{ get { return _ammoMgr; } protected set { _ammoMgr = value; } }

		public int ActiveIndex
		{ get { return _activeIndex; } protected set { _activeIndex = value; } }

		public IItem ActiveWeapon
		{ get { return _activeWeapon; } protected set { _activeWeapon = value; } }

		///--------------------------------------------------------------------
		///	WeaponManager methods
		///--------------------------------------------------------------------

		public int GetActiveAmmoCount()
		{
			if ( ActiveWeapon is WeaponFirearm firearm )
			{
				return firearm.Ammo;
			}
			else
				return -1;
		}

		public int GetActiveAmmoMax()
		{
			if ( ActiveWeapon is WeaponFirearm firearm )
			{
				return firearm.AmmoMax;
			}
			else
				return -1;
		}
	}
}
