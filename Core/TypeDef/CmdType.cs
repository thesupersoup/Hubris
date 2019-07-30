namespace Hubris
{
	///--------------------------------------------------------------------
	/// Command Type (CmdType) enum
	/// Should have one-to-one parity with the static Command array
	///--------------------------------------------------------------------
	public enum CmdType
	{
		// General commands
		None = 0,
		Submit,
		Interact0,
		Interact1,
		Interact2,

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
		NextSlot,
		PrevSlot,

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

		Num_Cmds	// Keep at the end for handy enum length hack
	}
}
