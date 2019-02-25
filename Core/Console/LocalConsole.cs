using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
    public class LocalConsole : Base
    {
        public class Msg
        {
            // Msg instance variables
            private int _id;        // ID # of Msg, for referencing specific Msgs when desired
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

        // Console instance vars
        private bool _active = true;    // This LocalConsole script initialized correctly
        private bool _ready = false;    // Ready to process commands
        private List<Msg> _msgList;
        private int _msgCounter = 0;
        private List<Command> _cmdQueue;    
        private List<string> _cmdData;
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

        public static LocalConsole Instance
        {
            get { return Core.Instance.Console; }    // Moved LocalConsole instance to Core class
            protected set { }
        }

        // Console methods
        public void Init()
        {
            _msgList = new List<Msg>();
            _cmdQueue = new List<Command>();
            _cmdData = new List<string>();

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
                            UIManager.Instance.ConsoleSubmitInput();
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
                            UIManager.Instance.ConsoleToggle();
                            break;
                        case Command.CmdType.RotLeft:
                            Player.Instance.Rotate(InputManager.Axis.Y, -1.0f);
                            break;
                        case Command.CmdType.RotRight:
                            Player.Instance.Rotate(InputManager.Axis.Y, 1.0f);
                            break;
                        case Command.CmdType.Net_Send:
                            Player.Instance.SendData(_cmdQueue[i].Data);
                            break;
                        case Command.CmdType.Version:
                            Log("Current Hubris Build: " + Core.Instance.Version);
                            break;
                        case Command.CmdType.Net_Info:
                            Log("NetLibType: " + Core.Instance.NetLibType);
                            Log("NetSendMethod: " + Core.Instance.NetSendMethod);
                            break;
                        default:
                            break;

                    }

                    cmd.ClearData();    // Clear out data from processed commands
                }

                _cmdQueue.Clear();      // Clear out cmdQueue for next update
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

            Log("LocalConsole ProcessInput(): Processing input \'" + nIn + "\'");

            if (nIn != null)
            {
                strArr = nIn.Split(new char[] { ' ' }, 2); // Split at whitespace, max two strings (Cmd and data)

                if (strArr != null && strArr.Length > 0)
                {
                    Command temp = Command.CheckCmdName(strArr[0]);

                    if (temp != Command.None)
                    {
                        success = true;

                        if (strArr.Length > 1)
                        {
                            temp.SetData(strArr[1]);    // Assume the other string in the array is the data
                            Log("Calling " + temp.CmdName + " with data " + temp.Data);
                        }
                        else
                        {
                            Log("Calling " + temp.CmdName);
                        }

                        AddToQueue(temp);
                    }
                    else
                        Log("LocalConsole ProcessInput(): Unrecognized command \'" + strArr[0] + "\'");
                }
            }

            return success;
        }

        public void AddToQueue(Command nAdd, string nData = null)
        {
            if (nData != null)
                nAdd.SetData(nData);

            _cmdQueue.Add(nAdd);
        }

        // Tick is called once per frame with MonoBehaviour.Update()
        public new void Tick()
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

        // LateTick is called once per frame after Update() with MonoBehaviour.LateUpdate()
        public new void LateTick()
        {
            if (Active)
            {
                if (Ready)
                    ProcessMessages();
            }
        }

        public void AddMsg(Msg nMsg)
        {
            if(_msgList != null)
                _msgList.Add(nMsg);
        }

        public void Log(string msg, bool unity = false, Command cmd = null)
        {
            if (Instance != null)
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
            else
                UnityEngine.Debug.Log(msg);
        }

        public void LogWarning(string msg, bool unity = false, Command cmd = null)
        {
            if (Instance != null)
            {
                if (msg != null)
                {
                    string cmdName = null;
                    if (cmd != null)
                        cmdName = cmd.CmdName;

                    AddMsg(new Msg(_msgCounter, "*WARNING* " + msg, cmdName));

                    if (unity)
                        UnityEngine.Debug.LogWarning(msg);

                    _msgCounter++;
                }
            }
            else
                UnityEngine.Debug.LogWarning(msg);
        }

        public void LogError(string msg, bool unity = false, Command cmd = null)
        {
            if (Instance != null)
            {
                if (msg != null)
                {
                    string cmdName = null;
                    if (cmd != null)
                        cmdName = cmd.CmdName;

                    AddMsg(new Msg(_msgCounter, "*ERROR* " + msg, cmdName));

                    if (unity)
                        UnityEngine.Debug.LogError(msg);

                    _msgCounter++;
                }
            }
            else
                UnityEngine.Debug.LogError(msg);
        }
    }
}
