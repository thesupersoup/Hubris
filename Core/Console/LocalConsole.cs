using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
    public class LocalConsole
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
        private bool _ready = false;    // Ready to process commands
        private List<Msg> _msgList;
        private int _msgCounter = 0;
        private List<Command> _cmdQueue;    
        private List<string> _cmdData;
        private HubrisPlayer _pScript;


        // Console properties
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

            if (HubrisPlayer.Instance != null)
            {
                _pScript = HubrisPlayer.Instance;
                Ready = true;
            }
            else
            {
                if(Core.Instance.Debug)
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
                        case Command.CmdType.InteractA:
                            HubrisPlayer.Instance.InteractA();
                            break;
                        case Command.CmdType.InteractB:
                            HubrisPlayer.Instance.InteractB();
                            break;
                        case Command.CmdType.Submit:
                            UIManager.Instance.ConsoleSubmitInput();
                            break;
                        case Command.CmdType.MoveF:
                            HubrisPlayer.Instance.Move(InputManager.Axis.Z, 1.0f);
                            break;
                        case Command.CmdType.MoveB:
                            HubrisPlayer.Instance.Move(InputManager.Axis.Z, -1.0f);
                            break;
                        case Command.CmdType.MoveL:
                            HubrisPlayer.Instance.Move(InputManager.Axis.X, -1.0f);
                            break;
                        case Command.CmdType.MoveR:
                            HubrisPlayer.Instance.Move(InputManager.Axis.X, 1.0f);
                            break;
                        case Command.CmdType.Jump:
                            if(HubrisPlayer.Instance.Grounded)
                                HubrisPlayer.Instance.PhysImpulse(Vector3.up * HubrisPlayer.Instance.JumpSpd);
                            break;
                        case Command.CmdType.Console:
                            UIManager.Instance.ConsoleToggle();
                            break;
                        case Command.CmdType.ConClear:
                            UIManager.Instance.ConsoleClear();
                            break;
                        case Command.CmdType.RotLeft:
                            HubrisPlayer.Instance.Rotate(InputManager.Axis.Y, -1.0f);
                            break;
                        case Command.CmdType.RotRight:
                            HubrisPlayer.Instance.Rotate(InputManager.Axis.Y, 1.0f);
                            break;
                        case Command.CmdType.Net_Send:
                            HubrisPlayer.Instance.SendData(_cmdQueue[i].Data);
                            break;
                        case Command.CmdType.Version:
                            Core.Instance.VersionPrint();
                            break;
                        case Command.CmdType.Debug:
                            Core.Instance.DebugToggle();
                            UIManager.Instance.DevToggle();
                            break;
                        case Command.CmdType.Net_Info:
                            Core.Instance.NetInfoPrint();
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

            if (Core.Instance.Debug)
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
                            if (Core.Instance.Debug)
                                Log("Calling " + temp.CmdName + " with data " + temp.Data);
                        }
                        else
                        {
                            if (Core.Instance.Debug)
                                Log("Calling " + temp.CmdName);
                        }

                        if (temp.CmdName != Command.ConClear.CmdName)
                        {
                            if (temp.Data == null)
                                Log(temp.CmdName);
                            else
                                Log(temp.CmdName + ": " + temp.Data);
                        }

                        AddToQueue(temp);
                    }
                    else
                    {
                        if (Core.Instance.Debug)
                            Log("LocalConsole ProcessInput(): Unrecognized command \'" + strArr[0] + "\'");
                        else
                            Log("Unrecognized command \"" + strArr[0] + "\"");
                    }
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
            if (Ready)
                ProcessCommands();
            else
            {

                if (HubrisPlayer.Instance != null)
                {
                    _pScript = HubrisPlayer.Instance;
                    if (Core.Instance.Debug)
                        Log("LocalConsole Update(): FPSControl.Player found, setting Ready = true", true);
                    Ready = true;
                }
            }
        }

        // LateTick is called once per frame after Update() with MonoBehaviour.LateUpdate()
        public new void LateTick()
        {
            if (Ready)
                ProcessMessages();
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
