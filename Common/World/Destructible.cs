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
			_hp = _maxHp;
			base.OnEnable();
		}

		public override void SetHubrisActive(bool nAct)
		{

			if (_col != null)
				_col.enabled = Active;

			if (_visual != null)
				_visual.enabled = Active;
			
			CheckHealth();
		}

		public virtual bool TakeDmg( LiveEntity damager, int nType, int nDmg, bool nDirect)
		{
			if (_act && _hp > 0 && (DmgType)nType != DmgType.STAMINA)
			{
				_hp -= nDmg;
				CheckHealth();
				return true;
			}
			else
				return false;
		}

		public void Respawn()
		{
			_hp = _maxHp;
			SetHubrisActive( true );
		}

		void CheckHealth()
		{
			if(_act && _hp <= 0)
			{
				_hp = 0;
				NotifyObservers( true );
				SetHubrisActive( false );
			}
		}
	}
}
