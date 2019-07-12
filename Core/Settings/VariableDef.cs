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

			vars[(int)VarType.None] = new Variable( "none", VarData.OBJECT, VarType.None, null );

			// Player settings
			vars[(int)VarType.Sens] = new Variable( "sensitivity", VarData.FLOAT, VarType.Sens, SettingsHub.DEF_SENS );
			vars[(int)VarType.MSmooth] = new Variable( "msmooth", VarData.BOOL, VarType.MSmooth, SettingsHub.DEF_MSMOOTH );

			// Dev settings
			vars[(int)VarType.Useaccel] = new Variable( "useaccel", VarData.BOOL, VarType.Useaccel, SettingsHub.DEF_USEACCEL );
			vars[(int)VarType.Debug] = new Variable( "debug", VarData.BOOL, VarType.Debug, SettingsHub.DEF_DEBUG );
			vars[(int)VarType.Invis] = new Variable( "invis", VarData.BOOL, VarType.Invis, SettingsHub.DEF_INVIS );

			return vars;
		}
	}
}
