using System;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Struct which contains damage specific stats for Npcs, like type and value
	/// </summary>
	[Serializable]
	public struct NpcDamageStats
	{
		///--------------------------------------------------------------------
		/// NpcDamageStats instance vars
		///--------------------------------------------------------------------

		[SerializeField]
		private DmgType _commonType;
		[SerializeField]
		private int _commonAmt;
		[SerializeField]
		private DmgType _specType;
		[SerializeField]
		private int _specAmt;

		///--------------------------------------------------------------------
		/// NpcDamageStats properties
		///--------------------------------------------------------------------

		public DmgType CommonType => _commonType;
		public int CommonAmount => _commonAmt;
		public DmgType SpecType => _specType;
		public int SpecAmount => _specAmt;

		///--------------------------------------------------------------------
		/// NpcDamageStats methods
		///--------------------------------------------------------------------

		public NpcDamageStats( DmgType cType = DmgType.BASE, int cAmt = 0, DmgType sType = DmgType.BASE, int sAmt = 0 )
		{
			_commonType = cType;
			_commonAmt = cAmt;
			_specType = sType;
			_specAmt = sAmt;
		}

		public void SetCommonType( DmgType type )
		{
			_commonType = type;
		}

		public void SetCommonAmount( int amt )
		{
			_commonAmt = amt;
		}

		public void SetSpecType( DmgType type )
		{
			_specType = type;
		}

		public void SetSpecAmount( int amt )
		{
			_specAmt = amt;
		}
	}
}
