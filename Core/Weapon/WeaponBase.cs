using System;
using UnityEngine;

namespace Hubris
{
    /// <summary>
    /// Example weapon class, derived from ScriptableObject for Editor functionality
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Hubris/Weapon", order = 1)]
    public class WeaponBase : InvItem
    {
        [SerializeField]
		protected string _name;
        [SerializeField]
		protected DmgType _dmgType;
        [SerializeField]
		protected int _dmg;
        [SerializeField]
        protected int _range;
		[SerializeField]
		protected AudioClip[] _weaponSfx = new AudioClip[(int)WeaponSfx.NUM_SFX];

        public string Name			{ get { return _name; } protected set { _name = value; } }
        public DmgType DamageType	{ get { return _dmgType; } protected set { _dmgType = value; } }
        public int Damage			{ get { return _dmg; } protected set { _dmg = value; } }
        public int Range			{ get { return _range; } protected set { _range = value; } }
		public AudioClip[] Sfx		{ get { return _weaponSfx; } protected set { _weaponSfx = value; } }
		public int NumSfx			{ get { return _weaponSfx.Length; } }

        public WeaponBase(string nName, DmgType nType, int nDmg, int nRange)
        {
            Name = nName;
            DamageType = nType;
            Damage = nDmg;
            Range = nRange;
        }

        public override void Interact0()
        {

        }

        public override void Interact1()
        {

        }
    }
}
