using System;
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
		private TextMeshProUGUI _conTxt = null;
		[SerializeField]
		private TMP_InputField _conIn = null;
		[SerializeField]
		private GameObject _conCan = null;
		[SerializeField]
		private TextMeshProUGUI _devSpd = null;
		[SerializeField]
		private GameObject _devCan = null;
		private string _input = null;
		private List<Msg> _msgList;
		private List<string> _inputList;
		private int _inputIndex = 0;        // Last index of input list accessed
		private int _msgCounter = 0;


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


		// UIManager methods
		void OnEnable()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else if (Instance != null)
			{
				Destroy(this.gameObject);
			}

			if(Instance == this)
			{
				if(DevCanvas != null)
				{
					DevCanvas.SetActive(HubrisCore.Instance.Debug);
				}

				_msgList = new List<Msg>();
				_inputList = new List<string>();
			}
		}

		private bool CheckActive(Behaviour b)
		{
			if (b != null)
				return b.enabled;
			else
				return false;
		}

		public void DevSet(bool nAct)
		{
			if (_devCan != null)
			{
				_devCan.SetActive(nAct);
			}
		}

		public void DevToggle()
		{
			if(_devCan != null)
			{
				_devCan.SetActive(!_devCan.activeSelf);
			}
		}

		public void ConsoleSet(bool nAct)
		{
			if(_conCan != null)
			{
				_conCan.SetActive(nAct);
			}
		}

		public void ConsoleToggle()
		{
			if (_conCan != null)
			{
				bool act = !_conCan.activeSelf;
				_conCan.SetActive(act);

				if (HubrisPlayer.Instance.PlayerType == HubrisPlayer.PType.FPS)
				{
					HubrisPlayer.Instance.SetMouse(!act);
				}

				if (InputManager.Instance != null)
					InputManager.Instance.SetReady(!act);

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

		public void SetInput(string nIn)
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

		private void ProcessMessages()
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
					if (InputIndex >= 0 && InputIndex <= InputList.Count)
					{
						if (InputIndex > 0)
						{
							InputIndex--;
						}

						if (ConInput != null)
						{
							ConInput.text = InputList[InputIndex];
						}

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
					if (InputIndex >= 0 && InputIndex <= InputList.Count)
					{
						if (InputIndex < InputList.Count - 1)
						{
							InputIndex++;
						}

						if (ConInput != null)
						{
							ConInput.text = InputList[InputIndex];
						}

						ConInput.MoveToEndOfLine(false, false);
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

		private void ClearExcess(int nLen)  // nLen is the num of chars to clear
		{
			_conTxt.text = _conTxt.text.Substring(nLen);    // Trim the oldest text first
		}

		void Update()
		{
			if(DevSpd != null && HubrisPlayer.Instance != null)
			{
				DevSpd.text = HubrisPlayer.Instance.Speed.ToString();
			}
		}

		void LateUpdate()
		{
			if (Active)
			{
				ProcessMessages();
			}
		}
	}
}
