using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
    public class InputManager : MonoBehaviour
    {
        public enum Axis { X, Y, Z, M_X, M_Y, NUM_AXIS }

        // Singleton instance
        public static InputManager _im = null;

        private static object _lock = new object();
        private static bool _disposing = false; // Check if we're in the process of disposing this singleton

        public static InputManager Instance
        {
            get
            {
                if (_disposing)
                    return null;
                else
                    return _im;
            }

            set
            {
                lock (_lock)
                {
                    if (_im == null)
                        _im = value;
                }
            }
        }

        // InputManager variables
        private bool _active = true;
        private KeyMap _km;

        // InputManager properties
        public bool Active
        {
            get { return _active; }
            protected set { _active = value; }
        }

        public KeyMap KeyMap
        {
            get { return _km; }
            protected set { _km = value; }
        }

        // InputManager methods
        public void SetActive(bool nAct)
        {
            LocalConsole.Instance.Log("Setting InputManager Active to " + nAct, true);
            Active = nAct;
        }

        private void OnEnable()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
            {
                Destroy(this.gameObject);
                Active = false;
            }
        }

        void Start()
        {
            KeyMap = new KeyMap();
            if (LocalConsole.Instance == null)
            {
                Debug.LogError("InputManager Start(): LocalConsole.Instance is null");
                _active = false;
            }
        }

        // Update is called once per frame
        void Update()
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

        private void FixedUpdate()
        {

        }
    }
}
