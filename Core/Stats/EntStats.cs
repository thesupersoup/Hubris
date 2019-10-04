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
			_hp = nHp;
			_hpMax = nHpMax;
			_ap = nAp;
			_apMax = nApMax;
			_sta = nSt;
			_staMax = nStMax;
			_invisible = nInvis;
			_demigod = nDemi;
		}

		public void SetHealth ( int hp )
		{
			_hp = hp;
		}

		public void SetArmor ( int ap )
		{
			_ap = ap;
		}

		public void SetStamina ( int sp )
		{
			_sta = sp;
		}

		public void SetInvisible( bool invis )
		{
			_invisible = invis;
		}

		public void SetDemigod( bool demi )
		{
			_demigod = demi;
		}

		public void Kill()
		{
			_hp = 0;
		}

		public bool AddHealth( int nHP ) // Returns true for successful addition, false if health is full
		{
			if ( _hp >= _hpMax )
				return false; // Health is full, ideally

			_hp += nHP;
			
			if ( _hp > _hpMax )
				_hp = _hpMax;

			return true;
		}

		public bool AddArmor( int nArmor ) // Returns true for successful addition, false if armor is full
		{
			if ( _ap >= _apMax )
				return false; // Armor is full, ideally

			_ap += nArmor;
			
			if ( _ap > _apMax )
				_ap = _apMax;

			return true;
		}

		public bool AddStamina( int nStamina ) // Returns true for successful addition, false if stamina is full
		{
			if ( _sta >= _staMax )
				return false; // Stamina is full, ideally

			_sta += nStamina;
			
			if ( _sta > _staMax )
				_sta = _staMax;

			return true;
		}

		public bool ReduceHealth( int nHP ) // Returns true for successful reduction, false if Health can not be reduced
		{
			if ( _demigod || _hp <= 0 )
				return false;

			_hp -= nHP;

			if ( _hp < 0 )
				_hp = 0;

			return true;
		}

		public bool ReduceArmor( int nArmor ) // Returns true for successful reduction, false if Armor can not be reduced
		{
			if ( _demigod || _ap <= 0 )
				return false;

			_ap -= nArmor;

			if ( _ap < 0 )
				_ap = 0;

			return true;
		}

		public bool ReduceStamina( int nStamina ) // Returns true for successful reduction, false if Stamina can not be reduced
		{
			if ( Demigod || Stamina <= 0 )
				return false;

			_sta -= nStamina;

			if ( _sta < 0 )
				_sta = 0;

			return true;
		}
		#endregion Methods
	}
}
