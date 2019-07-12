using System;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Enables unique behavior for a weapon
	/// </summary>
	[Serializable]
	[CreateAssetMenu( fileName = "NewWeaponAlt", menuName = "Hubris/WeaponAlt", order = 2 )]
	public class WeaponAlt : ScriptableObject
	{
		[SerializeField]
		private WeaponAltType _type;

		public virtual void Invoke( Camera pCam, LayerMask mask, WeaponBase weapon, LiveEntity owner )
		{
			switch( _type )
			{
				case WeaponAltType.CREATE_SOUND:
					if ( Physics.Raycast( pCam.transform.position, pCam.transform.forward, out RaycastHit hit, weapon.Range, mask ) )
						owner.EmitSoundEvent( new SoundEvent( null, hit.point, weapon.Range, SoundIntensity.NOTEWORTHY ) );
					break;
			}
		}
	}
}
