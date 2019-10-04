using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Partial HubrisEntity derived class to control any visible UI elements and events
	/// </summary>
	public partial class UIManager : Entity
	{
		/* Create another partial Hubris.UIManager to add unique methods and implementation */

		// UIManager consts
		private const int CON_LENGTH_MAX = 2056, MSG_COUNT_MAX = 64, CMD_COUNT_MAX = 64;

		// UIManager singleton
		private static UIManager _ui = null;

		private static object _lock = new object();
		private static bool _disposing = false; // Check if we're in the process of disposing this singleton

		public static UIManager Instance
		{
			get
			{
				if (_disposing)
					return null;
				else
				{
					return _ui;
				}
			}

			protected set
			{
				lock (_lock)
				{
					if (_ui == null)
						_ui = value;
				}
			}
		}

		// UIManager instance variables
		[SerializeField]
		protected TextMeshProUGUI _conTxt = null;
		[SerializeField]
		protected TMP_InputField _conIn = null;
		[SerializeField]
		protected GameObject _conCan = null;
		[SerializeField]
		protected TextMeshProUGUI _devSpd = null;
		[SerializeField]
		protected TextMeshProUGUI _devGrnd = null;
		[SerializeField]
		protected GameObject _devCan = null;
		protected string _input = null;
		protected List<Msg> _msgList;
		protected List<string> _inputList;
		protected int _inputIndex = 0;        // Last index of input list accessed
		protected int _msgCounter = 0;
		protected Coroutine _delayCo = null;


		// UIManager properties
		public TextMeshProUGUI ConText
		{
			get { return _conTxt; }
		}

		public TMP_InputField ConInput
		{
			get { return _conIn; }
		}

		public GameObject ConCanvas
		{
			get { return _conCan; }
		}

		public TextMeshProUGUI DevSpd
		{
			get { return _devSpd; }
		}

		public GameObject DevCanvas
		{
			get { return _devCan; }
		}

		public List<Msg> MsgList
		{
			get { return _msgList; }
		}

		public List<string> InputList
		{
			get { return _inputList; }
		}

		public int InputIndex
		{
			get { return _inputIndex; }
			protected set { _inputIndex = value; }
		}

		public int MsgCounter
		{
			get { return _msgCounter; }
			protected set { _msgCounter = value; }
		}

		public bool IsConsoleActive => _conCan.activeSelf;


		// UIManager methods
		public override void OnEnable()
		{
			base.OnEnable();

			if ( Instance == null )
			{
				Instance = this;
			}
			else if ( Instance != this )
			{
				Destroy( this.gameObject );
				return;
			}

			DontDestroyOnLoad( this );

			if( _devCan != null)
				_devCan.SetActive( HubrisCore.Instance.Debug );

			_msgList = new List<Msg>();
			_inputList = new List<string>();
		}

		public virtual void Escape()
		{
			if ( _conCan.activeSelf )
				ConsoleToggle();
		}

		protected bool CheckActive(Behaviour b)
		{
			if (b != null)
				return b.enabled;
			else
				return false;
		}

		public void DevSet(bool nAct)
		{
			if ( _devCan == null )
				return;

			_devCan.SetActive(nAct);
		}

		public void DevSetGrnd( Vector3 vect )
		{
			if ( _devGrnd == null )
				return;

			_devGrnd.text = vect.ToString();
		}

		public void DevToggle()
		{
			if ( _devCan == null )
				return;

			_devCan.SetActive(!_devCan.activeSelf);
		}

		public void ConsoleSet(bool nAct)
		{
			if ( _conCan == null )
				return;

			_conCan.SetActive(nAct);
		}

		public virtual void ConsoleToggle()
		{
			if (_conCan != null)
			{
				bool act = !_conCan.activeSelf;
				_conCan.SetActive(act);

				if ( HubrisPlayer.Instance != null )
				{
					if ( HubrisPlayer.Instance.PlayerType == HubrisPlayer.PType.FPS )
						EnablePlayerInput( !act );
				}
				else
				{
					if ( HubrisCore.Instance.Ingame )
					{
						// Enter Lite mode when the console is up, to prevent unwanted input
						InputManager.Instance.SetLite( act );
					}
				}

				if (_conIn != null)
				{
					_conIn.interactable = act;

					if (act)
					{
						_conIn.ActivateInputField();    
					}
					else
					{
						_conIn.text = "";
						_conIn.DeactivateInputField();
						if(InputList != null)
						{
							InputIndex = InputList.Count;
						}
					}
				}
			}
		}

		/// <summary>
		/// Immediately enable or disable player input
		/// </summary>
		public void EnablePlayerInput( bool enable )
		{
			if ( HubrisPlayer.Instance != null )
				HubrisPlayer.Instance.EnablePlayerInput( enable );
		}

		/// <summary>
		/// Assumes the player input will be enabled; arguement is delay in seconds to enable
		/// </summary>
		public void EnablePlayerInput( float delay )
		{
			EnablePlayerInputDelay( true, delay );
		}

		public void EnablePlayerInputDelay( bool enable, float delay )
		{
			if ( _delayCo != null )
				StopCoroutine( _delayCo );

			_delayCo = StartCoroutine( DelayEnablePlayerInput( enable, delay ) );
		}

		protected virtual IEnumerator DelayEnablePlayerInput( bool enable, float delay )
		{
			yield return new WaitForSeconds( delay );
			EnablePlayerInput( enable );
		}

		public void SetConsoleInput(string nIn)
		{
			if(ConInput != null)
			{
				if (ConCanvas.activeSelf)
				{
					ConInput.text = nIn;
				}
			}
		}

		public void ConsoleSubmitInput()
		{
			if (_conCan != null)
			{
				if (_conIn != null)
				{
					if (LocalConsole.Instance != null)
					{
						if (_conCan.activeSelf)
						{
							_input = _conIn.text;
							LocalConsole.Instance.ProcessInput(_input);
							_conIn.text = "";
							_conIn.ActivateInputField();
						}
						else
						{
							if (HubrisCore.Instance.Debug)
								LocalConsole.Instance.LogWarning("UIManager ConsoleSubmitInput(): Console is not active, cannot submit input from console", true);
						}
					}
					else
					{
						Debug.LogError("UIManager ConsoleSubmitInput(): LocalConsole.Instance is null, can't proceed");
					}
				}
			}
		}


		public void ConsoleClear()
		{
			if (_conTxt != null)
			{
				_conTxt.text = "";
				InputList.Clear();
				MsgList.Clear();
			}
		}

		public void AddMsg(Msg nMsg)
		{
			if (_msgList != null)
			{
				_msgList.Add(nMsg);
				_msgCounter++;
			}
		}

		public void AddMsg(string nMsg)
		{
			if (_msgList != null)
			{
				_msgList.Add(new Msg(_msgCounter, nMsg));
				_msgCounter++;
			}
		}

		public void AddInput(string nInput)
		{
			if (nInput != null)
			{
				InputList.Add(nInput);

				InputIndex = InputList.Count;

				if (InputList.Count > CMD_COUNT_MAX)
				{
					int numToRemove = InputList.Count - CMD_COUNT_MAX;
					for (int i = 0; i < numToRemove; i++)
					{
						InputList.RemoveRange(0, numToRemove);
					}
				}
			}
		}

		protected virtual void ProcessMessages()
		{
			if (MsgList != null && MsgList.Count > 0)
				AddConsoleText(MsgList.ToArray());
            
			if(MsgList.Count > MSG_COUNT_MAX)
			{
				int numToRemove = MsgList.Count - MSG_COUNT_MAX;
				for (int i = 0; i < numToRemove; i++)
				{
					MsgList.RemoveRange(0, numToRemove);
				}
			}

			MsgList.Clear();
		}

		public void CheckPrevCmd()
		{
			if (ConCanvas.activeSelf)
			{
				if (InputList != null && InputList.Count > 0)
				{
					if ( InputIndex > 0 )
					{
						InputIndex--;

						if (ConInput != null)
							ConInput.text = InputList[InputIndex];

						ConInput.MoveToEndOfLine(false, false);
					}
				}
			}
		}

		public void CheckNextCmd()
		{
			if (ConCanvas.activeSelf)
			{
				if (InputList != null && InputList.Count > 0)
				{
					int max = InputList.Count - 1;

					if ( InputIndex < max )
					{
						InputIndex++;

						if (ConInput != null)
							ConInput.text = InputList[InputIndex];

						ConInput.MoveToEndOfLine(false, false);
					}
					else if ( InputIndex == max )
					{
						if ( ConInput != null )
							ConInput.text = "";

						ConInput.MoveToEndOfLine( false, false );
					}
				}
			}
		}

		public void AddConsoleText(Msg nMsg)
		{
			if (_conTxt != null)
			{
				int trimLen = (_conTxt.text.Length + nMsg.Txt.Length) - CON_LENGTH_MAX;
				if (trimLen > 0)
					ClearExcess(trimLen);

				_conTxt.text += nMsg.Txt + "\n";
			}
		}

		public void AddConsoleText(Msg[] nMsgs)
		{
			if (_conTxt != null)
			{
				for(int i = 0; i < nMsgs.Length; i++)
				{
					int trimLen = (_conTxt.text.Length + nMsgs[i].Txt.Length) - CON_LENGTH_MAX;
					if (trimLen > 0)
						ClearExcess(trimLen);

					_conTxt.text += nMsgs[i].Txt + "\n";
				}
			}
		}

		protected void ClearExcess(int nLen)  // nLen is the num of chars to clear
		{
			_conTxt.text = _conTxt.text.Substring(nLen);    // Trim the oldest text first
		}

		public virtual void SetCrosshairTransformSize( float size )
		{
			// Handle crosshair size here
		}

		public virtual void SetCrosshairColor( int color )
		{
			// Handle crosshair color change (and conversion from int) here
		}

		public virtual void SetCrosshairColor( Color color )
		{
			// Handle crosshair color change here
		}

		public virtual void SetCrosshairDot( bool dot )
		{
			// Handle crosshair dot enable/disable here
		}

		public virtual void SetHudColor( int color )
		{
			// Handle hud color change (and conversion from int) here
		}

		public virtual void SetHudColor( Color color )
		{
			// Handle HUD color change here
		}

		public virtual void DisplayInfo( bool fade )
		{
			// Handle info display here
		}

		protected virtual void Update()
		{
			if( DevSpd != null )
			{
				DevSpd.text = HubrisPlayer.Instance != null ? HubrisPlayer.Instance.Speed.ToString() : "No Player Instance";
			}
		}

		protected virtual void LateUpdate()
		{
			if ( Active )
			{
				ProcessMessages();
			}
		}
	}
}
