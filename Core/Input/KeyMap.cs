using UnityEngine;
using System.Collections.Generic;

namespace Hubris
{
    public class KeyMap
    {
        // Static array of KeyBinds, set elsewhere 
        public static Dictionary<KeyCode, Command> kbDict;

        // KeyMap properties
        public static Dictionary<KeyCode, Command> Dict
        {
            get { return kbDict; }
            protected set { /* This space intentionally left blank */ }
        }

        // KeyMap methods
        public KeyMap()
        {
            kbDict = new Dictionary<KeyCode, Command>();
            SetDefaults();
        }

        public static void SetDefaults()
        {
            kbDict.Clear();
            kbDict.Add(KeyCode.None, Command.None);

            // Basic movement
            kbDict.Add(KeyCode.W, Command.MoveF);
            kbDict.Add(KeyCode.S, Command.MoveB);
            kbDict.Add(KeyCode.A, Command.MoveL);
            kbDict.Add(KeyCode.D, Command.MoveR);
            kbDict.Add(KeyCode.Space, Command.Jump);
            kbDict.Add(KeyCode.LeftShift, Command.RunHold);
            kbDict.Add(KeyCode.LeftControl, Command.CrouchHold);

            // Weapons
            kbDict.Add(KeyCode.Alpha1, Command.Slot1);
            kbDict.Add(KeyCode.Alpha2, Command.Slot2);
            kbDict.Add(KeyCode.Alpha3, Command.Slot3);
            kbDict.Add(KeyCode.Alpha4, Command.Slot4);

            // Multiplayer
            kbDict.Add(KeyCode.V, Command.ChatPublic);
            kbDict.Add(KeyCode.B, Command.ChatPrivate);

            // Console and console-only cmds
            kbDict.Add(KeyCode.Tilde, Command.Console);

            // FreeLook specific
            kbDict.Add(KeyCode.LeftArrow, Command.RotLeft);
            kbDict.Add(KeyCode.RightArrow, Command.RotRight);
        }

        public static Command CheckKeyCmd(KeyCode kcKey)
        {
            Command cmd;
            if (kbDict.TryGetValue(kcKey, out cmd))
                return cmd;
            else
                return Command.None;
        }
    }
}