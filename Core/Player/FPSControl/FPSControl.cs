using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class FPSControl : MonoBehaviour
{
    // Singleton Instance
    private static FPSControl _p = null;

    private static object _lock = new object();
    private static bool _disposing = false; // Check if we're in the process of disposing this singleton

    public static FPSControl Player
    {
        get
        {
            if (_disposing)
                return null;
            else
                return _p;
        }

        set
        {
            lock(_lock)
            {
                if (_p == null)
                    _p = value;
            }
        }
    }

    // Temporary vars for test
    public enum Axis { H, V, M_X, M_Y, NUM_AXIS }

    private bool standing = true;
    private float baseSpd = 0.1f;
    private float spd = 1.0f;
    private float h = 0.0f;
    private float v = 0.0f;
    private float sens = 2.0f;
    private MouseLook mLook = null;
    private Vector2 look = Vector2.zero;
    private Vector3 aH = Vector3.zero;
    private Vector3 aV = Vector3.zero;
    private Vector3 move = Vector3.zero;

    // FPSControl instance vars
    [SerializeField]
    private bool _active = false;
    [SerializeField]
    private GameObject gObj = null;
    [SerializeField]
    private Camera pCam = null;

    // FPSControl properties
    public bool Active
    {
        get { return _active; }
        protected set { _active = value; }
    }
    
    // FPSControl methods
    public void ChangeSpeed(float fSpdNew)
    {
        spd = fSpdNew;
    }

    private Vector3 GetMoveAsVector(Axis ax, float val)
    {
        Vector3 dir = Vector3.zero;

        if (ax == Axis.H)
            dir = new Vector3(val, 0, 0);
        else if (ax == Axis.V)
            dir = new Vector3(0, 0, val);
        else
            Debug.LogError("InputManager GetAxisAsVector(): Invalid Axis Vector requested");

        return dir;
    }

    private Vector2 GetMouseLookAsVector()
    {
        Vector2 dir = Vector2.zero;

        dir.x = Input.GetAxis("Mouse X");   // Horizontal
        dir.y = Input.GetAxis("Mouse Y");   // Vertical

        return dir;
    }

    public void Move(Axis ax, float val)
    {
        if (Active)
        {
            gObj.transform.Translate(GetMoveAsVector(ax, val));
        }
    }

    void OnEnable()
    {
        if (Player == null)
            Player = this;
        else if(Player != null)
        {
            Active = false;
            Destroy(this.gameObject);
        }

        if (gObj == null)
            gObj = this.gameObject;

        if (pCam == null)
            pCam = GetComponent<Camera>();

        if (pCam && gObj != null)
        {
            Active = true;
            mLook = new MouseLook();
            mLook.Init(gObj.transform, pCam.transform, sens, sens);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }
}
