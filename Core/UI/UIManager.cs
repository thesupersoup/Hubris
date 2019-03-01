using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Hubris
{
    public class UIManager : MonoBehaviour
    {
        // UIManager consts
        private const int CON_LENGTH_MAX = 2056;

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
                    return _ui;
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
        TextMeshProUGUI _conTxt = null;
        [SerializeField]
        TMP_InputField _conIn = null;
        [SerializeField]
        Canvas _conCan = null;
        private string _input = null;
        private Thread _uiThread;

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
        }

        public bool ConsoleCheckActive()
        {
            if (_conCan != null)
                return _conCan.enabled;
            else
                return false;
        }

        public void ConsoleToggle()
        {
            if (_conCan != null)
            {
                bool act = !_conCan.enabled;
                _conCan.enabled = act;
                Player.Instance.SetMouse(!act);

                if (InputManager.Instance != null)
                    InputManager.Instance.SetActive(!act);

                if (_conIn != null)
                {
                    if (act)
                    {
                        _conIn.ActivateInputField();
                    }
                    else
                    {
                        _conIn.text = "";
                        _conIn.DeactivateInputField();
                    }
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
                        if (ConsoleCheckActive())
                        {
                            _input = _conIn.text;
                            LocalConsole.Instance.ProcessInput(_input);
                            _conIn.text = "";
                            _conIn.ActivateInputField();
                        }
                        else
                        {
                            if (Core.Instance.Debug)
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
            }
        }

        public void AddConsoleText(LocalConsole.Msg nMsg)
        {
            if (_conTxt != null)
            {
                int trimLen = (_conTxt.text.Length + nMsg.Txt.Length) - CON_LENGTH_MAX;
                if (trimLen > 0)
                    ClearExcess(trimLen);

                _conTxt.text += nMsg.Txt + "\n";
            }
        }

        public void AddConsoleText(LocalConsole.Msg[] nMsgs)
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

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
