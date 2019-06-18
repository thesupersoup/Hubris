using System;
using UnityEngine;

namespace Hubris
{
    /// <summary>
    /// Example weapon class, derived from ScriptableObject for Editor functionality
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Hubris/Weapon", order = 1)]
    public class Weapon : InvItem
    {
        [SerializeField]
        private string _name;
        [SerializeField]
        private DmgType _dmgType;
        [SerializeField]
        private int _dmg;
        [SerializeField]
        private int _range;

        public string Name { get { return _name; } protected set { _name = value; } }
        public DmgType DamageType { get { return _dmgType; } protected set { _dmgType = value; } }
        public int Damage { get { return _dmg; } protected set { _dmg = value; } }
        public int Range { get { return _range; } protected set { _range = value; } }

        public Weapon(string nName, DmgType nType, int nDmg, int nRange)
        {
            Name = nName;
            DamageType = nType;
            Damage = nDmg;
            Range = nRange;
        }

        public virtual void Interact0()
        {

        }

        public virtual void Interact1()
        {

        }
    }
}
