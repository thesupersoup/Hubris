using UnityEngine;
using System.Collections.Generic;
using System;

namespace Hubris
{
	public class KeyMap
	{
		///--------------------------------------------------------------------
		/// Static array of keybinds, set elsewhere
		///--------------------------------------------------------------------

		public static KeyCode[] _kcArr;     // Array of used KeyCodes to speed up KeyCode checks (just check against this arr instead of all KeyCodes)
		public static List<KeyBind> _kbList;

		///--------------------------------------------------------------------
		/// KeyMap properties
		///--------------------------------------------------------------------

		public static KeyCode[] KeysInUse
		{
			get { return _kcArr; }
		}

		public static List<KeyBind> Binds
		{
			get { return _kbList; }
		}

		///--------------------------------------------------------------------
		/// KeyMap methods
		///--------------------------------------------------------------------

		public KeyMap()
		{
			_kcArr = null;
			_kbList = new List<KeyBind>();
			SetDefaults();
			RefreshKeysInUse();    
		}

		public static void RefreshKeysInUse()  // Refresh the array of used KeyCodes and array of indicies
		{
			_kcArr = new KeyCode[_kbList.Count];    // _kcArr should always match the _kbList size

			for(int i = 0; i < _kbList.Count; i++)
			{
				_kcArr[i] = _kbList[i].Key;
			}
		}

		public static void SetDefaults()
		{
			_kbList.Clear();

			// General
			_kbList.Add( new KeyBind( KeyCode.None, Command.None ) );
			_kbList.Add( new KeyBind( KeyCode.Return, Command.Submit ) );
			_kbList.Add( new KeyBind( KeyCode.Mouse0, Command.Interact0 ) );
			_kbList.Add( new KeyBind( KeyCode.Mouse1, Command.Interact1 ) );
			_kbList.Add( new KeyBind( KeyCode.R, Command.Interact2 ) );

			// Basic movement
			_kbList.Add( new KeyBind( KeyCode.W, Command.MoveF ) );
			_kbList.Add( new KeyBind( KeyCode.S, Command.MoveB ) );
			_kbList.Add( new KeyBind( KeyCode.A, Command.MoveL ) );
			_kbList.Add( new KeyBind( KeyCode.D, Command.MoveR ) );
			_kbList.Add( new KeyBind( KeyCode.Q, Command.RotLeft ) );
			_kbList.Add( new KeyBind( KeyCode.E, Command.RotRight ) );
			_kbList.Add( new KeyBind( KeyCode.Space, Command.Jump ) );
			_kbList.Add( new KeyBind( KeyCode.LeftShift, Command.RunHold ) );
			_kbList.Add( new KeyBind( KeyCode.LeftControl, Command.CrouchHold ) );

			// Weapons
			_kbList.Add( new KeyBind( KeyCode.Alpha1, Command.Slot1 ) );
			_kbList.Add( new KeyBind( KeyCode.Alpha2, Command.Slot2 ) );
			_kbList.Add( new KeyBind( KeyCode.Alpha3, Command.Slot3 ) );
			_kbList.Add( new KeyBind( KeyCode.Alpha4, Command.Slot4 ) );
			_kbList.Add( new KeyBind( KeyCode.None, Command.NextSlot ) );	// InputManager maps ScrollUp to Mouse5
			_kbList.Add( new KeyBind( KeyCode.None, Command.PrevSlot ) );	// InputManager maps ScrollDown to Mouse6

			// Multiplayer
			_kbList.Add( new KeyBind( KeyCode.V, Command.ChatPublic ) );
			_kbList.Add( new KeyBind( KeyCode.B, Command.ChatPrivate ) );

			// Console and console-only cmds
			_kbList.Add( new KeyBind( KeyCode.BackQuote, Command.Console ) ); // KeyCode.BackQuote is the ~/` (Tilde) key
			_kbList.Add( new KeyBind( KeyCode.UpArrow, Command.PrevCmd ) );
			_kbList.Add( new KeyBind( KeyCode.DownArrow, Command.NextCmd ) );

			// FreeLook specific
			_kbList.Add( new KeyBind( KeyCode.LeftArrow, Command.RotLeft ) );
			_kbList.Add( new KeyBind( KeyCode.RightArrow, Command.RotRight ) );

			_kbList.Sort(); // Sorted in case we need to perform a binary search or some such eventually. KeyBind.CompareTo compares the KeyCode value.
		}

		public static Command CheckKeyCmd(KeyCode kcKey)
		{
			// Return the Cmd where KeyBind.Key is equal to kcKey
			for(int i = 0; i < _kbList.Count; i++)
			{
				if (_kbList[i].Key == kcKey)
					return _kbList[i].Cmd;
			}

			return Command.None;
		}
	}
}