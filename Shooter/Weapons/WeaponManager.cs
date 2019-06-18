using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
    public class WeaponManager
    {
        //[SerializeField] private WeaponObj[] weaponBook;
        [SerializeField] private GameObject weaponDock;
        //[SerializeField] private Loadout weaponLoadout = null;
        //WeaponObj[] weapons;

        private AmmoManager _ammoMgr = new AmmoManager();
        private GameObject wObj = null;                 // The GameObject attached to weaponDock which matches the active weapon
        private Animator activeAnim = null;             // Holding variable for main weapon Animator
        private ParticleSystem[] activeEffects = null;  // Holding variable for weapon ParticleSystem effects
        private Light activeLight = null;               // Holding variable for active weapon Light effect
        private AudioSource activeAudio = null;         // Holding variable for active weapon AudioSource
        private AudioClip[] activeFireArray = null;     // Holding variable for active weapon fireArray sounds
        private AudioClip activeDryFire = null;         // Holding variable for active weapon dryFire sound
        private bool activeIsMag = false;               // Is the activeWeapon magazine-fed (true), or not? (false, one shell/cartridge at a time)
        private AudioClip activeReloadIn = null;        // Holding variable for active weapon reloadIn sound
        private AudioClip activeReloadOut = null;       // Holding variable for active weapon reloadOut sound
        private AudioClip activeReloadSingle = null;    // Holding variable for active weapon reloadSingle sound
        private AudioClip activeReloadCharge = null;    // Holding variable for active weapon reloadCharge sound
        private AudioClip activeWeaponHandle = null;    // Holding variable for active weapon handling sound
        private bool canFire;

        public int activeIndex; // Index of active WeaponSlot
    }
}
