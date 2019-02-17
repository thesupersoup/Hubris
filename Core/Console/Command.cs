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

            // FreeLook specific
            RotLeft,
            RotRight,

            Num_Cmds    // Keep at the end for handy enum length hack
        }

        // Static array of Commands
        public static Command[] cmdArr = InitCmds();

        private static Command[] InitCmds()
        {
            Command[] cmds = new Command[(int)CmdType.Num_Cmds]; // Use Num_Cmds to ensure proper array length

            cmds[(int)CmdType.None] = new Command("Null", "none", CmdType.None);
            cmds[(int)CmdType.Submit] = new Command("Submit", "submit", CmdType.Submit, 0, false);

            // Basic movement
            cmds[(int)CmdType.MoveF] = new Command("Move Forward", "movef", CmdType.MoveF, 0, true);
            cmds[(int)CmdType.MoveB] = new Command("Move Backward", "moveb", CmdType.MoveB, 0, true);
            cmds[(int)CmdType.MoveL] = new Command("Strafe Left", "movel", CmdType.MoveL, 0, true);
            cmds[(int)CmdType.MoveR] = new Command("Strafe Right", "mover", CmdType.MoveR, 0, true);
            cmds[(int)CmdType.Jump] = new Command("Jump", "jump", CmdType.Jump, 0, false);
            cmds[(int)CmdType.RunHold] = new Command("Run (hold)", "runhold", CmdType.RunHold);
            cmds[(int)CmdType.RunToggle] = new Command("Run (toggle)", "runtoggle", CmdType.RunToggle);
            cmds[(int)CmdType.CrouchHold] = new Command("Crouch (hold)", "crouchhold", CmdType.CrouchHold);
            cmds[(int)CmdType.CrouchToggle] = new Command("Crouch (toggle)", "crouchtoggle", CmdType.CrouchToggle);

            // Weapons
            cmds[(int)CmdType.Slot1] = new Command("Weapon Slot - Primary", "slot1", CmdType.Slot1, 0, false);
            cmds[(int)CmdType.Slot2] = new Command("Weapon Slot - Secondary", "slot2", CmdType.Slot2, 0, false);
            cmds[(int)CmdType.Slot3] = new Command("Weapon Slot - Melee", "slot3", CmdType.Slot3, 0, false);
            cmds[(int)CmdType.Slot4] = new Command("Weapon Slot - Special", "slot4", CmdType.Slot4, 0, false);

            // Multiplayer
            cmds[(int)CmdType.ChatPublic] = new Command("Chat (all)", "chatall", CmdType.ChatPublic, 0, false);
            cmds[(int)CmdType.ChatPrivate] = new Command("Chat (team)", "chatteam", CmdType.ChatPrivate, 0, false);

            // Console and console-only cmds
            cmds[(int)CmdType.Console] = new Command("Open Console", "console", CmdType.Console, 0, false);
            cmds[(int)CmdType.MapKey] = new Command("Map Key to Command", "mapkey", CmdType.MapKey);
            cmds[(int)CmdType.Disconnect] = new Command("Disconnect from server", "disconnect", CmdType.Disconnect, 0, false);
            cmds[(int)CmdType.Quit] = new Command("Quit to desktop", "quit", CmdType.Quit, 0, false);

            // FreeLook specific
            cmds[(int)CmdType.RotLeft] = new Command("Rotate Camera Left", "rotleft", CmdType.RotLeft, 0, true);
            cmds[(int)CmdType.RotRight] = new Command("Rotate Camera Right", "rotright", CmdType.RotRight, 0, true);

            return cmds;
        }

        // Command instance variables
        private string _uiName;     // Name to be used in UI and other human-readable contexts
        private string _cmdName;    // Name to be used when invoking the Command in console or config
        private CmdType _type;      // What type of Command is it? See CmdType enum
        private byte _value;        // Value of command, used for settings in the format of (command name) #
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

        public byte Value
        {
            get { return _value; }
            set { _value = value; } // Let the record show that I briefly reconsidered the name of the instance variable
        }

        public bool Continuous
        {
            get { return _cont; }
            protected set { _cont = value; }
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
            _cont = true;
        }

        public Command(string sUI = "DefaultUIName", string sCmd = "DefaultCmdName", CmdType eType = CmdType.None, byte bValue = 0, bool bSign = true)
        {
            _uiName = sUI;
            _cmdName = sCmd;
            _type = eType;
            _value = bValue;
            _cont= bSign;
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
