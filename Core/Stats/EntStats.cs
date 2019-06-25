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

		public bool IsDead => Health <= 0.0f;
		public bool IsAsleep => Stamina <= 0.0f;
		public int Health { get { return _hp; } protected set { _hp = value; } }
		public int HealthMax { get { return _hpMax; } protected set { _hpMax = value; } }
		public int Armor { get { return _ap; } protected set { _ap = value; } }
		public int ArmorMax { get { return _apMax; } protected set { _apMax = value; } }
		public int Stamina { get { return _sta; } protected set { _sta = value; } }
		public int StaminaMax { get { return _sta; } protected set { _staMax = value; } }

		public EntStats(int nHp, int nHpMax, int nAp, int nApMax, int nSt, int nStMax)
		{
			Health = nHp;
			HealthMax = nHpMax;
			Armor = nAp;
			ArmorMax = nApMax;
			Stamina = nSt;
			StaminaMax = nStMax;
		}

		public bool AddHealth(int nHP) // Returns true for successful addition, false if health is full
		{
			if (Health < HealthMax)
			{
				if (Health + nHP <= HealthMax)
				{
					Health += nHP;
					return true;
				}
				else // (Health + nHP > HealthMax)
				{
					Health = HealthMax;
					return true;
				}
			}
			else
				return false; // Health is full, ideally
		}

		public bool AddArmor(int nArmor) // Returns true for successful addition, false if armor is full
		{
			if (Armor < ArmorMax)
			{
				if (Armor + nArmor <= ArmorMax)
				{
					Armor += nArmor;
					return true;
				}
				else // (Armor + nArmor > ArmorMax)
				{
					Armor = ArmorMax;
					return true;
				}
			}
			else
				return false; // Armor is full, ideally
		}

		public bool AddStamina(int nStamina) // Returns true for successful addition, false if stamina is full
		{
			if (Stamina < StaminaMax)
			{
				if (Stamina + nStamina <= StaminaMax)
				{
					Stamina += nStamina;
					return true;
				}
				else // (Stamina + nStamina > StaminaMax)
				{
					Stamina = StaminaMax;
					return true;
				}
			}
			else
				return false; // Stamina is full, ideally
		}

		public bool ReduceHealth(int nHP) // Returns true for successful reduction, false if Health would fall below negative HealthMax
		{
			if (Health - nHP >= -HealthMax)
			{
				Health -= nHP;
				return true;
			}
			else 
			{
				return false;
			}
		}

		public bool ReduceArmor(int nArmor) // Returns true for successful reduction, false if Armor would fall below negative ArmorMax
		{
			if (Armor - nArmor >= -ArmorMax)
			{
				Armor -= nArmor;
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool ReduceStamina(int nStamina) // Returns true for successful reduction, false if Stamina would fall below negative StaminaMax
		{
			if (Stamina - nStamina > -StaminaMax)
			{
				Stamina -= nStamina;
				return true;
			}
			else
			{
				return false;
			}
		}

		public static EntStats Create(LiveEntity.EType type)
		{
			EntStats stats;

			switch(type)
			{
				case LiveEntity.EType.BASE:
					stats = new EntStats(100, 100, 0, 0, 100, 100);
					break;
				case LiveEntity.EType.PLAYER:
					stats = new EntStats(100, 100, 0, 0, 100, 100);
					break;
				case LiveEntity.EType.ENEMY:
					stats = new EntStats(80, 80, 0, 100, 80, 80);
					break;
				default:
					stats = new EntStats(100, 100, 0, 0, 100, 100);
					break;
			}

			return stats;
		}
	}
}
