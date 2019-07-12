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
		[SerializeField]
		protected WeaponAlt _alt = null;

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

		public override void Interact0( Camera pCam, LayerMask mask, LiveEntity owner )
		{
			if ( Physics.Raycast( pCam.transform.position, pCam.transform.forward, out RaycastHit hit, Range, mask ) )
			{
				GameObject target = hit.transform.root.gameObject;  // Grab root because hitboxes
				IDamageable ent = target.GetComponent<IDamageable>();

				if ( ent != null )
				{
					ent.TakeDmg( DamageType, Damage, false );
				}
			}
		}

		public override void Interact1( Camera pCam, LayerMask mask, LiveEntity owner )
		{
			if ( _alt == null )
				return;

			_alt.Invoke( pCam, mask, this, owner );
		}
	}
}
