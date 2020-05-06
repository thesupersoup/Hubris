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
		private CmdType _type;      // What type of Command is it? See CmdType enum
		private bool _cont;			// Continuous key (true), or only on keypress (false)?
		private bool _sendBoth;		// For non-continuous commands, send on both keydown and keyup (true) or just keydown (false)


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

		public bool SendBoth
		{
			get { return _sendBoth; }
			private set { _sendBoth = value; }
		}

		#region Static Commands
		///---------------------------------------------------------------------
		/// 
		/// Static Command methods for fetching commands by index, type, or 
		/// specific properties for each command
		/// 
		///---------------------------------------------------------------------

		public static Command None => cmdArr[(int)CmdType.None];

		public static Command Submit => cmdArr[(int)CmdType.Submit];

		public static Command Escape => cmdArr[(int)CmdType.Escape];

		public static Command Interact0 => cmdArr[(int)CmdType.Interact0];

		public static Command Interact1 => cmdArr[(int)CmdType.Interact1];

		public static Command Interact2 => cmdArr[(int)CmdType.Interact2];

		public static Command Interact3 => cmdArr[(int)CmdType.Interact3];

		public static Command Info => cmdArr[(int)CmdType.Info];

		public static Command MoveF => cmdArr[(int)CmdType.MoveF];

		public static Command MoveB => cmdArr[(int)CmdType.MoveB];

		public static Command MoveL => cmdArr[(int)CmdType.MoveL];

		public static Command MoveR => cmdArr[(int)CmdType.MoveR];

		public static Command Jump => cmdArr[(int)CmdType.Jump];

		public static Command RunHold => cmdArr[(int)CmdType.RunHold];

		public static Command RunToggle => cmdArr[(int)CmdType.RunToggle];

		public static Command CrouchHold => cmdArr[(int)CmdType.CrouchHold];

		public static Command CrouchToggle => cmdArr[(int)CmdType.CrouchToggle];

		public static Command Slot1 => cmdArr[(int)CmdType.Slot1];

		public static Command Slot2 => cmdArr[(int)CmdType.Slot2];

		public static Command Slot3 => cmdArr[(int)CmdType.Slot3];

		public static Command Slot4 => cmdArr[(int)CmdType.Slot4];

		public static Command NextSlot => cmdArr[(int)CmdType.NextSlot];

		public static Command PrevSlot => cmdArr[(int)CmdType.PrevSlot];

		public static Command ChatPublic => cmdArr[(int)CmdType.ChatPublic];

		public static Command ChatPrivate => cmdArr[(int)CmdType.ChatPrivate]; 

		public static Command Console => cmdArr[(int)CmdType.Console];

		public static Command MapKey => cmdArr[(int)CmdType.MapKey];

		public static Command Disconnect => cmdArr[(int)CmdType.Disconnect];

		public static Command Quit => cmdArr[(int)CmdType.Quit];

		public static Command ConClear => cmdArr[(int)CmdType.ConClear];

		public static Command SetVar => cmdArr[(int)CmdType.SetVar];

		public static Command PrevCmd => cmdArr[(int)CmdType.PrevCmd];

		public static Command NextCmd => cmdArr[(int)CmdType.NextCmd];

		public static Command RotLeft => cmdArr[(int)CmdType.RotLeft];

		public static Command RotRight => cmdArr[(int)CmdType.RotRight];

		public static Command Net_Send => cmdArr[(int)CmdType.Net_Send];

		public static Command Net_Send_Udp => cmdArr[(int)CmdType.Net_Send_Udp];

		public static Command Net_Send_Tcp => cmdArr[(int)CmdType.Net_Send_Tcp];

		public static Command Net_Connect => cmdArr[(int)CmdType.Net_Connect]; 

		public static Command Version => cmdArr[(int)CmdType.Version];

		public static Command Net_Info => cmdArr[(int)CmdType.Net_Info];

		public static Command Kill => cmdArr[(int)CmdType.Kill];

		public static Command Give => cmdArr[(int)CmdType.Give];

		public static Command Spawn => cmdArr[(int)CmdType.Spawn];

		public static int Num_Cmds => (int)CmdType.Num_Cmds;

		#endregion Static Commands

		///---------------------------------------------------------------------
		/// Command methods
		///---------------------------------------------------------------------

		internal Command()
		{
			_uiName = "DefaultUIName";
			_cmdName = "DefaultCmdName".ToLower();
			_type = CmdType.None;
			_cont = true;
			_sendBoth = false;
		}

		internal Command ( string sUI = "DefaultUIName", string sCmd = "DefaultCmdName", CmdType eType = CmdType.None, bool bCont = true, bool bBoth = false)
		{
			_uiName = sUI;
			_cmdName = sCmd.ToLower();
			_type = eType;
			_cont = bCont;
			_sendBoth = bBoth;
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
