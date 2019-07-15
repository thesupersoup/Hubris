namespace Hubris
{
	/// <summary>
	/// Class for variable definitions
	/// </summary>
	public static class VariableDef
	{
		public static Variable[] GetVarArr()
		{
			Variable[] vars = new Variable[(int)VarType.Num_Vars]; // Use Num_Vars to ensure proper array length

			vars[(int)VarType.None] = new Variable( "none", Variable.NO_HELP, VarData.OBJECT, VarType.None, null );

			// Player settings
			vars[(int)VarType.Sens] = new Variable( "sensitivity", "Mouse sensitivity, both X and Y", VarData.FLOAT, VarType.Sens, SettingsHub.DEF_SENS );
			vars[(int)VarType.MSmooth] = new Variable( "msmooth", "Enable mouse smoothing", VarData.BOOL, VarType.MSmooth, SettingsHub.DEF_MSMOOTH );

			// Dev settings
			vars[(int)VarType.Useaccel] = new Variable( "useaccel", "Enable acceleration values for player movement", VarData.BOOL, VarType.Useaccel, SettingsHub.DEF_USEACCEL );
			vars[(int)VarType.Debug] = new Variable( "debug", "Enable debug mode and verbose console logging", VarData.BOOL, VarType.Debug, SettingsHub.DEF_DEBUG );
			vars[(int)VarType.Invis] = new Variable( "invis", "Set player invisible to AI", VarData.BOOL, VarType.Invis, SettingsHub.DEF_INVIS );

			return vars;
		}
	}
}
