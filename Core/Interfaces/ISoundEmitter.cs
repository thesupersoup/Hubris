using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Interface for objects which can emit sound events
	/// </summary>
	public interface ISoundEmitter
	{
		void EmitSoundEvent( SoundEvent ev );
	}
}