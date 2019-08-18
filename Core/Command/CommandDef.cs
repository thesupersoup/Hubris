namespace Hubris
{
	/// <summary>
	/// Class for console command definitions
	/// </summary>
	public static class CommandDef
	{
		public static Command[] GetCmdArr()
		{
			Command[] cmds = new Command[(int)CmdType.Num_Cmds]; // Use Num_Cmds to ensure proper array length

			// General commands
			cmds[(int)CmdType.None] = new Command( "Null", "none", CmdType.None );
			cmds[(int)CmdType.Submit] = new Command( "Submit", "submit", CmdType.Submit, false );
			cmds[(int)CmdType.Escape] = new Command( "Escape", "escape", CmdType.Escape, false );
			cmds[(int)CmdType.Interact0] = new Command( "Interact0", "inter0", CmdType.Interact0, true );
			cmds[(int)CmdType.Interact1] = new Command( "Interact1", "inter1", CmdType.Interact1, false );
			cmds[(int)CmdType.Interact2] = new Command( "Interact2", "inter2", CmdType.Interact2, false );
			cmds[(int)CmdType.Interact3] = new Command( "Interact3", "inter3", CmdType.Interact3, false );

			// Basic movement commands
			cmds[(int)CmdType.MoveF] = new Command( "Move Forward", "movef", CmdType.MoveF, true );
			cmds[(int)CmdType.MoveB] = new Command( "Move Backward", "moveb", CmdType.MoveB, true );
			cmds[(int)CmdType.MoveL] = new Command( "Strafe Left", "movel", CmdType.MoveL, true );
			cmds[(int)CmdType.MoveR] = new Command( "Strafe Right", "mover", CmdType.MoveR, true );
			cmds[(int)CmdType.Jump] = new Command( "Jump", "jump", CmdType.Jump, false );
			cmds[(int)CmdType.RunHold] = new Command( "Run (hold)", "runhold", CmdType.RunHold, true );
			cmds[(int)CmdType.RunToggle] = new Command( "Run (toggle)", "runtoggle", CmdType.RunToggle );
			cmds[(int)CmdType.CrouchHold] = new Command( "Crouch (hold)", "crouchhold", CmdType.CrouchHold );
			cmds[(int)CmdType.CrouchToggle] = new Command( "Crouch (toggle)", "crouchtoggle", CmdType.CrouchToggle );

			// Weapons commands
			cmds[(int)CmdType.Slot1] = new Command( "Weapon Slot - Primary", "slot1", CmdType.Slot1, false );
			cmds[(int)CmdType.Slot2] = new Command( "Weapon Slot - Secondary", "slot2", CmdType.Slot2, false );
			cmds[(int)CmdType.Slot3] = new Command( "Weapon Slot - Melee", "slot3", CmdType.Slot3, false );
			cmds[(int)CmdType.Slot4] = new Command( "Weapon Slot - Special", "slot4", CmdType.Slot4, false );
			cmds[(int)CmdType.NextSlot] = new Command( "Weapon Slot - Next", "nextslot", CmdType.NextSlot, false );
			cmds[(int)CmdType.PrevSlot] = new Command( "Weapon Slot - Previous", "prevslot", CmdType.PrevSlot, false );

			// Multiplayer commands
			cmds[(int)CmdType.ChatPublic] = new Command( "Chat (all)", "chatall", CmdType.ChatPublic, false );
			cmds[(int)CmdType.ChatPrivate] = new Command( "Chat (team)", "chatteam", CmdType.ChatPrivate, false );

			// Console and console-only cmds
			cmds[(int)CmdType.Console] = new Command( "Open Console", "console", CmdType.Console, false );
			cmds[(int)CmdType.MapKey] = new Command( "Map Key to Command", "mapkey", CmdType.MapKey );
			cmds[(int)CmdType.Disconnect] = new Command( "Disconnect from server", "disconnect", CmdType.Disconnect, false );
			cmds[(int)CmdType.Quit] = new Command( "Quit to desktop", "quit", CmdType.Quit, false );
			cmds[(int)CmdType.ConClear] = new Command( "Clear Console text", "clr", CmdType.ConClear, false );
			cmds[(int)CmdType.SetVar] = new Command( "Set specified variable to value", "setvar", CmdType.SetVar, false );
			cmds[(int)CmdType.PrevCmd] = new Command( "See previous command", "prevcmd", CmdType.PrevCmd, false );
			cmds[(int)CmdType.NextCmd] = new Command( "See next command", "nextcmd", CmdType.NextCmd, false );

			// FreeLook commands
			cmds[(int)CmdType.RotLeft] = new Command( "Rotate Camera Left", "rotleft", CmdType.RotLeft, true );
			cmds[(int)CmdType.RotRight] = new Command( "Rotate Camera Right", "rotright", CmdType.RotRight, true );

			// Network commands
			cmds[(int)CmdType.Net_Send] = new Command( "Send data to server via default protocol", "net_send", CmdType.Net_Send, false );
			cmds[(int)CmdType.Net_Send_Udp] = new Command( "Send data to server via UDP", "net_send_udp", CmdType.Net_Send_Udp, false );
			cmds[(int)CmdType.Net_Send_Tcp] = new Command( "Send data to server via TCP", "net_send_tcp", CmdType.Net_Send_Tcp, false );
			cmds[(int)CmdType.Net_Connect] = new Command( "Connect to the specified IP", "net_connect", CmdType.Net_Connect, false );
			cmds[(int)CmdType.Net_Disconnect] = new Command( "Disconnect active connection", "net_disconnect", CmdType.Net_Disconnect, false );

			// Developer commands
			cmds[(int)CmdType.Version] = new Command( "Display version info", "version", CmdType.Version, false );
			cmds[(int)CmdType.Net_Info] = new Command( "Display networking info", "net_info", CmdType.Net_Info, false );
			cmds[(int)CmdType.Kill] = new Command( "Kill local player", "kill", CmdType.Kill, false );
			cmds[(int)CmdType.Give] = new Command( "Give item or weapon", "give", CmdType.Give, false );

			// SetVar commands

			return cmds;
		}
	}
}
