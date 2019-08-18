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
			vars[(int)VarType.CrossColor] = new Variable( "crosscolor", "Set crosshair color", VarData.INT, VarType.CrossColor, SettingsHub.DEF_CROSSCOLOR );
			vars[(int)VarType.CrossDot] = new Variable( "crossdot", "Enable/disable crosshair dot", VarData.BOOL, VarType.CrossDot, SettingsHub.DEF_CROSSDOT );
			vars[(int)VarType.HudColor] = new Variable( "hudcolor", "Set HUD color", VarData.INT, VarType.HudColor, SettingsHub.DEF_HUDCOLOR );
			vars[(int)VarType.AlwaysRun] = new Variable( "alwaysrun", "Enable/disable run by default", VarData.BOOL, VarType.AlwaysRun, SettingsHub.DEF_ALWAYSRUN );

			// Game settings
			vars[(int)VarType.MasterVol] = new Variable( "volume", "Set the master volume", VarData.FLOAT, VarType.MasterVol, SettingsHub.DEF_MASTERVOL );
			vars[(int)VarType.MenuVol] = new Variable( "menuvolume", "Set the main menu music volume", VarData.FLOAT, VarType.MenuVol, SettingsHub.DEF_MENUVOL );
			vars[(int)VarType.MusicVol] = new Variable( "musicvolume", "Set the ingame music volume", VarData.FLOAT, VarType.MusicVol, SettingsHub.DEF_MUSICVOL );

			// Dev settings
			vars[(int)VarType.Useaccel] = new Variable( "useaccel", "Enable acceleration values for player movement", VarData.BOOL, VarType.Useaccel, SettingsHub.DEF_USEACCEL );
			vars[(int)VarType.Debug] = new Variable( "debug", "Enable debug mode and verbose console logging", VarData.BOOL, VarType.Debug, SettingsHub.DEF_DEBUG );
			vars[(int)VarType.Invis] = new Variable( "invis", "Set player invisible to AI", VarData.BOOL, VarType.Invis, SettingsHub.DEF_INVIS );

			// ??
			vars[(int)VarType.Cheats] = new Variable( "cheats", "Enable restricted commands", VarData.BOOL, VarType.Cheats, false );

			return vars;
		}
	}
}
