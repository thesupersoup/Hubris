using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
    public class FreeLookControl : HubrisPlayer
    {
        // Temporary vars for test
        private bool standing = true;
        private float h = 0.0f;
        private float v = 0.0f;
        private Vector2 look = Vector2.zero;
        private Vector3 aH = Vector3.zero;
        private Vector3 aV = Vector3.zero;

        // FreeLookControl instance vars


        // FreeLookControl properties


        // FreeLookControl methods
        public override void InteractA()
        {

        }

        public override void InteractB()
        {

        }

        public override void Move(InputManager.Axis ax, float val)
        {
            if (Active)
            {
                _move = GetMoveAsVector(ax, val, true);
                PhysAccel(_move * _spd);
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
                    if (Core.Instance.Debug)
                        LocalConsole.Instance.LogError("FreeLookControl Rotate(): Invalid Axis specified", true);
                }
            }
        }

        protected override void ProcessState()
        {
            ProcessGravity();
            ProcessDeltas();
        }

        protected override void SetSpecifics()
        {

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
        }

        void Start()
        {
            if (Instance == this)
            {
                Type = (byte)PType.FL;

                base.Init();
                SetSpecifics();

                if (_gObj == null)
                    _gObj = this.gameObject;

                if (_pBod == null)
                    _pBod = GetComponent<Rigidbody>();

                if (_pCol == null)
                    _pCol = GetComponent<Collider>();

                if (_pCam == null)
                    _pCam = GetComponent<Camera>();

                if ( _gObj != null && _pBod != null && _pCol != null && _pCam != null)
                {
                    Active = true;
                    _mLook = new MouseLook(_gObj.transform, _pCam.transform, _sens, _sens);
                }
            }
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
