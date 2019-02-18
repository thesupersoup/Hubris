using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
    public class FreeLookControl : Player
    {
        // Temporary vars for test
        private bool standing = true;
        private float h = 0.0f;
        private float v = 0.0f;
        private Vector2 look = Vector2.zero;
        private Vector3 aH = Vector3.zero;
        private Vector3 aV = Vector3.zero;
        private Vector3 move = Vector3.zero;

        // FreeLookControl instance vars


        // FreeLookControl properties
        public bool Active
        {
            get { return _active; }
            protected set { _active = value; }
        }

        // FreeLookControl methods
        public override void Move(InputManager.Axis ax, float val)
        {
            if (Active)
            {
                Vector3 dir = GetMoveAsVector(ax, val, true);
                PhysAccel(dir * _spd);
            }
        }

        public override void Rotate(InputManager.Axis ax, float val)
        {
            if (Active)
            {
                if (ax == InputManager.Axis.X)
                {
                    _gObj.transform.Rotate(val, 0.0f, 0.0f, Space.World);
                }
                if (ax == InputManager.Axis.Y)
                {
                    _gObj.transform.Rotate(0.0f, val, 0.0f, Space.World);
                }
                else if (ax == InputManager.Axis.Z)
                {
                    _gObj.transform.Rotate(0.0f, 0.0f, val, Space.World);
                }
                else
                {
                    LocalConsole.Instance.LogError("FreeLookControl Rotate(): Invalid Axis specified", true);
                }

                LocalConsole.Instance.Log("FPSControl Rotate(): Calling a rotation on Player...", true);
            }
        }

        protected override void ProcessState()
        {
            ProcessGravity();
        }

        void OnEnable()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != null)
            {
                Active = false;
                Destroy(this.gameObject);
            }

            if (Instance == this)
            {
                Type = (byte)PType.FL;

                base.Init();

                if (_gObj == null)
                    _gObj = this.gameObject;

                if (_pCam == null)
                    _pCam = GetComponent<Camera>();

                if (_pCol == null)
                    _pCol = GetComponent<Collider>();

                if (_pCam != null && _gObj != null && _pCol != null)
                {
                    Active = true;
                    _mLook = new MouseLook(_gObj.transform, _pCam.transform, _sens, _sens);
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Active)
            {
                base.Update();
                UpdateMouse();
            }
        }

        void LateUpdate()
        {
            if (Active)
            {
                base.LateUpdate();
            }
        }

        private void FixedUpdate()
        {
            if (Active)
            {
                CheckCollisions();
                ProcessState();
            }
        }
    }
}
