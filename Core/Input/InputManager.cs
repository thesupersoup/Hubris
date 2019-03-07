using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
    public class InputManager : Entity
    {
        public enum Axis { X, Y, Z, M_X, M_Y, NUM_AXIS }

        // InputManager instance variables
        private KeyMap _km;
        private HubrisPlayer.PType _type = HubrisPlayer.PType.NONE;

        // InputManager properties
        public KeyMap KeyMap
        {
            get { return _km; }
            protected set { _km = value; }
        }

        public static InputManager Instance
        {
            get { return HubrisPlayer.Input; }
            protected set { }
        }

        // InputManager methods
        public void SetActive(bool nAct)
        {
            if (Core.Instance.Debug)
                LocalConsole.Instance.Log("Setting InputManager Active to " + nAct, true);
            Active = nAct;
        }

        public void Init(HubrisPlayer.PType nType)
        {
            _type = nType;
            KeyMap = new KeyMap();
            if (LocalConsole.Instance == null)
            {
                Debug.LogError("InputManager Start(): LocalConsole.Instance is null");
                Active = false;
            }
        }

        private bool CheckValidForType(Command nCmd)
        {
            bool valid;

            switch(_type)
            {
                case HubrisPlayer.PType.FPS:
                    valid = true;
                    break;
                case HubrisPlayer.PType.FL:
                    valid = true;
                    break;
                case HubrisPlayer.PType.RTS:
                    switch(nCmd.Type)
                    {
                        case Command.CmdType.Jump:
                        case Command.CmdType.CrouchHold:
                        case Command.CmdType.CrouchToggle:
                            valid = false;
                            break;
                        default:
                            valid = true;
                            break;
                    }
                    break;
                default:
                    valid = true;
                    break;
            }

            return valid;
        }

        public new void Tick()
        {
            if (Active)
            {
                if (Input.anyKey)
                {
                    foreach (KeyCode kc in System.Enum.GetValues(typeof(KeyCode)))
                    {
                        if (Input.GetKey(kc))
                        {
                            Command cmd = KeyMap.CheckKeyCmd(kc);

                            if (cmd != Command.None)
                            {
                                if (CheckValidForType(cmd))
                                {
                                    if (cmd.Continuous)
                                    {
                                        LocalConsole.Instance.AddToQueue(cmd);
                                    }
                                    else
                                    {
                                        if (Input.GetKeyDown(kc))
                                        {
                                            LocalConsole.Instance.AddToQueue(cmd);
                                        }
                                    }
                                }
                                else
                                {
                                    if (Core.Instance.Debug)
                                        LocalConsole.Instance.LogWarning("InputManager Tick(): CheckValidForType(" + cmd.CmdName + ") returned false", true);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // Only accept input for the following specific keys when the InputManager is inactive
                if (Input.anyKey)
                {
                    foreach (KeyCode kc in System.Enum.GetValues(typeof(KeyCode)))
                    {
                        if (Input.GetKey(kc))
                        {
                            Command cmd = KeyMap.CheckKeyCmd(kc);

                            if (cmd == Command.Console || cmd == Command.Submit)
                            {
                                if (Input.GetKeyDown(kc))
                                {
                                    LocalConsole.Instance.AddToQueue(cmd);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
