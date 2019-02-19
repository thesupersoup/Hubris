using System;
using UnityEngine;
using Hubris;

[Serializable]
public class MouseLook
{
    // MouseLook instance vars
    private bool cursorLock = false;
    private float XSens = 1f;
    private float YSens = 1f;
    private bool clampVerticalRotation = true;
    private float MinimumX = -90F;
    private float MaximumX = 90F;
    private bool smooth;
    private float smoothTime = 5f;
    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;

    public Vector2 Sens
    {
        get { return new Vector2(XSens, YSens); }
    }

    // MouseLook properties
    public bool CursorLock
    {
        get { return cursorLock; }
        set { cursorLock = value; }
    }

    // MouseLook Methods
    public MouseLook(Transform character, Transform camera, float nX = 0.0f, float nY = 0.0f)
    {
        Init(character, camera, nX, nY);
    }

    public void Init(Transform character, Transform camera, float nX = 0.0f, float nY = 0.0f)
    {
		if(nX != 0.0f)
			XSens = nX;
		if(nY != 0.0f)
			YSens = nY;
        smooth = false;
        m_CharacterTargetRot = character.localRotation;
        m_CameraTargetRot = camera.localRotation;
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
            if(!UIManager.Instance.ConsoleCheckActive())
                prevLock = true;
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
