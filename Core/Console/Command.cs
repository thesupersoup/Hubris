using UnityEngine;


public class Command
{
    // Command Type (CmdType) enum, of short type for smaller size if conveying over network
    public enum CmdType : short {
        None = 0,

        // Basic movement
        MoveF,
        MoveB,
        MoveL,
        MoveR,
        Jump,
        RunHold,
        RunToggle,
        CrouchHold,
        CrouchToggle,

        // Weapons
        Slot1,
        Slot2,
        Slot3,
        Slot4,

        // Multiplayer
        ChatPublic,
        ChatPrivate,

        // Console and console-only cmds
        Console,
        MapKey,
        Disconnect,
        Quit,
        Num_Cmds }

    // Static array of Commands
    public static Command[] cmdArr = new Command[(int)CmdType.Num_Cmds] {   // Use Num_Cmds to ensure proper array length
        new Command("Null", "none", CmdType.None),

        // Basic movement
        new Command("Move Forward", "movef", CmdType.MoveF),
        new Command("Move Backward", "moveb", CmdType.MoveB),
        new Command("Strafe Left", "movel", CmdType.MoveL),
        new Command("Strafe Right", "mover", CmdType.MoveR),
        new Command("Jump", "jump", CmdType.Jump),
        new Command("Run (hold)", "runhold", CmdType.RunHold),
        new Command("Run (toggle)", "runtoggle", CmdType.RunToggle),
        new Command("Crouch (hold)", "crouchhold", CmdType.CrouchHold),
        new Command("Crouch (toggle)", "crouchtoggle", CmdType.CrouchToggle),

        // Weapons
        new Command("Weapon Slot - Primary", "slot1", CmdType.Slot1),
        new Command("Weapon Slot - Secondary", "slot2", CmdType.Slot2),
        new Command("Weapon Slot - Melee", "slot3", CmdType.Slot3),
        new Command("Weapon Slot - Special", "slot4", CmdType.Slot4),

        // Multiplayer
        new Command("Chat (all)", "chatall", CmdType.ChatPublic),
        new Command("Chat (team)", "chatteam", CmdType.ChatPrivate),

        // Console and console-only cmds
        new Command("Open Console", "console", CmdType.Console),
        new Command("Map Key to Command", "mapkey", CmdType.MapKey),
        new Command("Disconnect from server", "disconnect", CmdType.Disconnect),
        new Command("Quit to desktop", "quit", CmdType.Quit)
    };

    // Command instance variables
    private string _uiName;     // Name to be used in UI and other human-readable contexts
    private string _cmdName;    // Name to be used when invoking the Command in console or config
    private CmdType _type;      // What type of Command is it? See CmdType enum
    private byte _value;        // Value of command, used for settings in the format of (command name) #
    private bool _sign;         // Positive (true) or negative (false), used when detecting key down vs. up

    // Command properties
    public string UIName
    {
        get { return _uiName; }
        protected set { _uiName = value; }
    }

    public string CmdName
    {
        get { return _cmdName; }
        protected set { _cmdName = value; }
    }

    public CmdType Type
    {
        get { return _type; }
        protected set { if (value >= CmdType.None && value < CmdType.Num_Cmds) { _type = value; } else { _type = CmdType.None; } }
    }

    public byte Value
    {
        get { return _value; }
        set { _value = value; } // Let the record show that I briefly reconsidered the name of the instance variable
    }

    public bool Sign
    {
        get { return _sign; }
        protected set { _sign = value; }
    }

    public static Command None
    {
        get { return cmdArr[(int)CmdType.None];  }
        protected set { /* This space intentionally left blank */ }
    }

    public static Command MoveF
    {
        get { return cmdArr[(int)CmdType.MoveF]; }
        protected set { /* This space intentionally left blank */ }
    }

    public static Command MoveB
    {
        get { return cmdArr[(int)CmdType.MoveB]; }
        protected set { /* This space intentionally left blank */ }
    }

    public static Command MoveL
    {
        get { return cmdArr[(int)CmdType.MoveL]; }
        protected set { /* This space intentionally left blank */ }
    }

    public static Command MoveR
    {
        get { return cmdArr[(int)CmdType.MoveR]; }
        protected set { /* This space intentionally left blank */ }
    }

    public static Command Jump
    {
        get { return cmdArr[(int)CmdType.Jump]; }
        protected set { /* This space intentionally left blank */ }
    }

    public static Command RunHold
    {
        get { return cmdArr[(int)CmdType.RunHold]; }
        protected set { /* This space intentionally left blank */ }
    }

    public static Command RunToggle
    {
        get { return cmdArr[(int)CmdType.RunToggle]; }
        protected set { /* This space intentionally left blank */ }
    }

    public static Command CrouchHold
    {
        get { return cmdArr[(int)CmdType.CrouchHold]; }
        protected set { /* This space intentionally left blank */ }
    }

    public static Command CrouchToggle
    {
        get { return cmdArr[(int)CmdType.CrouchToggle]; }
        protected set { /* This space intentionally left blank */ }
    }

    public static Command Slot1
    {
        get { return cmdArr[(int)CmdType.Slot1]; }
        protected set { /* This space intentionally left blank */ }
    }

    public static Command Slot2
    {
        get { return cmdArr[(int)CmdType.Slot2]; }
        protected set { /* This space intentionally left blank */ }
    }

    public static Command Slot3
    {
        get { return cmdArr[(int)CmdType.Slot3]; }
        protected set { /* This space intentionally left blank */ }
    }

    public static Command Slot4
    {
        get { return cmdArr[(int)CmdType.Slot4]; }
        protected set { /* This space intentionally left blank */ }
    }

    public static Command ChatPublic
    {
        get { return cmdArr[(int)CmdType.ChatPublic]; }
        protected set { /* This space intentionally left blank */ }
    }

    public static Command ChatPrivate
    {
        get { return cmdArr[(int)CmdType.ChatPrivate]; }
        protected set { /* This space intentionally left blank */ }
    }

    public static Command Console
    {
        get { return cmdArr[(int)CmdType.Console]; }
        protected set { /* This space intentionally left blank */ }
    }

    public static Command MapKey
    {
        get { return cmdArr[(int)CmdType.MapKey]; }
        protected set { /* This space intentionally left blank */ }
    }

    public static Command Disconnect
    {
        get { return cmdArr[(int)CmdType.Disconnect]; }
        protected set { /* This space intentionally left blank */ }
    }

    public static Command Quit
    {
        get { return cmdArr[(int)CmdType.Quit]; }
        protected set { /* This space intentionally left blank */ }
    }

    public static short Num_Cmds
    {
        get { return (short)CmdType.Num_Cmds; }
        protected set { /* This space intentionally left blank */ }
    }

    // Command methods
    public Command()
    {
        _uiName = "DefaultUIName";
        _cmdName = "DefaultCmdName";
        _type = CmdType.None;
        _value = 0;
        _sign = true;
    }

    public Command(string sUI = "DefaultUIName", string sCmd = "DefaultCmdName", CmdType eType = CmdType.None, byte bValue = 0, bool bSign = true)
    {
        _uiName = sUI;
        _cmdName = sCmd;
        _type = eType;
        _value = bValue;
        _sign = bSign;
    }

    
}
