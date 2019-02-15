using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class FreeLookControl : Player
{
    // Temporary vars for test
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

    private Vector3 GetMoveAsVector(InputManager.Axis ax, float val)
    {
        Vector3 dir = Vector3.zero;

        if (ax == InputManager.Axis.H)
            dir = new Vector3(val, 0, 0);
        else if (ax == InputManager.Axis.V)
            dir = new Vector3(0, 0, val);
        else
            Debug.LogError("InputManager GetMoveAsVector(): Invalid Axis Vector requested");

        return dir;
    }

    public override void Move(InputManager.Axis ax, float val)
    {
        if (Active)
        {
            gObj.transform.Translate(GetMoveAsVector(ax, val) * baseSpd);
        }
    }

    public override void CamRot(InputManager.Axis ax, float val)
    {
        Quaternion m_CharacterTargetRot;
        Quaternion m_CameraTargetRot;
        float yRot;
        float xRot;

        m_CharacterTargetRot = gObj.transform.localRotation;
        m_CameraTargetRot = pCam.transform.localRotation;

        if (ax == InputManager.Axis.H)
        {
            yRot = Input.GetAxis("Mouse X") * 1.0f;
            xRot = 0.0f;
        }
        else if (ax == InputManager.Axis.V)
        {
            xRot = Input.GetAxis("Mouse Y") * 1.0f;
            yRot = 0.0f;
        }
        else
        {
            xRot = 0.0f;
            yRot = 0.0f;
        }

        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        m_CameraTargetRot = mLook.ClampRotationAroundXAxis(m_CameraTargetRot);

        gObj.transform.localRotation = m_CharacterTargetRot;
        pCam.transform.localRotation = m_CameraTargetRot;
    }

    private void UpdateMouse()
    {
        mLook.LookRotationSingleAxis(InputManager.Axis.H, gObj.transform, pCam.transform);
    }

    void OnEnable()
    {
        if (Instance == null)
            Instance = this;
        else if(Instance != null)
        {
            Active = false;
            Destroy(this.gameObject);
        }

        Type = (byte)PType.FL;

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
        UpdateMouse(); 
    }

    private void FixedUpdate()
    {
        
    }
}
