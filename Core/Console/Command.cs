using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Hubris
{
    public class Command
    {
        // Command Type (CmdType) enum, of short type for smaller size if conveying over network
        public enum CmdType : short
        {
            None = 0,
            Submit,

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

            // FreeLook commands
            RotLeft,
            RotRight,

            // Network testing commands
            Net_Send,

            // Developer commands
            Version,
            Net_Info,

            Num_Cmds    // Keep at the end for handy enum length hack
        }

        // Static array of Commands
        public static Command[] cmdArr = InitCmds();

        private static Command[] InitCmds()
        {
            Command[] cmds = new Command[(int)CmdType.Num_Cmds]; // Use Num_Cmds to ensure proper array length

            cmds[(int)CmdType.None] = new Command("Null", "none", CmdType.None);
            cmds[(int)CmdType.Submit] = new Command("Submit", "submit", CmdType.Submit, null, false);

            // Basic movement commands
            cmds[(int)CmdType.MoveF] = new Command("Move Forward", "movef", CmdType.MoveF, null, true);
            cmds[(int)CmdType.MoveB] = new Command("Move Backward", "moveb", CmdType.MoveB, null, true);
            cmds[(int)CmdType.MoveL] = new Command("Strafe Left", "movel", CmdType.MoveL, null, true);
            cmds[(int)CmdType.MoveR] = new Command("Strafe Right", "mover", CmdType.MoveR, null, true);
            cmds[(int)CmdType.Jump] = new Command("Jump", "jump", CmdType.Jump, null, false);
            cmds[(int)CmdType.RunHold] = new Command("Run (hold)", "runhold", CmdType.RunHold);
            cmds[(int)CmdType.RunToggle] = new Command("Run (toggle)", "runtoggle", CmdType.RunToggle);
            cmds[(int)CmdType.CrouchHold] = new Command("Crouch (hold)", "crouchhold", CmdType.CrouchHold);
            cmds[(int)CmdType.CrouchToggle] = new Command("Crouch (toggle)", "crouchtoggle", CmdType.CrouchToggle);

            // Weapons commands
            cmds[(int)CmdType.Slot1] = new Command("Weapon Slot - Primary", "slot1", CmdType.Slot1, null, false);
            cmds[(int)CmdType.Slot2] = new Command("Weapon Slot - Secondary", "slot2", CmdType.Slot2, null, false);
            cmds[(int)CmdType.Slot3] = new Command("Weapon Slot - Melee", "slot3", CmdType.Slot3, null, false);
            cmds[(int)CmdType.Slot4] = new Command("Weapon Slot - Special", "slot4", CmdType.Slot4, null, false);

            // Multiplayer commands
            cmds[(int)CmdType.ChatPublic] = new Command("Chat (all)", "chatall", CmdType.ChatPublic, null, false);
            cmds[(int)CmdType.ChatPrivate] = new Command("Chat (team)", "chatteam", CmdType.ChatPrivate, null, false);

            // Console and console-only cmds
            cmds[(int)CmdType.Console] = new Command("Open Console", "console", CmdType.Console, null, false);
            cmds[(int)CmdType.MapKey] = new Command("Map Key to Command", "mapkey", CmdType.MapKey);
            cmds[(int)CmdType.Disconnect] = new Command("Disconnect from server", "disconnect", CmdType.Disconnect, null, false);
            cmds[(int)CmdType.Quit] = new Command("Quit to desktop", "quit", CmdType.Quit, null, false);

            // FreeLook commands
            cmds[(int)CmdType.RotLeft] = new Command("Rotate Camera Left", "rotleft", CmdType.RotLeft, null, true);
            cmds[(int)CmdType.RotRight] = new Command("Rotate Camera Right", "rotright", CmdType.RotRight, null, true);

            // Network testing commands
            cmds[(int)CmdType.Net_Send] = new Command("Send data to server", "net_send", CmdType.Net_Send, null, false);

            // Developer commands
            cmds[(int)CmdType.Version] = new Command("Display version info", "version", CmdType.Version, null, false);
            cmds[(int)CmdType.Net_Info] = new Command("Display networking info", "net_info", CmdType.Net_Info, null, false);

            return cmds;
        }

        // Command instance variables
        private string _uiName;     // Name to be used in UI and other human-readable contexts
        private string _cmdName;    // Name to be used when invoking the Command in console or config
        private CmdType _type;      // What type of Command is it? See CmdType enum
        private string _data;       // Data included with command, used for providing setting values or sending associated data
        private bool _cont;         // Continuous key (true), or only on key down (false)?

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

        public string Data
        {
            get { return _data; }
            protected set { _data = value; } // Let the record show that I have renamed the instance variable
        }

        public bool Continuous
        {
            get { return _cont; }
            protected set { _cont = value; }
        }

        public void SetData(string nData)
        {
            _data = nData;
        }

        public static Command None
        {
            get { return cmdArr[(int)CmdType.None]; }
            protected set { /* This space intentionally left blank */ }
        }

        public static Command Submit
        {
            get { return cmdArr[(int)CmdType.Submit]; }
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

        public static Command RotLeft
        {
            get { return cmdArr[(int)CmdType.RotLeft]; }
            protected set { /* This space intentionally left blank */ }
        }

        public static Command RotRight
        {
            get { return cmdArr[(int)CmdType.RotRight]; }
            protected set { /* This space intentionally left blank */ }
        }

        public static Command Net_Send
        {
            get { return cmdArr[(int)CmdType.Net_Send]; }
            protected set { /* This space intentionally left blank */ }
        }

        public static Command Version
        {
            get { return cmdArr[(int)CmdType.Version]; }
            protected set { /* This space intentionally left blank */ }
        }

        public static Command Net_Info
        {
            get { return cmdArr[(int)CmdType.Net_Info]; }
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
            _data = null;
            _cont = true;
        }

        public Command(string sUI = "DefaultUIName", string sCmd = "DefaultCmdName", CmdType eType = CmdType.None, string sData = null, bool bSign = true)
        {
            _uiName = sUI;
            _cmdName = sCmd;
            _type = eType;
            _data = sData;
            _cont= bSign;
        }

        public void ClearData()
        {
            _data = null;
        }

        public static Command CheckCmdName(string nName)
        {
            Command cmd = Command.None;

            for(int i = 0; i < cmdArr.Length; i++)
            {
                if (cmdArr[i].CmdName == nName)
                {
                    cmd = cmdArr[i];
                    break;  // Found it, don't need to keep searching
                }
            }

            return cmd;
        }
    }
}
