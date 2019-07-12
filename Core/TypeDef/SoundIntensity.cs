namespace Hubris
{
	/// <summary>
	/// Intensity for a sound event
	/// </summary>
	public enum SoundIntensity
	{
		// Most sounds should be normal
		NORMAL = 0,

		// Something worth investigating for an Npc 
		NOTEWORTHY,

		// Something that should cause client-side effects or restrict Npc vision/hearing
		DEAFENING,

		
		NUM_VARS
	}
}