using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// The backend console which processes Commands placed into the queue by the InputManager and calls the appropriate methods
	/// </summary>
	public class LocalConsole : LogicalEntity
	{
		///--------------------------------------------------------------------
		/// LocalConsole constants
		///--------------------------------------------------------------------

		public const char HELP_CHAR = '?';  // Character which triggers printing of help text
		public const int CMD_LIMIT = 16;    // Max number of commands to be processed per Tick

		///--------------------------------------------------------------------
		/// LocalConsole instance vars
		///--------------------------------------------------------------------

		private bool _ready = false;        // Ready to process commands
		private SettingsHub _settings;
		private CmdQueue _cmdQueue;
		private List<string> _cmdData;
		private HubrisPlayer _pScript;
		private Variable _setVarTemp;       // Temporary holding place for the variable being set with a SetVar command

		///--------------------------------------------------------------------
		/// LocalConsole properties
		///--------------------------------------------------------------------

		public bool Ready
		{
			get { return _ready; }
			set { _ready = value; }
		}

		public SettingsHub Settings
		{
			get { return _settings; }
		}

		public static LocalConsole Instance
		{
			get { return HubrisCore.Instance.Console; }    // Moved LocalConsole instance to Core class
		}

		///--------------------------------------------------------------------
		/// LocalConsole methods
		///--------------------------------------------------------------------

		public override void Init()
		{
			if ( HubrisCore.Instance == null || HubrisCore.Instance.Console != this )
				return;

			SubTick();

			_settings = new SettingsHub();
			_cmdQueue = new CmdQueue();
			_cmdData = new List<string>();
			_setVarTemp = Settings.None;

			Ready = true;

			if ( HubrisPlayer.Instance != null )
			{
				_pScript = HubrisPlayer.Instance;
			}
			else
			{
				if ( HubrisCore.Instance.Debug )
					Log( "LocalConsole OnEnable(): Waiting for Player.Instance...", true );

				_pScript = null;
			}
		}

		private void ProcessInstructions()
		{
			if ( _cmdQueue.HasNodes() )
			{
				for ( int i = 0; i < CMD_LIMIT && _cmdQueue.HasNodes(); i++ )
				{
					Command cmd = _cmdQueue.Dequeue( out string data );

					switch ( cmd.Type )
					{
						case CmdType.Interact0:
							HubrisPlayer.Instance.Interact0();
							break;
						case CmdType.Interact1:
							HubrisPlayer.Instance.Interact1();
							break;
						case CmdType.Slot1:
							HubrisPlayer.Instance.SetActiveSlot( 0 );
							break;
						case CmdType.Slot2:
							HubrisPlayer.Instance.SetActiveSlot( 1 );
							break;
						case CmdType.Slot3:
							HubrisPlayer.Instance.SetActiveSlot( 2 );
							break;
						case CmdType.Slot4:
							HubrisPlayer.Instance.SetActiveSlot( 3 );
							break;
						case CmdType.Submit:
							UIManager.Instance.ConsoleSubmitInput();
							break;
						case CmdType.MoveF:
							HubrisPlayer.Instance.Move( InputManager.Axis.Z, 1.0f );
							break;
						case CmdType.MoveB:
							HubrisPlayer.Instance.Move( InputManager.Axis.Z, -1.0f );
							break;
						case CmdType.MoveL:
							HubrisPlayer.Instance.Move( InputManager.Axis.X, -1.0f );
							break;
						case CmdType.MoveR:
							HubrisPlayer.Instance.Move( InputManager.Axis.X, 1.0f );
							break;
						case CmdType.Jump:  // Only valid for PType FPS
							if ( HubrisPlayer.Instance.PlayerType == HubrisPlayer.PType.FPS )
							{
								HubrisPlayer.Instance.SpecMove( HubrisPlayer.SpecMoveType.JUMP, InputManager.Axis.Y, HubrisPlayer.Instance.Movement.JumpSpd );
							}
							break;
						case CmdType.RunHold:
							HubrisPlayer.Instance.SetSpeedTarget( HubrisPlayer.Instance.Movement.SpeedHigh );
							break;
						case CmdType.Console:
							UIManager.Instance.ConsoleToggle();
							break;
						case CmdType.ConClear:
							UIManager.Instance.ConsoleClear();
							break;
						case CmdType.SetVar:
							if ( data != null )
								ProcessSetVar( data );
							else
								Variable.DisplayVarHelp( Settings.None );
							break;
						case CmdType.PrevCmd:
							UIManager.Instance.CheckPrevCmd();
							break;
						case CmdType.NextCmd:
							UIManager.Instance.CheckNextCmd();
							break;
						case CmdType.RotLeft:
							HubrisPlayer.Instance.Rotate( InputManager.Axis.Y, -1.0f );
							break;
						case CmdType.RotRight:
							HubrisPlayer.Instance.Rotate( InputManager.Axis.Y, 1.0f );
							break;
						case CmdType.Net_Send:
						case CmdType.Net_Send_Tcp:
						case CmdType.Net_Send_Udp:
							bool reliable;

							if ( cmd.Type == CmdType.Net_Send_Udp )
								reliable = false;
							else
								reliable = true;

							if ( data != null )
								SendData( data, reliable );
							else
								Log( "Data to send is null" );
							break;
						case CmdType.Net_Connect:
							if ( data != null )
								ConnectToIp( data );
							else
								Log( "No IP address specified" );
							break;
						case CmdType.Net_Disconnect:
							Disconnect();
							break;
						case CmdType.Version:
							HubrisCore.Instance.VersionPrint();
							break;
						case CmdType.Net_Info:
							HubrisCore.Instance.NetInfoPrint();
							break;
						default:
							break;
					}
				}

				if ( HubrisCore.Instance.Debug )
				{
					int left = _cmdQueue.NumNodes();    // Check if there's anything left in the queue

					if ( left > 0 )
						Debug.Log( "LocalConsole ProcessInstructions(): Left in InsQueue: " + left );
				}
			}
		}

		/// <summary>
		/// Attempts to parse user input into a command or a variable; returns success
		/// </summary>
		public bool ProcessInput( string nIn )
		{
			bool success = false;
			string[] strArr;

			Log( nIn );

			if ( nIn == null || nIn.Length == 0 )
				return false;

			// User requested help
			if ( nIn.ToCharArray()[0] == HELP_CHAR )
			{
				for ( int i = 0; i < Settings.VarArrLength; i++ )
					Variable.DisplayVarHelp( Settings.GetVariable( i ) );

				return true;
			}

			strArr = nIn.Split( new char[] { ' ' }, 2 ); // Split at whitespace, max two strings (Cmd and data)

			if ( UIManager.Instance != null )
			{
				UIManager.Instance.AddInput( nIn );
			}

			if ( strArr != null && strArr.Length > 0 )
			{
				Command temp = Command.GetCmdByName( strArr[0] );
				string data = null;

				if ( temp != Command.None )
				{
					if ( strArr.Length > 1 )
						data = strArr[1];

					success = true;
					AddToQueue( temp, data );
				}
				else
				{
					_setVarTemp = Settings.GetVarByName( strArr[0] );
					if ( _setVarTemp != Settings.None )
					{
						temp = Command.GetCommand( CmdType.SetVar );

						success = true;
						AddToQueue( temp, nIn );
					}
					else
					{
						if ( HubrisCore.Instance.Debug )
							Log( "LocalConsole ProcessInput(): Unrecognized command \'" + strArr[0] + "\'", true );
						else
							Log( "Unrecognized command \'" + strArr[0] + "\'" );
					}
				}
			}

			return success;
		}

		public void ProcessSetVar( string data )
		{
			string[] strArr = null;
			bool change = false;

			if ( data == null || data.Length == 0 )
			{
				Log( "Invalid data" );
				return;
			}

			strArr = data.Split( new char[] { ' ' } );	// Split at whitespace

			if ( strArr.Length >= 2 )		// One string for the variable, one for the value
			{
				if ( strArr[1] == null || strArr[1].Length == 0 || strArr[1].ToCharArray()[0] == HELP_CHAR )
					Variable.DisplayVarHelp( _setVarTemp );
				else
				{
					Settings.PushChanges( _setVarTemp.Type, strArr[1] );
					change = true;
				}
			}
			else if ( strArr.Length == 1 )	// Only the variable provided
			{
				switch ( _setVarTemp.DataType )
				{
					case VarData.BOOL:
						Settings.PushChanges( _setVarTemp.Type, !(bool)_setVarTemp.Data );	// Toggle if boolean
						change = true;
						break;
					default:
						Variable.DisplayVarHelp( _setVarTemp );
						break;
				}
			}
			else
				Log( "Unexpected data length" );

			if ( change )
			{
				if ( !Settings.GetVariable( _setVarTemp.Type ).Dirty )
				{
					Log( $"Invalid value entered for {_setVarTemp.Name}" );
					Variable.DisplayVarHelp( _setVarTemp ); // Assume the player entered the wrong data
				}
				else
					Settings.UpdateDirtyVar( _setVarTemp.Type );
			}

			_setVarTemp = Settings.None;	// Reset temp variable
		}

		public void SendData( string nData, bool reliable )
		{
			if ( reliable )
			{
				if ( HubrisNet.Instance != null )
					HubrisNet.Instance.SendMsgTcp( nData );
			}
			else
			{
				if ( HubrisNet.Instance != null )
					HubrisNet.Instance.SendMsgUdp( nData );
			}
		}

		public void ConnectToIp( string nData )
		{
			if ( HubrisNet.Instance != null )
			{
				HubrisNet.Instance.ConnectToRemote( nData );
			}
		}

		public void Disconnect()
		{
			if ( HubrisNet.Instance != null )
			{
				HubrisNet.Instance.Disconnect();
			}
		}

		public void AddToQueue( Command nAdd, string nData = null )
		{
			if ( Active && Ready )
				_cmdQueue.Enqueue( nAdd, nData );
			else
				Log( "Attempted to add a command into the queue of LocalConsole when inactive/not ready" );
		}

		// FixedTick is called once per fixed time interval with MonoBehaviour.FixedUpdate()
		public override void FixedTick()
		{
			if ( _pScript == null && HubrisPlayer.Instance != null )
			{
				_pScript = HubrisPlayer.Instance;
				Settings.UpdateAllDirtyVars();
				if ( HubrisCore.Instance.Debug )
					Log( "LocalConsole Update(): FPSControl.Player found" );
			}

			if ( Active && Ready )
				ProcessInstructions();
		}

		public void Log( string msg, bool unity = false )
		{
			if ( Active )
			{
				if ( msg != null )
				{
					if ( UIManager.Instance != null )
					{
						UIManager.Instance.AddMsg( msg );
					}

					if ( unity )
						UnityEngine.Debug.Log( msg );
				}
			}
			else
				UnityEngine.Debug.Log( msg );
		}

		public void LogWarning( string msg, bool unity = false )
		{
			if ( Active )
			{
				if ( msg != null )
				{
					if ( UIManager.Instance != null )
					{
						UIManager.Instance.AddMsg( "*WARNING* " + msg );
					}

					if ( unity )
						UnityEngine.Debug.LogWarning( msg );
				}
			}
			else
				UnityEngine.Debug.LogWarning( msg );
		}

		public void LogError( string msg, bool unity = false )
		{
			if ( Active )
			{
				if ( msg != null )
				{
					if ( UIManager.Instance != null )
					{
						UIManager.Instance.AddMsg( "*ERROR* " + msg );
					}

					if ( unity )
						UnityEngine.Debug.LogError( msg );
				}
			}
			else
				UnityEngine.Debug.LogError( msg );
		}

		public override void CleanUp( bool full = true )
		{
			if ( !this._disposed )
			{
				if ( full )
				{
					_act = false;
					_name = null;
				}

				_pScript = null;

				UnsubTick();    // Need to Unsubscribe from Tick Event to prevent errors
				_disposed = true;
			}
		}
	}
}
