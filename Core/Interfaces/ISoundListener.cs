namespace Hubris
{
	/// <summary>
	/// Interface for objects which can receive sound events
	/// </summary>
	public interface ISoundListener
	{
		void AddSoundEvent( SoundEvent ev );
	}
}
