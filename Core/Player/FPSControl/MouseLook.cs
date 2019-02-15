using System;
using UnityEngine;


[Serializable]
public class MouseLook
{
    // MouseLook instance vars
    private bool lockCursor = false;
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

    // MouseLook Methods
    public void Init(Transform character, Transform camera, float nX = 0.0f, float nY = 0.0f)
    {
		if(nX != 0.0f)
			XSens = nX;
		if(nY != 0.0f)
			YSens = nY;
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


    public void SetCursorLock()
    {
        if(!lockCursor)
        {//we force unlock the cursor if the user disable the cursor locking helper
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
        bool prevLock = lockCursor;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            lockCursor = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            lockCursor = true;
        }

        if (prevLock != lockCursor) 
            SetCursorLock();
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
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
