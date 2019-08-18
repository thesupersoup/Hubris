using System;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Class for storing shared LiveEntity stats and accessors
	/// </summary>
	[Serializable]
	public class EntStats
	{
		#region InstanceVars
		///--------------------------------------------------------------------
		/// EntStats instance vars
		///--------------------------------------------------------------------

		[SerializeField]
		protected int _hp;
		[SerializeField]
		protected int _hpMax;
		[SerializeField]
		protected int _ap;
		[SerializeField]
		protected int _apMax;
		[SerializeField]
		protected int _sta;
		[SerializeField]
		protected int _staMax;
		[SerializeField]
		protected bool _invisible;
		[SerializeField]
		protected bool _demigod;
		#endregion InstanceVars

		#region Properties
		///--------------------------------------------------------------------
		/// EntStats properties
		///--------------------------------------------------------------------

		public bool IsDead => Health <= 0.0f;
		public bool IsAsleep => Stamina <= 0.0f;
		public int Health { get { return _hp; } protected set { _hp = value; } }
		public int HealthMax { get { return _hpMax; } protected set { _hpMax = value; } }
		public bool Wounded => Health < HealthMax;
		public int Armor { get { return _ap; } protected set { _ap = value; } }
		public int ArmorMax { get { return _apMax; } protected set { _apMax = value; } }
		public int Stamina { get { return _sta; } protected set { _sta = value; } }
		public int StaminaMax { get { return _sta; } protected set { _staMax = value; } }

		/// <summary>
		/// Invisible determines whether this LiveEntity can be detected by others,
		/// and whether it emits sounds
		/// </summary>
		public bool Invisible { get { return _invisible; } protected set { _invisible = value; } }

		/// <summary>
		/// Demigod prevents any incoming damage sources
		/// </summary>
		public bool Demigod { get { return _demigod; } protected set { _demigod = value; } }
		#endregion Properties

		#region Methods
		///--------------------------------------------------------------------
		/// EntStats methods
		///--------------------------------------------------------------------

		public EntStats ( int nHp, int nHpMax, int nAp, int nApMax, int nSt, int nStMax, bool nInvis = false, bool nDemi = false )
		{
			Health = nHp;
			HealthMax = nHpMax;
			Armor = nAp;
			ArmorMax = nApMax;
			Stamina = nSt;
			StaminaMax = nStMax;
			Invisible = nInvis;
			Demigod = nDemi;
		}

		public void SetHealth ( int hp )
		{
			Health = hp;
		}

		public void SetArmor ( int ap )
		{
			Armor = ap;
		}

		public void SetStamina ( int sp )
		{
			Stamina = sp;
		}

		public void SetInvisible( bool invis )
		{
			Invisible = invis;
		}

		public void SetDemigod( bool demi )
		{
			Demigod = demi;
		}

		public bool AddHealth( int nHP ) // Returns true for successful addition, false if health is full
		{
			if ( Health >= HealthMax )
				return false; // Health is full, ideally

			Health += nHP;
			
			if ( Health > HealthMax )
				Health = HealthMax;

			return true;
		}

		public bool AddArmor( int nArmor ) // Returns true for successful addition, false if armor is full
		{
			if ( Armor >= ArmorMax )
				return false; // Armor is full, ideally

			Armor += nArmor;
			
			if ( Armor > ArmorMax )
				Armor = ArmorMax;

			return true;
		}

		public bool AddStamina( int nStamina ) // Returns true for successful addition, false if stamina is full
		{
			if ( Stamina >= StaminaMax )
				return false; // Stamina is full, ideally

			Stamina += nStamina;
			
			if ( Stamina > StaminaMax )
				Stamina = StaminaMax;

			return true;
		}

		public bool ReduceHealth( int nHP ) // Returns true for successful reduction, false if Health can not be reduced
		{
			if ( Demigod || Health <= 0 )
				return false;

			Health -= nHP;

			if ( Health < 0 )
				Health = 0;

			return true;
		}

		public bool ReduceArmor( int nArmor ) // Returns true for successful reduction, false if Armor can not be reduced
		{
			if ( Demigod || Armor <= 0 )
				return false;

			Armor -= nArmor;

			if ( Armor < 0 )
				Armor = 0;

			return true;
		}

		public bool ReduceStamina( int nStamina ) // Returns true for successful reduction, false if Stamina can not be reduced
		{
			if ( Demigod || Stamina <= 0 )
				return false;

			Stamina -= nStamina;

			if ( Stamina < 0 )
				Stamina = 0;

			return true;
		}
		#endregion Methods
	}
}
