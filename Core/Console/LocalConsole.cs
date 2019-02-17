using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hubris
{
    public class LocalConsole : MonoBehaviour
    {
        // Singleton instance
        public static LocalConsole _con = null;

        private static object _lock = new object();
        private static bool _disposing = false; // Check if we're in the process of disposing this singleton

        public static LocalConsole Instance
        {
            get
            {
                if (_disposing)
                    return null;
                else
                    return _con;
            }

            set
            {
                lock (_lock)
                {
                    if (_con == null)
                        _con = value;
                }
            }
        }

        // Console instance vars
        private bool _active = true;    // This LocalConsole script initialized correctly
        private bool _ready = false;    // Ready to process commands
        private List<Msg> _msgList;
        private int _msgCounter = 0;
        private List<Command> _cmdQueue;
        private Player _pScript;


        // Console properties
        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        public bool Ready
        {
            get { return _ready; }
            set { _ready = value; }
        }

        // Console methods
        private void OnEnable()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
            {
                Active = false;
                Destroy(this.gameObject);
            }

            if (Active)
            {
                _msgList = new List<Msg>();
                _cmdQueue = new List<Command>();
                if (Player.Instance != null)
                {
                    _pScript = Player.Instance;
                    Ready = true;
                }
                else
                {
                    LogError("LocalConsole OnEnable(): Player.Instance is null", true);
                    _pScript = null;
                }
            }
        }

        private void ProcessCommands()
        {
            if (_cmdQueue != null && _cmdQueue.Count > 0)
            {
                for(int i = 0; i < _cmdQueue.Count; i++)
                {
                    Command cmd = _cmdQueue[i];

                    switch (cmd.Type)
                    {
                        case Command.CmdType.Submit:
                            if (UIManager.Instance != null)
                                UIManager.Instance.SubmitConsoleInput();
                            break;
                        case Command.CmdType.MoveF:
                            Player.Instance.Move(InputManager.Axis.Z, 1.0f);
                            break;
                        case Command.CmdType.MoveB:
                            Player.Instance.Move(InputManager.Axis.Z, -1.0f);
                            break;
                        case Command.CmdType.MoveL:
                            Player.Instance.Move(InputManager.Axis.X, -1.0f);
                            break;
                        case Command.CmdType.MoveR:
                            Player.Instance.Move(InputManager.Axis.X, 1.0f);
                            break;
                        case Command.CmdType.Jump:
                            if(Player.Instance.Grounded)
                                Player.Instance.PhysImpulse(Vector3.up * Player.Instance.JumpSpd);
                            break;
                        case Command.CmdType.Console:
                            if (UIManager.Instance != null)
                                UIManager.Instance.ToggleConsole();
                            break;
                        case Command.CmdType.RotLeft:
                            Player.Instance.Rotate(InputManager.Axis.X, -5.0f);
                            break;
                        case Command.CmdType.RotRight:
                            Player.Instance.Rotate(InputManager.Axis.X, 5.0f);
                            break;
                        default:
                            break;

                    }
                }

                _cmdQueue.Clear();
            }
        }

        private void ProcessMessages()
        {
            if(UIManager.Instance != null && _msgList != null && _msgList.Count > 0)
                UIManager.Instance.AddConsoleText(_msgList.ToArray());

            _msgList.Clear();
        }

        public bool ProcessInput(string nIn)
        {
            bool success = false;
            string[] strArr;

            Log("Processing input: " + nIn);

            if (nIn != null)
            {
                strArr = nIn.Split(new char[] { ' ' });

                if (strArr != null && strArr.Length > 0)
                {
                    Command temp = Command.CheckCmdName(strArr[0]);

                    if (temp != Command.None)
                    {
                        success = true;
                        Log(temp.CmdName);
                        AddToQueue(temp);
                    }
                    else
                        Log("Unrecognized command \'" + strArr[0] + "\'");
                }
            }

            return success;
        }

        public void AddToQueue(Command cAdd)
        {
            _cmdQueue.Add(cAdd);
        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Active)
            {
                if (Ready)
                    ProcessCommands();
                else
                {

                    if (Player.Instance != null)
                    {
                        _pScript = Player.Instance;
                        Log("LocalConsole Update(): FPSControl.Player found, setting Ready = true", true);
                        Ready = true;
                    }
                }
            }
        }

        private void LateUpdate()
        {
            if (Active)
            {
                if (Ready)
                    ProcessMessages();
            }
        }

        void FixedUpdate()
        {

        }

        public void AddMsg(Msg nMsg)
        {
            if(_msgList != null)
                _msgList.Add(nMsg);
        }

        public void Log(string msg, bool unity = false, Command cmd = null)
        {
            if (msg != null)
            {
                string cmdName = null;
                if (cmd != null)
                    cmdName = cmd.CmdName;

                AddMsg(new Msg(_msgCounter, msg, cmdName));

                if (unity)
                    UnityEngine.Debug.Log(msg);

                _msgCounter++;
            }
        }

        public void LogWarning(string msg, bool unity = false, Command cmd = null)
        {
            if (msg != null)
            {
                string cmdName = null;
                if (cmd != null)
                    cmdName = cmd.CmdName;

                AddMsg(new Msg(_msgCounter, "WARNING: " + msg, cmdName));

                if (unity)
                    UnityEngine.Debug.LogWarning(msg);

                _msgCounter++;
            }
        }

        public void LogError(string msg, bool unity = false, Command cmd = null)
        {
            if (msg != null)
            {
                string cmdName = null;
                if (cmd != null)
                    cmdName = cmd.CmdName;

                AddMsg(new Msg(_msgCounter, "ERROR: " + msg, cmdName));

                if (unity)
                    UnityEngine.Debug.LogError(msg);

                _msgCounter++;
            }
        }

        public class Msg
        {
            // Msg instance variables
            private int _id;       // ID # of Msg, for referencing specific Msgs when desired
            private string _txt;    // The text of the message, to be displayed in the LocalConsole
            private string _cmd;    // The command which generated the message, for debugging purposes

            // Msg properties
            public int Id
            {
                get { return _id; }
                protected set { _id = value; }  // Set in constructor, not to be changed
            }

            public string Txt
            {
                get { return _txt; }
                protected set { _txt = value; } // Set in constructor, not to be changed
            }

            public string Cmd
            {
                get { return _cmd; }
                protected set { _cmd = value; } // Set in constructor, not to be changed
            }
            
            // Msg methods
            public Msg(int nId, string nTxt, string nCmd)
            {
                _id = nId;
                _txt = nTxt;
                _cmd = nCmd;
            }
        }
    }
}
