using System;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Represents a slot in the inventory
	/// </summary>
	[Serializable]
	public class InvSlot
	{
		[SerializeField]
		private InvItem _item;

		public InvItem Item { get { return _item; } protected set { _item = value; } }

		InvSlot(InvItem nItem = null)
		{
			Item = nItem;
		}

		~InvSlot()
		{
			Item = null;
		}
	}
}
