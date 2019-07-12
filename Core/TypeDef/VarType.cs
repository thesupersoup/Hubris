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

		// Dev settings
		Useaccel,
		Debug,
		Invis,

		Num_Vars	// Keep at the end for handy enum length hack
	}
}
