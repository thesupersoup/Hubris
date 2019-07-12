using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Hubris
{
	/// <summary>
	/// Represents possible interactions between the player and the game. Mapped to a specific key in KeyMap.
	/// Treat as a static class.
	/// </summary>
	public sealed class Command
	{
		///--------------------------------------------------------------------
		/// Static array of Commands initialized in CommandDef class.
		/// Should have one-to-one parity with the CmdType enum.
		///--------------------------------------------------------------------

		private static Command[] cmdArr = CommandDef.GetCmdArr();

		///---------------------------------------------------------------------
		/// Command Instance variables
		///--------------------------------------------------------------------- 

		private string _uiName;		// Name to be used in UI and other human-readable contexts
		private string _cmdName;	// Name to be used when invoking the Command in console or config. Should always be lowercase
		private CmdType _type;		// What type of Command is it? See CmdType enum
		private bool _cont;			// Continuous key (true), or only on keypress (false)?


		///---------------------------------------------------------------------
		/// Command properties
		///---------------------------------------------------------------------

		public string UIName
		{
			get { return _uiName; }
			private set { _uiName = value; }
		}

		public string CmdName
		{
			// CmdName should always be lowercase
			get { return _cmdName.ToLower(); }
			private set { _cmdName = value.ToLower(); }
		}

		public CmdType Type
		{
			get { return _type; }
			private set { if (value >= CmdType.None && value < CmdType.Num_Cmds) { _type = value; } else { _type = CmdType.None; } }
		}

		public bool Continuous
		{
			get { return _cont; }
			private set { _cont = value; }
		}


		///---------------------------------------------------------------------
		/// 
		/// Static Command methods for fetching commands by index, type, or 
		/// specific properties for each command
		/// 
		///---------------------------------------------------------------------

		public static Command None
		{
			get { return cmdArr[(int)CmdType.None]; }
		}

		public static Command Submit
		{
			get { return cmdArr[(int)CmdType.Submit]; }
		}

		public static Command Interact0
		{
			get { return cmdArr[(int)CmdType.Interact0]; }
		}

		public static Command Interact1
		{
			get { return cmdArr[(int)CmdType.Interact1]; }
		}

		public static Command MoveF
		{
			get { return cmdArr[(int)CmdType.MoveF]; }
		}

		public static Command MoveB
		{
			get { return cmdArr[(int)CmdType.MoveB]; }
		}

		public static Command MoveL
		{
			get { return cmdArr[(int)CmdType.MoveL]; }
		}

		public static Command MoveR
		{
			get { return cmdArr[(int)CmdType.MoveR]; }
		}

		public static Command Jump
		{
			get { return cmdArr[(int)CmdType.Jump]; }
		}

		public static Command RunHold
		{
			get { return cmdArr[(int)CmdType.RunHold]; }
		}

		public static Command RunToggle
		{
			get { return cmdArr[(int)CmdType.RunToggle]; }
		}

		public static Command CrouchHold
		{
			get { return cmdArr[(int)CmdType.CrouchHold]; }
		}

		public static Command CrouchToggle
		{
			get { return cmdArr[(int)CmdType.CrouchToggle]; }
		}

		public static Command Slot1
		{
			get { return cmdArr[(int)CmdType.Slot1]; }
		}

		public static Command Slot2
		{
			get { return cmdArr[(int)CmdType.Slot2]; }
		}

		public static Command Slot3
		{
			get { return cmdArr[(int)CmdType.Slot3]; }
		}

		public static Command Slot4
		{
			get { return cmdArr[(int)CmdType.Slot4]; }
		}

		public static Command ChatPublic
		{
			get { return cmdArr[(int)CmdType.ChatPublic]; }
		}

		public static Command ChatPrivate
		{
			get { return cmdArr[(int)CmdType.ChatPrivate]; }
		}

		public static Command Console
		{
			get { return cmdArr[(int)CmdType.Console]; }
		}

		public static Command MapKey
		{
			get { return cmdArr[(int)CmdType.MapKey]; }
		}

		public static Command Disconnect
		{
			get { return cmdArr[(int)CmdType.Disconnect]; }
		}

		public static Command Quit
		{
			get { return cmdArr[(int)CmdType.Quit]; }
		}

		public static Command ConClear
		{
			get { return cmdArr[(int)CmdType.ConClear]; }
		}

		public static Command SetVar
		{
			get { return cmdArr[(int)CmdType.SetVar]; }
		}

		public static Command PrevCmd
		{
			get { return cmdArr[(int)CmdType.PrevCmd]; }
		}

		public static Command NextCmd
		{
			get { return cmdArr[(int)CmdType.NextCmd]; }
		}

		public static Command RotLeft
		{
			get { return cmdArr[(int)CmdType.RotLeft]; }
		}

		public static Command RotRight
		{
			get { return cmdArr[(int)CmdType.RotRight]; }
		}

		public static Command Net_Send
		{
			get { return cmdArr[(int)CmdType.Net_Send]; }
		}

		public static Command Net_Send_Udp
		{ 
			get { return cmdArr[(int)CmdType.Net_Send_Udp]; }
		}

		public static Command Net_Send_Tcp
		{
			get { return cmdArr[(int)CmdType.Net_Send_Tcp]; }
		}

		public static Command Net_Connect
		{
			get { return cmdArr[(int)CmdType.Net_Connect]; }
		}

		public static Command Version
		{
			get { return cmdArr[(int)CmdType.Version]; }
		}

		public static Command Net_Info
		{
			get { return cmdArr[(int)CmdType.Net_Info]; }
		}

		public static int Num_Cmds
		{
			get { return (int)CmdType.Num_Cmds; }
		}

		internal Command()
		{
			_uiName = "DefaultUIName";
			_cmdName = "DefaultCmdName".ToLower();
			_type = CmdType.None;
			_cont = true;
		}

		internal Command(string sUI = "DefaultUIName", string sCmd = "DefaultCmdName", CmdType eType = CmdType.None, bool bCont = true)
		{
			_uiName = sUI;
			_cmdName = sCmd.ToLower();
			_type = eType;
			_cont = bCont;
		}

		public static Command GetCommand(int index)
		{
			if (index >= 0 && index < cmdArr.Length)
				return cmdArr[index];
			else
			{
				LocalConsole.Instance.LogError("Command GetCommand(): Invalid index requested, returning Command.None");
				return cmdArr[(int)CmdType.None];
			}
		}

		public static Command GetCommand(CmdType type)
		{
			int index = (int)type;
			if (index >= 0 && index < cmdArr.Length)
				return cmdArr[index];
			else
			{
				LocalConsole.Instance.LogError("Command GetCommand(): Invalid index requested, returning Command.None");
				return cmdArr[(int)CmdType.None];
			}
		}

		/// <summary>
		/// Searches for a command by name (string param), non-case-sensitive
		/// </summary>
		public static Command GetCmdByName(string nName)
		{
			Command cmdObj = cmdArr[(int)CmdType.None];
			string cmdLower = nName.ToLower();

			for (int i = 0; i < cmdArr.Length; i++)
			{
				if (cmdArr[i].CmdName.Equals(cmdLower))
				{
					cmdObj = cmdArr[i];
					break;  // Found it, don't need to keep searching
				}
			}

			return cmdObj;
		}
	}
}
