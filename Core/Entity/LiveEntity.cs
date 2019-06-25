using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	public class LiveEntity : Entity, IDamageable
	{
		public const float ARMOR_ABSORB = 0.75f;

		public enum EType
		{
			BASE = 0,
			PLAYER,
			ENEMY,
			NUM_TYPES
		}

		[SerializeField]
		EType _type;
		[SerializeField]
		EntStats _stats;
		[SerializeField]
		private bool _alignSurface = true;        // Should this NPC align itself to the surface normal?

		public EType EntType { get { return _type; } protected set { _type = value; } }
		public EntStats Stats { get { return _stats; } protected set { _stats = value; } }
		public bool AlignToSurface { get { return _alignSurface; } protected set { _alignSurface = value; } }

		// Start is called before the first frame update
		protected virtual void Init()
		{
			Stats = EntStats.Create(EntType);
		}

		public bool TakeDmg(DmgType nType, int nDmg, bool nDirect)     // int nDmg is the amount of damage total, bool nDirect is true if the damage should directly damage or obey restrictions
		{
			if (nType == DmgType.STAMINA)
				return Stats.ReduceStamina(nDmg);
			else if (nType == DmgType.ARMOR)
				return Stats.ReduceArmor(nDmg);


			int modDmg = nDmg; // modDmg will change if armor takes any of the hit, leaving less to hit health
			bool armorCheck;

			if (!nDirect)
			{
				if (Stats.Armor > 0)
				{
					int armorLast = Stats.Armor;
					armorCheck = Stats.ReduceArmor((int)(modDmg * ARMOR_ABSORB));
					if (armorCheck)
						modDmg = modDmg - (armorLast - Stats.Armor);
				}
			}

			return Stats.ReduceHealth(modDmg);
		}


		// Update is called once per frame
		void Update()
		{

		}
	}
}
