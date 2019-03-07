using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Hubris
{
    public class Command
    {
        // Command Type (CmdType) enum, of short type for smaller size if conveying over network
        // Should have one-to-one parity with the Commands in the Command[] below and in the KeyMap
        public enum CmdType : short
        {
            // General commands
            None = 0,
            Submit,
            InteractA,
            InteractB,

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

            // FreeLook commands
            RotLeft,
            RotRight,

            // Network testing commands
            Net_Send,

            // Developer commands
            Version,
            Debug,
            Net_Info,

            Num_Cmds    // Keep at the end for handy enum length hack
        }

        // Static array of Commands, individual commands initialized in InitCmds()
        // Should have one-to-one parity with the CmdType enum above
        [SerializeField]
        public static Command[] cmdArr = InitCmds();

        [ExecuteInEditMode]
        private static Command[] InitCmds()
        {
            Command[] cmds = new Command[(int)CmdType.Num_Cmds]; // Use Num_Cmds to ensure proper array length

            // General commands
            cmds[(int)CmdType.None] = new Command("Null", "none", CmdType.None);
            cmds[(int)CmdType.Submit] = new Command("Submit", "submit", CmdType.Submit, null, false);
            cmds[(int)CmdType.InteractA] = new Command("Select", "intera", CmdType.InteractA, null, false);
            cmds[(int)CmdType.InteractB] = new Command("Deselect", "interb", CmdType.InteractB, null, false);

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
            cmds[(int)CmdType.ConClear] = new Command("Clear Console text", "clr", CmdType.ConClear, null, false);

            // FreeLook commands
            cmds[(int)CmdType.RotLeft] = new Command("Rotate Camera Left", "rotleft", CmdType.RotLeft, null, true);
            cmds[(int)CmdType.RotRight] = new Command("Rotate Camera Right", "rotright", CmdType.RotRight, null, true);

            // Network testing commands
            cmds[(int)CmdType.Net_Send] = new Command("Send data to server", "net_send", CmdType.Net_Send, null, false);

            // Developer commands
            cmds[(int)CmdType.Version] = new Command("Display version info", "version", CmdType.Version, null, false);
            cmds[(int)CmdType.Debug] = new Command("Debug mode toggle", "debug", CmdType.Debug, null, false);
            cmds[(int)CmdType.Net_Info] = new Command("Display networking info", "net_info", CmdType.Net_Info, null, false);

            return cmds;
        }

        // Command instance variables
        private string _uiName;     // Name to be used in UI and other human-readable contexts
        private string _cmdName;    // Name to be used when invoking the Command in console or config. Should always be lowercase
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
            protected set { _data = value; } 
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
        }

        public static Command Submit
        {
            get { return cmdArr[(int)CmdType.Submit]; }
        }

        public static Command InteractA
        {
            get { return cmdArr[(int)CmdType.InteractA]; }
        }

        public static Command InteractB
        {
            get { return cmdArr[(int)CmdType.InteractB]; }
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

        public static Command Version
        {
            get { return cmdArr[(int)CmdType.Version]; }
        }

        public static Command Debug
        {
            get { return cmdArr[(int)CmdType.Debug]; }
        }

        public static Command Net_Info
        {
            get { return cmdArr[(int)CmdType.Net_Info]; }
        }

        public static short Num_Cmds
        {
            get { return (short)CmdType.Num_Cmds; }
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
            _cont = bSign;
        }

        public void ClearData()
        {
            _data = null;
        }

        public static Command CheckCmdName(string nName)
        {
            Command cmd = Command.None;
            string cmdLower = nName.ToLower();

            for (int i = 0; i < cmdArr.Length; i++)
            {
                if (cmdArr[i].CmdName == cmdLower)
                {
                    cmd = cmdArr[i];
                    break;  // Found it, don't need to keep searching
                }
            }

            return cmd;
        }
    }
}
