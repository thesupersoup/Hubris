namespace Hubris
{
	///--------------------------------------------------------------------
	/// Variable Type (VarType) enum
	/// Should have one-to-one parity with the static Variable array
	///--------------------------------------------------------------------
	public enum VarType
	{
		None = 0,

		// Player settings 
		Sens,
		MSmooth,
		CrossColor,
		CrossDot,
		HudColor,
		AlwaysRun,

		// Game settings
		MasterVol,
		MenuVol,
		MusicVol,

		// Dev settings
		Useaccel,
		Debug,
		Invis,

		// ??
		Cheats,

		Num_Vars	// Keep at the end for handy enum length hack
	}
}
