using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Peeple;

namespace Hubris
{
    public class RTSControl : HubrisPlayer
    {
        // Temporary vars for test
        private float h = 0.0f;
        private float v = 0.0f;
        private Vector2 look = Vector2.zero;
        private Vector3 aH = Vector3.zero;
        private Vector3 aV = Vector3.zero;

        // RTSControl instance vars


        // RTSControl properties


        // RTSControl methods
        public override void InteractA()   // For RTSControl, InteractA is contextually select or move
        {
            if(_pCam != null)
            {
                RaycastHit hit;

                if(Physics.Raycast(_pCam.ScreenPointToRay(UnityEngine.Input.mousePosition), out hit))
                {
                    if (!GameManager.Instance.CheckSelected)
                    {
                        Component chkComp = hit.collider.GetComponent<Peep>();
                        if (chkComp != null)
                        {
                            GameManager.Instance.SetSelected((Peep)chkComp);
                        }
                    }
                    else
                    {
                        GameManager.Instance.MoveSelected(hit.point);
                    }
                }
            }
        }

        public override void InteractB()   // For RTSControl, InteractB is deselect
        {
            GameManager.Instance.Deselect();
        }

        public override void Move(InputManager.Axis ax, float val)
        {
            if(Active)
            {
                _gObj.transform.Translate(GetMoveAsVector(ax, val * _spd, true), Space.World);
                // PhysForce(_move * _spd);
            }
        }

        public override void Rotate(InputManager.Axis ax, float val)
        {
            if (Active)
            {
                if (ax == InputManager.Axis.Y)
                {
                    _gObj.transform.Rotate(0.0f, val, 0.0f, Space.World);
                }
                else
                {
                    if (Core.Instance.Debug)
                        LocalConsole.Instance.LogError("RTSControl Rotate(): Invalid Axis specified", true);
                }
            }
        }

        protected override void ProcessState()
        {

        }

        protected override void SetSpecifics()
        {
            Speed = 0.25f;
        }

        void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;

            }
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
                Type = (byte)PType.RTS;

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

                if (_gObj != null && _pBod != null && _pCol != null && _pCam != null)
                {
                    Active = true;
                    _mLook = new MouseLook(_gObj.transform, _pCam.transform, _sens, _sens);
                    _mLook.SetCursorLock(false);
                }
            }
        }

        void Update()
        {
            if (Active)
            {
                base.Update();
            }
        }

        void LateUpdate()
        {
            if (Active)
            {
                base.LateUpdate();
            }
        }

        void FixedUpdate()
        {
            if (Active)
            {
                CheckCollisions();
                ProcessState();
            }
        }
    }
}
