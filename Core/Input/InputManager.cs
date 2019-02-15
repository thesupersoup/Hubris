using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public enum Axis { H, V, M_X, M_Y, NUM_AXIS }

    // InputManager variables
    private bool _active = true;
    private KeyMap _km;

    // InputManager properties
    public bool Active
    {
        get { return _active; }
        protected set { _active = value;  }
    }

    public KeyMap KeyMap
    {
        get { return _km; }
        protected set { _km = value; }
    }

    // InputManager methods
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
        if (_active)
        {
            if (Input.anyKey)
            {
                foreach (KeyCode kc in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if(Input.GetKey(kc))
                    {
                        Command cmd = KeyMap.CheckKeyCmd(kc);

                        if (cmd != Command.None)
                            LocalConsole.Instance.AddToQueue(cmd);
                    }
                }
            }
        }
        else
        {
            if (LocalConsole.Instance != null)
            {
                Debug.LogError("InputManager Update(): Found LocalConsole.Instance, setting Active = true");
                _active = true;
            }
        }
    }

    private void FixedUpdate()
    {
        
    }
}
