using System;
using System.Collections;
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
		private KeyMap _km;

		///--------------------------------------------------------------------
		/// InputManager properties
		///--------------------------------------------------------------------

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

		public void SetLite( bool lite )
		{
			if ( HubrisCore.Instance.Debug )
				LocalConsole.Instance.Log( "Setting InputManager Lite to " + lite, true );
			Lite = lite;
		}

		public void Init( bool lite = false )
		{
			KeyMap = new KeyMap();

			// Force lite mode when there's no player instance, or if were not ingame (in a menu)
			if( HubrisPlayer.Instance == null || !HubrisCore.Instance.Ingame )
			{
				SetLite( false );
				return;
			}

			SetLite( lite );
		}

		/// <summary>
		/// Check if the passed Command is valid for a particular player type; return bool
		/// </summary>
		private bool CheckValidForType( Command nCmd )
		{
			if ( HubrisPlayer.Instance == null )
				throw new NullReferenceException( "No player instance found" );

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
						case Command.CmdType.Jump:
						case Command.CmdType.CrouchHold:
						case Command.CmdType.CrouchToggle:
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
		/// Check if a movement key is active as of the current frame; return bool
		/// </summary>
		private bool CheckMoveKey( Command nCmd )
		{
			switch ( nCmd.Type )
			{
				case Command.CmdType.MoveF:
				case Command.CmdType.MoveB:
				case Command.CmdType.MoveL:
				case Command.CmdType.MoveR:
					MoveKey = true;
					break;
			}

			return MoveKey;
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

			if ( !Lite )
			{
				if ( Input.anyKey )
				{
					for ( int i = 0; i < KeyMap.KeysInUse.Length; i++ )
					{
						KeyCode kc = KeyMap.KeysInUse[i];
						Command cmd = KeyMap.CheckKeyCmd( kc );

						if ( Input.GetKey( kc ) )
						{
							if ( cmd != Command.None )
							{
								if ( CheckValidForType( cmd ) )
								{
									CheckMoveKey( cmd );

									if ( cmd.Continuous )
									{
										LocalConsole.Instance.AddToQueue( cmd );
									}
									else
									{
										if ( Input.GetKeyDown( kc ) )
										{
											LocalConsole.Instance.AddToQueue( cmd );
										}
									}
								}
								else
								{
									if ( HubrisCore.Instance.Debug )
										LocalConsole.Instance.LogWarning( "InputManager Tick(): CheckValidForType(" + cmd.CmdName + ") returned false", true );
								}
							}
						}
						else
						{
							if ( Input.GetKeyUp( kc ) )
							{
								// if (Core.Instance.Debug)
								// LocalConsole.Instance.Log("Key " + kc + " up this frame", true);

								if ( cmd.Continuous )
								{
									LocalConsole.Instance.AddToQueue( cmd );
								}
							}
						}
					}
				}
				else    // No keys pressed this tick, so check for any keys released
				{
					for ( int i = 0; i < KeyMap.KeysInUse.Length; i++ )
					{
						KeyCode kc = KeyMap.KeysInUse[i];
						Command cmd = KeyMap.CheckKeyCmd( kc );

						if ( Input.GetKeyUp( kc ) )
						{
							if ( HubrisCore.Instance.Debug )
								LocalConsole.Instance.Log( "Key " + kc + " up on previous frame", true );

							if ( cmd.Continuous )
							{
								LocalConsole.Instance.AddToQueue( cmd );
							}
						}
					}
				}
			}
			else
			{
				// Only accept input for the following specific keys when the InputManager is in Lite mode
				if ( Input.anyKey )
				{
					for ( int i = 0; i < KeyMap.KeysInUse.Length; i++ )
					{
						KeyCode kc = KeyMap.KeysInUse[i];

						if ( Input.GetKey( kc ) )
						{
							Command cmd = KeyMap.CheckKeyCmd( kc );

							if ( cmd == Command.Console || cmd == Command.Submit || cmd == Command.PrevCmd || cmd == Command.NextCmd )
							{
								if ( Input.GetKeyDown( kc ) )
								{
									LocalConsole.Instance.AddToQueue( cmd );  // Key is down, state is true
								}
							}
						}
					}
				}
			}
		}

		public void LateUpdate()
		{

		}
	}
}
