using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Hubris
{
    public class UIManager : MonoBehaviour
    {
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
        GameObject _conObj = null;
        [SerializeField]
        TextMeshProUGUI _conTxt = null;
        [SerializeField]
        TMP_InputField _conIn = null;
        private string input = null;

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
            if (_conObj != null)
                return _conObj.activeSelf;
            else
                return false;
        }

        public void ConsoleToggle()
        {
            if (_conObj != null)
            {
                bool act = !_conObj.activeSelf;
                _conObj.SetActive(act);
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
            if (_conObj != null)
            {
                if (_conIn != null)
                {
                    input = _conIn.text;
                    if (LocalConsole.Instance != null)
                    {
                        LocalConsole.Instance.ProcessInput(input);
                    }
                    _conIn.text = "";
                    _conIn.ActivateInputField();
                }
            }
        }

        public void AddConsoleText(LocalConsole.Msg nMsg)
        {
            if (_conTxt != null)
                _conTxt.text += nMsg.Txt + "\n";
        }

        public void AddConsoleText(LocalConsole.Msg[] nMsgs)
        {
            if (_conTxt != null)
            {
                for(int i = 0; i < nMsgs.Length; i++)
                {
                    _conTxt.text += nMsgs[i].Txt + "\n";
                }
            }
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
