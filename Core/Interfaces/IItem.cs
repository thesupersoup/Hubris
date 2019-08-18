using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Interface for Items in the Inventory which have primary and secondary functions when equipped
	/// </summary>
	public interface IItem
	{
		bool Interact0( Camera pCam, LayerMask mask, LiveEntity owner );
		bool Interact1( Camera pCam, LayerMask mask, LiveEntity owner );
	}
}
