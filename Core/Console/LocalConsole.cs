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
        private List<Command> _cmdQueue;
        private Player _pScript;

        // Add references to Player, InputManager, mainly used keys here


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
                _cmdQueue = new List<Command>();
                if (Player.Instance != null)
                {
                    _pScript = Player.Instance;
                    Ready = true;
                }
                else
                {
                    Debug.LogError("LocalConsole OnEnable(): Player.Instance is null");
                    _pScript = null;
                }
            }
        }

        private void ProcessCommands()
        {
            if (_cmdQueue != null && _cmdQueue.Count > 0)
            {
                foreach (Command cmd in _cmdQueue)
                {
                    switch (cmd.Type)
                    {
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
                        Debug.Log("LocalConsole Update(): FPSControl.Player found, setting Ready = true");
                        Ready = true;
                    }
                }
            }
        }

        void FixedUpdate()
        {

        }
    }
}
