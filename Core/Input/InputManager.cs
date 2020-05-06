using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Handles user input and mapping of input to Commands
	/// </summary>
	public class InputManager
	{
		public enum Axis { X, Y, Z, M_X, M_Y, NUM_AXIS }

		///--------------------------------------------------------------------
		/// InputManager instance vars
		///--------------------------------------------------------------------

		private bool _lite = false;
		private bool _enabled = true;
		private KeyMap _km;
		private bool[] _keyDownArr;

	
		///--------------------------------------------------------------------
		/// InputManager properties
		///--------------------------------------------------------------------

		public bool Enabled => _enabled;

		/// <summary>
		/// Lite mode - minimal input management
		/// </summary>
		public bool Lite
		{
			get { return _lite; }
			protected set { _lite = value; }
		}

		public KeyMap KeyMap
		{
			get { return _km; }
			protected set { _km = value; }
		}

		public static InputManager Instance => HubrisCore.Instance.Input;

		public bool MoveKey { get; protected set; }

		///--------------------------------------------------------------------
		/// InputManager methods
		///--------------------------------------------------------------------

		public void Init( bool lite = false )
		{
			// TODO: Allow KeyMap to accept an existing KeyMap config on init
			_km = new KeyMap();
			_keyDownArr = new bool[KeyMap.KeysInUse.Length];

			// Force lite mode when there's no player instance, or if we're not ingame (in a menu)
			if( HubrisPlayer.Instance == null || !HubrisCore.Instance.Ingame )
			{
				SetLite( true );
				return;
			}

			SetLite( lite );
		}

		/// <summary>
		/// Check if the passed Command is valid for the current player type; return bool
		/// </summary>
		private bool CheckValidForType( Command nCmd )
		{
			if ( HubrisPlayer.Instance == null )
				throw new NullReferenceException( "No player instance found" );

			if ( nCmd == Command.None )
				return false;

			bool valid;

			switch ( HubrisPlayer.Instance.PlayerType )
			{
				case HubrisPlayer.PType.FPS:
					valid = true;
					break;
				case HubrisPlayer.PType.FL:
					valid = true;
					break;
				case HubrisPlayer.PType.RTS:
					switch ( nCmd.Type )
					{
						case CmdType.Jump:
						case CmdType.CrouchHold:
						case CmdType.CrouchToggle:
							valid = false;
							break;
						default:
							valid = true;
							break;
					}
					break;
				default:
					valid = true;
					break;
			}

			return valid;
		}


		/// <summary>
		/// Check if a movement key is active as of the current frame
		/// </summary>
		private void UpdateMoveKey( Command nCmd )
		{
			switch ( nCmd.Type )
			{
				case CmdType.MoveF:
				case CmdType.MoveB:
				case CmdType.MoveL:
				case CmdType.MoveR:
					MoveKey = true;
					break;
			}
		}

		public void SetEnabled( bool enable )
		{
			_enabled = enable;
		}

		public void SetLite( bool lite )
		{
			if ( HubrisCore.Instance.Debug )
				LocalConsole.Instance.Log( "Setting InputManager Lite to " + lite, true );
			Lite = lite;
		}

		private void GetKeyState( Command cmd, bool keyDown, bool keyDownPrev, out InputState state )
		{
			if ( !cmd.Continuous )
			{
				// Non-continuous commands are not sent multiple times
				if ( keyDown && keyDownPrev )
				{
					state = InputState.INVALID;
					return;
				}

				// Update state for non-continuous commands
				if ( keyDown && !keyDownPrev )
				{
					state = InputState.KEY_DOWN;
					return;
				}

				// If the key was released this frame
				if ( !keyDown && keyDownPrev )
				{
					// If the command is set to send both key up and key down
					if ( cmd.SendBoth )
					{
						state = InputState.KEY_UP;
						return;
					}

					state = InputState.INVALID;
					return;
				}

				state = InputState.INVALID;
				return;
			}
			else
			{
				if ( keyDown && keyDownPrev )
					state = InputState.KEY_HOLD;
				else
				{
					// Update state for non-continuous commands
					if ( keyDown && !keyDownPrev )
						state = InputState.KEY_DOWN;
					else
						state = InputState.KEY_UP;
				}
			}
		}

		private void ProcessKeys()
		{
			// No keys pressed this tick
			if ( !Input.anyKey )
			{
				// Check for any keys previously pressed
				for ( int i = 0; i < KeyMap.KeysInUse.Length; i++ )
				{
					if ( _keyDownArr[i] )
					{
						KeyCode kc = KeyMap.KeysInUse[i];
						Command cmd = KeyMap.CheckKeyCmd( kc );
						GetKeyState( cmd, false, _keyDownArr[i], out InputState state );

						_keyDownArr[i] = false;

						if ( state == InputState.INVALID )
							continue;

						if( _lite )
						{
							// Only accept the following specific commands in Lite mode
							if ( cmd == Command.Console || cmd == Command.Escape || cmd == Command.Submit || cmd == Command.PrevCmd || cmd == Command.NextCmd )
								LocalConsole.Instance.AddToQueue( cmd, state );
						}
						else
							LocalConsole.Instance.AddToQueue( cmd, state );
					}
				}
				return;
			}

			// If running in Lite mode
			if ( _lite )
			{
				for ( int i = 0; i < KeyMap.KeysInUse.Length; i++ )
				{
					KeyCode kc = KeyMap.KeysInUse[i];
					bool keyDown = Input.GetKey( kc );

					// Key is not pressed and was not pressed; continue
					if ( !keyDown && !_keyDownArr[i] )
						continue;

					Command cmd = KeyMap.CheckKeyCmd( kc );
					GetKeyState( cmd, keyDown, _keyDownArr[i], out InputState state );

					_keyDownArr[i] = keyDown;

					if ( state == InputState.INVALID )
						continue;

					// Only accept the following specific commands in Lite mode
					if ( cmd == Command.Console || cmd == Command.Escape || cmd == Command.Submit || cmd == Command.PrevCmd || cmd == Command.NextCmd )
						LocalConsole.Instance.AddToQueue( cmd, state );
				}

				return;
			}

			// Temporary mouse scroll behavior, to be checked in non-Lite mode
			float mouseScroll = Input.mouseScrollDelta.y;

			if ( mouseScroll != 0.0f )
			{
				if ( mouseScroll > 0.0f )
					LocalConsole.Instance.AddToQueue( Command.NextSlot, InputState.MWHEEL_UP );
				else
					LocalConsole.Instance.AddToQueue( Command.PrevSlot, InputState.MWHEEL_DOWN );
			}

			// Check which keys were pressed and map to commands
			for ( int i = 0; i < KeyMap.KeysInUse.Length; i++ )
			{
				KeyCode kc = KeyMap.KeysInUse[i];
				bool keyDown = Input.GetKey( kc );

				// Key is not pressed and was not pressed; continue
				if ( !keyDown && !_keyDownArr[i] )
					continue;

				Command cmd = KeyMap.CheckKeyCmd( kc );
				GetKeyState( cmd, keyDown, _keyDownArr[i], out InputState state );

				_keyDownArr[i] = keyDown;

				if ( state == InputState.INVALID )
					continue;

				if ( CheckValidForType( cmd ) )
				{
					UpdateMoveKey( cmd );
					LocalConsole.Instance.AddToQueue( cmd, state );
				}
				else
				{
					if ( HubrisCore.Instance.Debug )
						LocalConsole.Instance.LogWarning( "InputManager Tick(): CheckValidForType(" + cmd.CmdName + ") returned false", true );
				}
			}
		}

		public void FixedUpdate()
		{

		}

		// Process user input on each frame for accuracy
		public void Update()
		{
			// Keep MoveKey = false out here to prevent moving with console open bug
			// Will be checked and set appropriately if InputManager is Ready
			MoveKey = false;

			// If disabled, quick return
			if ( !_enabled )
				return;

			ProcessKeys();
		}

		public void LateUpdate()
		{

		}
	}
}
