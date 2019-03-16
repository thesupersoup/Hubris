using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Hubris
{
    public class UIManager : Entity
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
        private TextMeshProUGUI _conTxt = null;
        [SerializeField]
        private TMP_InputField _conIn = null;
        [SerializeField]
        private Canvas _conCan = null;
        [SerializeField]
        private Canvas _evCan = null;
        [SerializeField]
        private Transform _evPanel = null; 
        [SerializeField]
        private GameObject _evTemp = null;
        [SerializeField]
        private Canvas _devCan = null;
        [SerializeField]
        private Canvas _essCan = null;
        [SerializeField]
        private TextMeshProUGUI[] _essTxtArr = new TextMeshProUGUI[(int)Peeple.Essential.Type.NUM_TYPES];
        [SerializeField]
        private TextMeshProUGUI[] _essTitleArr = new TextMeshProUGUI[(int)Peeple.Essential.Type.NUM_TYPES];
        private List<GameObject> _evList = new List<GameObject>();
        private string _input = null;


        // UIManager properties
        public TextMeshProUGUI ConText
        {
            get { return _conTxt; }
        }

        public TMP_InputField ConInput
        {
            get { return _conIn; }
        }

        public Canvas ConCanvas
        {
            get { return _conCan; }
        }

        public Canvas DevCanvas
        {
            get { return _devCan; }
        }

        public Canvas EssCanvas
        {
            get { return _essCan; }
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
        }

        private bool CheckActive(Behaviour b)
        {
            if (b != null)
                return b.enabled;
            else
                return false;
        }

        public void DevToggle()
        {
            if(_devCan != null)
            {
                _devCan.enabled = !CheckActive(_devCan);
            }
        }

        public void ConsoleToggle()
        {
            if (_conCan != null)
            {
                bool act = !_conCan.enabled;
                _conCan.enabled = act;

                if (HubrisPlayer.Instance.Type == (byte)HubrisPlayer.PType.FPS)
                {
                    HubrisPlayer.Instance.SetMouse(!act);
                }

                if (InputManager.Instance != null)
                    InputManager.Instance.SetReady(!act);

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
                        if (CheckActive(_conCan))
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

        public void UpdateEssentialNames(string[] nNames)
        {
            if (_essTitleArr != null && _essTitleArr.Length == (int)Peeple.Essential.Type.NUM_TYPES)
            {
                for (int i = 0; i < nNames.Length; i++)
                {
                    _essTitleArr[i].text = nNames[i].ToString() + ":";
                }
            }
        }

        public void UpdateEssentialVals(int[] nVals)
        {
            if (_essTxtArr != null && _essTxtArr.Length == (int)Peeple.Essential.Type.NUM_TYPES)
            {
                for(int i = 0; i < nVals.Length; i++)
                {
                    _essTxtArr[i].text = nVals[i].ToString();
                }
            }
        }

        public void AddEvent(Peeple.PlayerPeep nOwner)
        {
            if (_evList != null)
            {
                if (_evPanel != null)
                {
                    if (_evTemp != null)
                    {
                        GameObject tempObj = Instantiate(_evTemp, _evPanel);
                        tempObj.GetComponent<Peeple.Event>().SetEventDetails(0, nOwner);
                    }
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
            if(Core.Instance.Debug)
            {
               
            }
        }
    }
}
