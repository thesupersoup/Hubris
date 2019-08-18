using UnityEngine;
using static Hubris.Variable;

namespace Hubris
{
	/// <summary>
	/// Used to define and store settings as variables, and provide an interfact for modification
	/// that relays changes to the appropriate objects
	/// </summary>
	public class SettingsHub
	{
		#region Constants
		///--------------------------------------------------------------------
		/// Settings defaults as constants
		/// Defaults for variables should have one-to-one parity with the 
		/// static Variable array
		///--------------------------------------------------------------------

		public const int DEF_CROSSCOLOR = 0, DEF_HUDCOLOR = 1,
							MIN_STRING_SIZE = 4;

		public const float DEF_SENS = 1.0f, DEF_MASTERVOL = 1.0f, DEF_MENUVOL = 1.0f, 
							DEF_MUSICVOL = 1.0f;
		public const bool DEF_MSMOOTH = false, DEF_USEACCEL = true, DEF_DEBUG = false,
							DEF_INVIS = false, DEF_CROSSDOT = false, DEF_ALWAYSRUN = false;
		#endregion Constants
		
		///---------------------------------------------------------------------
		/// Array of Variables initialized in VariableDef class.
		/// Should have one-to-one parity with VarType enum.
		///--------------------------------------------------------------------

		private Variable[] varArr = VariableDef.GetVarArr();

		public int VarArrLength => varArr?.Length ?? 0;

		#region QuickVariables
		///---------------------------------------------------------------------
		/// Specific Variable properties
		///---------------------------------------------------------------------

		public Variable None => varArr[(int)VarType.None];
		public Variable Sens => varArr[(int)VarType.Sens];
		public Variable MSmooth => varArr[(int)VarType.MSmooth];
		public Variable CrossColor => varArr[(int)VarType.CrossColor];
		public Variable CrossDot => varArr[(int)VarType.CrossDot];
		public Variable HudColor => varArr[(int)VarType.HudColor];
		public Variable AlwaysRun => varArr[(int)VarType.AlwaysRun];
		public Variable Volume => varArr[(int)VarType.MasterVol];
		public Variable MenuVolume => varArr[(int)VarType.MenuVol];
		public Variable MusicVolume => varArr[(int)VarType.MusicVol];
		public Variable Useaccel => varArr[(int)VarType.Useaccel];
		public Variable Debug => varArr[(int)VarType.Debug];
		public Variable Invis => varArr[(int)VarType.Invis];
		public Variable Cheats => varArr[(int)VarType.Cheats];

		#endregion QuickVariables


		public Variable GetVariable( int index )
		{
			if ( index < 0 || index >= varArr.Length )
			{
				LocalConsole.Instance.LogError( "HubrisSettings GetVariable(): Invalid index requested, returning Variable.None" );
				return varArr[(int)VarType.None];
			}

			return varArr[index];
		}

		public Variable GetVariable( VarType type )
		{
			int index = (int)type;
			if ( index >= 0 && index < varArr.Length )
				return varArr[index];
			else
			{
				LocalConsole.Instance.LogError( "HubrisSettings GetVariable(): Invalid index requested, returning Variable.None" );
				return varArr[(int)VarType.None];
			}
		}

		/// <summary>
		/// Searches for a variable by name (string param), non-case-sensitive
		/// </summary>
		public Variable GetVarByName( string nName )
		{
			Variable varObj = varArr[(int)VarType.None];
			char[] nameArr = nName.ToCharArray();
			string varTest = "";

			// Test up to the first four characters in standardized lower case
			for ( int i = 0; i < MIN_STRING_SIZE; i++ )
			{
				if ( i >= nameArr.Length )
				{
					varTest += " ";
					continue;
				}

				varTest += char.ToLower( nameArr[i] );
			}

			for (int i = 0; i < varArr.Length; i++)
			{
				if ( varArr[i].Name.Substring( 0, varTest.Length ).Equals( varTest ) )
				{
					varObj = varArr[i];
					break;  // Found it, don't need to keep searching
				}
			}

			return varObj;
		}

		/// <summary>
		/// Pushes changes to a variable into the array; UpdateDirtyVar() must then be called manually
		/// </summary>
		public void PushChanges( VarType nType, object nData )
		{
			int index = (int) nType;
			if( index >= 0 && index < varArr.Length )
			{
				varArr[index].Data = nData;
			}
		}

		/// <summary>
		/// Checks if the variable specified by the VarType param is dirty, and if so, sends the new value
		/// </summary>
		public bool UpdateDirtyVar( VarType nType, bool verbose = true )
		{
			if ( nType == VarType.None )  // Let's not waste our time
				return false;

			int index = (int)nType;
			bool success = true;

			if ( varArr[index].Dirty )
			{
				// VarType determines where the new value is sent
				switch ( nType )
				{
					case VarType.Sens:
						if( HubrisPlayer.Instance != null )
							HubrisPlayer.Instance.Sensitivity = (float)varArr[index].Data;
						break;
					case VarType.MSmooth:
						if ( HubrisPlayer.Instance != null )
							HubrisPlayer.Instance.MSmooth = (bool)varArr[index].Data;
						break;
					case VarType.CrossColor:
						if ( UIManager.Instance != null )
							UIManager.Instance.SetCrosshairColor( (int)varArr[index].Data );
						break;
					case VarType.CrossDot:
						if ( UIManager.Instance != null )
							UIManager.Instance.SetCrosshairDot( (bool)varArr[index].Data );
						break;
					case VarType.HudColor:
						if ( UIManager.Instance != null )
							UIManager.Instance.SetHudColor( (int)varArr[index].Data );
						break;
					case VarType.AlwaysRun:
						if ( HubrisPlayer.Instance != null )
							HubrisPlayer.Instance.AlwaysRun = (bool)varArr[index].Data;
						break;
					case VarType.MasterVol:
					case VarType.MenuVol:
					case VarType.MusicVol:
						// Automatically handled, nothing to do
						break;
					case VarType.Useaccel:
						if ( HubrisPlayer.Instance != null )
							HubrisPlayer.Instance.Movement.SetUseAccel( (bool)varArr[index].Data );
						break;
					case VarType.Debug:
						HubrisCore.Instance.DebugToggle();
						break;
					case VarType.Invis:
						if ( HubrisPlayer.Instance != null )
							HubrisPlayer.Instance.Stats.SetInvisible( (bool)varArr[(int)VarType.Invis].Data );
						break;
					case VarType.Cheats:
						// Automatically handled, nothing to do
						break;
					default:
						if( (int)nType < 0 || nType >= VarType.Num_Vars )
							success = false;
						break;
				}

				if ( success )
				{
					if( verbose )
						LocalConsole.Instance.Log( varArr[index].Name + " : " + varArr[index].Data );
					varArr[index].CleanVar();
				}
				else
					LocalConsole.Instance.LogWarning( "HubrisSettings UpdateDirtyVar(): No corresponding behavior for dirty variable at array index " + index );
			}
			else
			{
				success = false;
				LocalConsole.Instance.LogWarning( "HubrisSettings UpdateDirtyVar(): Variable at array index " + index + " was not dirty" );
			}

			return success;
		}

		/// <summary>
		/// Updates all dirty variables in the static array
		/// </summary>
		public void UpdateAllDirtyVars( bool verbose = true )
		{
			for (int i = 0; i < varArr.Length; i++)
			{
				UpdateDirtyVar((VarType)i, verbose );
			}
		}
	}
}
