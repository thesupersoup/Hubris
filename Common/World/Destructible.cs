using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Represents a destructible object in the environment, with a physical and visual presence
	/// </summary>
	public class Destructible : InterestPoint, IDamageable, IRespawnable
	{
		///--------------------------------------------------------------------
		/// Destructible instance vars
		///--------------------------------------------------------------------

		[SerializeField]
		private Collider _col = null;
		[SerializeField]
		private Renderer _visual = null;
		[SerializeField]
		private int _maxHp;
		[SerializeField]
		private int _hp;

		///--------------------------------------------------------------------
		/// Destructible properties
		///--------------------------------------------------------------------

		public Collider Phys
		{
			get { return _col; }
		}

		public Renderer Visual
		{
			get { return _visual; }
		}

		public int MaxHp
		{
			get { return _maxHp; }
			protected set { _maxHp = value; }
		}

		public int Hp
		{
			get { return _hp; }
			protected set { _hp = value; }
		}

		///--------------------------------------------------------------------
		/// Destructible methods
		///--------------------------------------------------------------------

		public override void OnEnable()
		{
			Hp = MaxHp;
			base.OnEnable();
		}

		public override void SetActive(bool nAct)
		{
			if (nAct)
				Activate();
			else
				Deactivate();

			if (Phys != null)
				Phys.enabled = Active;

			if (Visual != null)
				Visual.enabled = Active;
		}

		public bool TakeDmg(DmgType nType, int nDmg, bool nDirect)
		{
			if (Active && Hp > 0 && nType != DmgType.STAMINA)
			{
				Hp -= nDmg;
				return true;
			}
			else
				return false;
		}

		public void Respawn()
		{
			Hp = MaxHp;
			SetActive(true);
		}

		void Update()
		{
			if(Active && Hp <= 0)
			{
				Hp = 0;
				NotifyObservers(true);
				SetActive(false);
			}
		}
	}
}
