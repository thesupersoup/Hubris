using UnityEngine;
using System.Collections.Generic;
using System;

namespace Hubris
{
    public class KeyMap
    {
        // Static array of KeyBinds, set elsewhere 
        public static List<KeyBind> _kbList;

        // KeyMap properties
        public static List<KeyBind> Binds
        {
            get { return _kbList; }
        }

        // KeyMap methods
        public KeyMap()
        {
            _kbList = new List<KeyBind>();
            SetDefaults();
        }

        public static void SetDefaults()
        {
            _kbList.Clear();

            // General
            _kbList.Add(new KeyBind(KeyCode.None, Command.None));
            _kbList.Add(new KeyBind(KeyCode.Return, Command.Submit));
            _kbList.Add(new KeyBind(KeyCode.Mouse0, Command.InteractA));
            _kbList.Add(new KeyBind(KeyCode.Mouse1, Command.InteractB));

            // Basic movement
            _kbList.Add(new KeyBind(KeyCode.W, Command.MoveF));
            _kbList.Add(new KeyBind(KeyCode.S, Command.MoveB));
            _kbList.Add(new KeyBind(KeyCode.A, Command.MoveL));
            _kbList.Add(new KeyBind(KeyCode.D, Command.MoveR));
            _kbList.Add(new KeyBind(KeyCode.Q, Command.RotLeft));
            _kbList.Add(new KeyBind(KeyCode.E, Command.RotRight));
            _kbList.Add(new KeyBind(KeyCode.Space, Command.Jump));
            _kbList.Add(new KeyBind(KeyCode.LeftShift, Command.RunHold));
            _kbList.Add(new KeyBind(KeyCode.LeftControl, Command.CrouchHold));

            // Weapons
            _kbList.Add(new KeyBind(KeyCode.Alpha1, Command.Slot1));
            _kbList.Add(new KeyBind(KeyCode.Alpha2, Command.Slot2));
            _kbList.Add(new KeyBind(KeyCode.Alpha3, Command.Slot3));
            _kbList.Add(new KeyBind(KeyCode.Alpha4, Command.Slot4));

            // Multiplayer
            _kbList.Add(new KeyBind(KeyCode.V, Command.ChatPublic));
            _kbList.Add(new KeyBind(KeyCode.B, Command.ChatPrivate));

            // Console and console-only cmds
            _kbList.Add(new KeyBind(KeyCode.BackQuote, Command.Console)); // KeyCode.BackQuote is the ~/` (Tilde) key

            // FreeLook specific
            _kbList.Add(new KeyBind(KeyCode.LeftArrow, Command.RotLeft));
            _kbList.Add(new KeyBind(KeyCode.RightArrow, Command.RotRight));

            _kbList.Sort(); // Sorted in case we need to perform a binary search or some such eventually. KeyBind.CompareTo checks the KeyCode.
        }

        public static Command CheckKeyCmd(KeyCode kcKey)
        {
            KeyBind bind = _kbList.Find(x => x.Key == kcKey);   // Find the first (only) instance of KeyBind x such that x.Key is equal to kcKey

            if (bind != null)
            {
                return bind.Cmd;
            }
            else
            {
                return Command.None;
            }
        }
    }
}