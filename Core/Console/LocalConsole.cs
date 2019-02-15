using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
                        Player.Instance.Move(InputManager.Axis.V, 1.0f);
                        break;
                    case Command.CmdType.MoveB:
                        Player.Instance.Move(InputManager.Axis.V, -1.0f);
                        break;
                    case Command.CmdType.MoveL:
                        Player.Instance.Move(InputManager.Axis.H, -1.0f);
                        break;
                    case Command.CmdType.MoveR:
                        Player.Instance.Move(InputManager.Axis.H, 1.0f);
                        break;
                    case Command.CmdType.RotLeft:
                        if (Player.Instance.Type == (byte)Player.PType.FL)
                            Player.Instance.CamRot(InputManager.Axis.H, -1.0f);
                        break;
                    case Command.CmdType.RotRight:
                        if (Player.Instance.Type == (byte)Player.PType.FL)
                            Player.Instance.CamRot(InputManager.Axis.H, 1.0f);
                        break;
                    default:
                        break;

                }
            }

            _cmdQueue.Clear();
        }
    }
    /*if (!Input.GetKey(KeyCode.LeftShift))
        spd = 1.0f;
    else
        spd = 3.0f;

    if (!Input.GetKey(KeyCode.LeftControl))
    {
        if (!standing)
        {
            standing = true;
            gObj.transform.localPosition = new Vector3(0, 1, 0);
            gObj.transform.localScale = new Vector3(1, 1, 1);
        }
    }
    else
    {
        if (standing)
        {
            standing = false;
            gObj.transform.localPosition = new Vector3(0, 0.5f, 0);
            gObj.transform.localScale = new Vector3(1, 0.5f, 1);
        }
    }

    spd = spd * baseSpd;

    move = (GetMoveAsVector(Axis.H) + GetMoveAsVector(Axis.V)) * spd;
    look = GetMouseLookAsVector();

    mLook.LookRotation(gObj.transform, cam.transform);*/

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
