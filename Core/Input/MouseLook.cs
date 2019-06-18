using System;
using UnityEngine;
using Hubris;

[Serializable]
public class MouseLook
{
    public const bool DEF_MSMOOTH = false;
    public const float DEF_MSMOOTH_AMT = 1.0f;

    ///--------------------------------------------------------------------
    /// MouseLook instance vars
    ///--------------------------------------------------------------------
    
    private bool cursorLock = false;
    private float XSens = 1f;
    private float YSens = 1f;
    private bool clampVerticalRotation = true;
    private float MinimumX = -90F;
    private float MaximumX = 90F;
    private bool smooth;
    private float smoothTime;
    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;

    ///--------------------------------------------------------------------
    /// MouseLook properties
    ///--------------------------------------------------------------------

    public Vector2 Sens
    {
        get { return new Vector2(XSens, YSens); }
    }

    public bool CursorLock
    {
        get { return cursorLock; }
        set { cursorLock = value; }
    }

    ///--------------------------------------------------------------------
    /// MouseLook methods
    ///--------------------------------------------------------------------

    public MouseLook(Transform character, Transform camera, float nX, float nY, bool mSmooth = DEF_MSMOOTH, float mSmoothAmt = DEF_MSMOOTH_AMT, bool nLock = true)
    {
        Init(character, camera, nX, nY, mSmooth, mSmoothAmt, nLock);
    }

    public void Init(Transform character, Transform camera, float nX, float nY, bool mSmooth = DEF_MSMOOTH, float mSmoothAmt = DEF_MSMOOTH_AMT, bool nLock = true)
    {
		if(nX != 0.0f)
			XSens = nX;
		if(nY != 0.0f)
			YSens = nY;
        smooth = mSmooth;
        smoothTime = mSmoothAmt;
        m_CharacterTargetRot = character.localRotation;
        m_CameraTargetRot = camera.localRotation;
        SetCursorLock(nLock);
    }

    public void SetSensitivity(float nSens)
    {
        XSens = nSens;
        YSens = nSens;
    }

    public void EnableMouseSmooth(bool nSmooth)
    {
        smooth = nSmooth;
    }

    public void SetSmoothAmt(float nAmt)
    {
        smoothTime = nAmt;
    }

    public void LookRotation(Transform character, Transform camera)
    {
        float yRot = Input.GetAxis("Mouse X") * XSens;
        float xRot = Input.GetAxis("Mouse Y") * YSens;

        m_CharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);

        if(clampVerticalRotation)
            m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);

        if(smooth)
        {
            character.localRotation = Quaternion.Slerp (character.localRotation, m_CharacterTargetRot,
                smoothTime * Time.deltaTime);
            camera.localRotation = Quaternion.Slerp (camera.localRotation, m_CameraTargetRot,
                smoothTime * Time.deltaTime);
        }
        else
        {
            character.localRotation = m_CharacterTargetRot;
            camera.localRotation = m_CameraTargetRot;
        }

        UpdateCursorLock();
    }

    public void LookRotationSingleAxis(InputManager.Axis ax, Transform character, Transform camera)
    {
        float yRot;
        float xRot;

        if (ax == InputManager.Axis.X)
        {
            yRot = Input.GetAxis("Mouse X") * XSens;
            xRot = 0.0f;
        }
        else if (ax == InputManager.Axis.Y)
        {
            xRot = Input.GetAxis("Mouse Y") * YSens;
            yRot = 0.0f;
        }
        else
        {
            xRot = 0.0f;
            yRot = 0.0f;
        }

        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        if (clampVerticalRotation)
            m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

        if (smooth)
        {
            character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                smoothTime * Time.deltaTime);
            camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                smoothTime * Time.deltaTime);
        }
        else
        {
            character.localRotation = m_CharacterTargetRot;
            camera.localRotation = m_CameraTargetRot;
        }

        UpdateCursorLock();
    }

    public void ToggleCursorLock()
    {
        SetCursorLock(!CursorLock);
    }

    public void SetCursorLock(bool nLock)
    {
        CursorLock = nLock;

        if(!CursorLock)
        {   // We force unlock the cursor if the user disables the cursor locking helper
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

   public void UpdateCursorLock()
    {
        bool prevLock = CursorLock;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            prevLock = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (UIManager.Instance != null)
            {
                if (UIManager.Instance.ConCanvas != null && !UIManager.Instance.ConCanvas.activeSelf)
                    prevLock = true;
            }
            else
            {
                prevLock = true;
            }
        }

        if (prevLock != CursorLock) 
            SetCursorLock(prevLock);
    }

    public Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

        angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

}
