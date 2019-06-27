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
		/// 
		/// Command Type (CmdType) enum
		/// Should have one-to-one parity with the static Command array
		/// 
		///--------------------------------------------------------------------

		public enum CmdType
		{
			// General commands
			None = 0,
			Submit,
			Interact0,
			Interact1,

			// Basic movement commands
			MoveF,
			MoveB,
			MoveL,
			MoveR,
			Jump,
			RunHold,
			RunToggle,
			CrouchHold,
			CrouchToggle,

			// Weapons commands
			Slot1,
			Slot2,
			Slot3,
			Slot4,

			// Multiplayer commands
			ChatPublic,
			ChatPrivate,

			// Console and console-only cmds
			Console,
			MapKey,
			Disconnect,
			Quit,
			ConClear,
			SetVar,
			PrevCmd,
			NextCmd,

			// FreeLook commands
			RotLeft,
			RotRight,

			// Network commands
			Net_Send,
			Net_Send_Udp,
			Net_Send_Tcp,
			Net_Connect,
			Net_Disconnect,

			// Developer commands
			Version,
			Net_Info,

			Num_Cmds    // Keep at the end for handy enum length hack
		}

		#region CommandArray
		///--------------------------------------------------------------------
		///
		/// Static array of Commands, individual commands initialized in 
		/// InitCmds(). Should have one-to-one parity with the CmdType enum.
		/// 
		///--------------------------------------------------------------------

		private static Command[] cmdArr = InitCmds();

		[ExecuteInEditMode]
		private static Command[] InitCmds()
		{
			Command[] cmds = new Command[(int)CmdType.Num_Cmds]; // Use Num_Cmds to ensure proper array length

			// General commands
			cmds[(int)CmdType.None] = new Command("Null", "none", CmdType.None);
			cmds[(int)CmdType.Submit] = new Command("Submit", "submit", CmdType.Submit, false);
			cmds[(int)CmdType.Interact0] = new Command("Interact0", "inter0", CmdType.Interact0, false);
			cmds[(int)CmdType.Interact1] = new Command("Interact1", "inter1", CmdType.Interact1, false);

			// Basic movement commands
			cmds[(int)CmdType.MoveF] = new Command("Move Forward", "movef", CmdType.MoveF, true);
			cmds[(int)CmdType.MoveB] = new Command("Move Backward", "moveb", CmdType.MoveB, true);
			cmds[(int)CmdType.MoveL] = new Command("Strafe Left", "movel", CmdType.MoveL, true);
			cmds[(int)CmdType.MoveR] = new Command("Strafe Right", "mover", CmdType.MoveR, true);
			cmds[(int)CmdType.Jump] = new Command("Jump", "jump", CmdType.Jump, false);
			cmds[(int)CmdType.RunHold] = new Command("Run (hold)", "runhold", CmdType.RunHold, true);
			cmds[(int)CmdType.RunToggle] = new Command("Run (toggle)", "runtoggle", CmdType.RunToggle);
			cmds[(int)CmdType.CrouchHold] = new Command("Crouch (hold)", "crouchhold", CmdType.CrouchHold);
			cmds[(int)CmdType.CrouchToggle] = new Command("Crouch (toggle)", "crouchtoggle", CmdType.CrouchToggle);

			// Weapons commands
			cmds[(int)CmdType.Slot1] = new Command("Weapon Slot - Primary", "slot1", CmdType.Slot1, false);
			cmds[(int)CmdType.Slot2] = new Command("Weapon Slot - Secondary", "slot2", CmdType.Slot2, false);
			cmds[(int)CmdType.Slot3] = new Command("Weapon Slot - Melee", "slot3", CmdType.Slot3, false);
			cmds[(int)CmdType.Slot4] = new Command("Weapon Slot - Special", "slot4", CmdType.Slot4, false);

			// Multiplayer commands
			cmds[(int)CmdType.ChatPublic] = new Command("Chat (all)", "chatall", CmdType.ChatPublic, false);
			cmds[(int)CmdType.ChatPrivate] = new Command("Chat (team)", "chatteam", CmdType.ChatPrivate, false);

			// Console and console-only cmds
			cmds[(int)CmdType.Console] = new Command("Open Console", "console", CmdType.Console, false);
			cmds[(int)CmdType.MapKey] = new Command("Map Key to Command", "mapkey", CmdType.MapKey);
			cmds[(int)CmdType.Disconnect] = new Command("Disconnect from server", "disconnect", CmdType.Disconnect, false);
			cmds[(int)CmdType.Quit] = new Command("Quit to desktop", "quit", CmdType.Quit, false);
			cmds[(int)CmdType.ConClear] = new Command("Clear Console text", "clr", CmdType.ConClear, false);
			cmds[(int)CmdType.SetVar] = new Command("Set specified variable to value", "setvar", CmdType.SetVar, false);
			cmds[(int)CmdType.PrevCmd] = new Command("See previous command", "prevcmd", CmdType.PrevCmd, false);
			cmds[(int)CmdType.NextCmd] = new Command("See next command", "nextcmd", CmdType.NextCmd, false);

			// FreeLook commands
			cmds[(int)CmdType.RotLeft] = new Command("Rotate Camera Left", "rotleft", CmdType.RotLeft, true);
			cmds[(int)CmdType.RotRight] = new Command("Rotate Camera Right", "rotright", CmdType.RotRight, true);

			// Network commands
			cmds[(int)CmdType.Net_Send] = new Command("Send data to server via default protocol", "net_send", CmdType.Net_Send, false);
			cmds[(int)CmdType.Net_Send_Udp] = new Command("Send data to server via UDP", "net_send_udp", CmdType.Net_Send_Udp, false);
			cmds[(int)CmdType.Net_Send_Tcp] = new Command("Send data to server via TCP", "net_send_tcp", CmdType.Net_Send_Tcp, false);
			cmds[(int)CmdType.Net_Connect] = new Command("Connect to the specified IP", "net_connect", CmdType.Net_Connect, false);
			cmds[(int)CmdType.Net_Disconnect] = new Command("Disconnect active connection", "net_disconnect", CmdType.Net_Disconnect, false);

			// Developer commands
			cmds[(int)CmdType.Version] = new Command("Display version info", "version", CmdType.Version, false);
			cmds[(int)CmdType.Net_Info] = new Command("Display networking info", "net_info", CmdType.Net_Info, false);

			// SetVar commands

			return cmds;
		}
		#endregion CommandArray

		///---------------------------------------------------------------------
		///
		/// Command Instance variables
		/// 
		///--------------------------------------------------------------------- 

		private string _uiName;     // Name to be used in UI and other human-readable contexts
		private string _cmdName;    // Name to be used when invoking the Command in console or config. Should always be lowercase
		private CmdType _type;      // What type of Command is it? See CmdType enum
		private bool _cont;         // Continuous key (true), or only on keypress (false)?


		///---------------------------------------------------------------------
		///
		/// Command properties
		/// 
		///---------------------------------------------------------------------
        
		public string UIName
		{
			get { return _uiName; }
			private set { _uiName = value; }
		}

		public string CmdName   // CmdName should always be lowercase
		{
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

		private Command()
		{
			_uiName = "DefaultUIName";
			_cmdName = "DefaultCmdName".ToLower();
			_type = CmdType.None;
			_cont = true;
		}

		private Command(string sUI = "DefaultUIName", string sCmd = "DefaultCmdName", CmdType eType = CmdType.None, bool bCont = true)
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
