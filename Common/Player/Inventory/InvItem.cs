using System;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Represents any item that can interact with the Inventory system
	/// </summary>
	[Serializable]
	public class InvItem : ScriptableObject, IItem
	{
		[SerializeField]
		private float _weight;

		public float Weight { get { return _weight; } protected set { _weight = value; } }

		public virtual bool Interact0( Camera pCam, LayerMask mask, LiveEntity owner )
		{
			// Override with unique implementation
			return true;
		}

		public virtual bool Interact1( Camera pCam, LayerMask mask, LiveEntity owner )
		{
			// Override with unique implementation
			return true;
		}
	}
}
