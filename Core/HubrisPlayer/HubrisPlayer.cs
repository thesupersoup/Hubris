using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
    public abstract class HubrisPlayer : LiveEntity
    {
        // HubrisPlayer constants
        public const float DIST_CHK_GROUND = 2.0f, ACCEL_THRESHOLD = 0.1f, DOT_THRESHOLD = -0.75f;

        // Player Type; whether First Person, Free Look, or others
        public enum PType { NONE = 0, FPS, FL, RTS, NUM_TYPES };       

        // Special Movement Type
        public enum SpecMoveType { NONE = 0, JUMP, CROUCH, NUM_TYPES}

        // Singleton instance, to be populated by the derived class
        private static HubrisPlayer _i = null;

        private static object _lock = new object();
        private static bool _disposing = false; // Check if we're in the process of disposing this singleton

        public static HubrisPlayer Instance
        {
            get
            {
                if (_disposing)
                    return null;
                else
                    return _i;
            }

            protected set
            {
                lock (_lock)
                {
                    if(_i == null)
                        _i = value;
                }
            }
        }

        // Player states, use properties to interact
        protected bool _canModSpdTgt = true;
        protected bool _prevGrounded = false;
        protected bool _moving = false;
        protected bool _fastMove = false;

        // Player instance variables
        [Header("Player type and components")]
        [SerializeField]
        protected PType _pType = PType.FPS;
        [SerializeField]
        protected GameObject _gObj = null;
        [SerializeField]
        protected CharacterController _pCon = null;
        [SerializeField]
        protected Rigidbody _pBod = null;
        [SerializeField]
        protected Camera _pCam = null;

        [Header("Mouse parameters")]
        [Tooltip("Sensitivity")]
        [SerializeField]
        protected float _sens = 2.0f;
        [Tooltip("Mouse smoothing")]
        [SerializeField]
        protected bool _mSmooth = false;
        [Tooltip("Mouse smoothing factor")]
        [SerializeField]
        protected float _mSmoothAmt = 1.0f;

        [SerializeField]
        protected MoveParams _moveParams = null;

        protected float _spd = 0.0f;
        protected float _spdTar = 0.0f;

        [SerializeField]
        protected Inventory _inv = new Inventory();

        protected PlayerState _pState = new PlayerState();
        protected Vector3 _gravVector = Vector3.zero;
        protected Vector3 _move = Vector3.zero;
        protected Vector3 _prevMove = Vector3.zero;
        protected CollisionFlags _flags;
        protected RaycastHit _grndChk;
        protected float _slopeChk;
        protected float _fallAng = 59.0f;   // The angle of the slope at which the player can no longer walk forward or jump
        protected static InputManager _im = null;
        protected static MouseLook _mLook = null;

        // Player properties
        public Camera PlayerCam
        {
            get { return _pCam; }
        }

        public MoveParams Movement
        {
            get { return _moveParams; }
        }

        public float Speed
        {
            get { return _spd; }
            protected set { _spd = value; }
        }

        public float SpeedTarget
        {
            get { return _spdTar; }
            protected set { _spdTar = value; }
        }

        public Inventory PlayerInv
        {
            get { return _inv; }
        }

        public PlayerState State
        {
            get { return _pState; }
        }

        public bool IsGrounded
        {
            get { return _pCon.isGrounded; }
        }

        public Vector3 Velocity
        {
            get {  if(_pBod != null) { return _pBod.velocity; } else { return Vector3.zero; } }
        }

        public byte PlayerType
        {
            get { return (byte)_pType; }
            protected set
            {
                if (value > (byte)PType.NONE && value < (byte)PType.NUM_TYPES)
                {
                    _pType = (PType)value;
                }
                else
                {
                    _pType = PType.FPS;
                }
            }
        }

        public bool MSmooth
        {
            get { return _mSmooth; }
            set
            {
                _mSmooth = value;
                _mLook.EnableMouseSmooth(value);
            }
        }

        public float MSmoothAmt
        {
            get { return _mSmoothAmt; }
            set
            {
                _mSmoothAmt = value;
                _mLook.SetSmoothAmt(value);
            }
        }

        public float Sensitivity
        {
            get { return _sens; }
            set
            {
                if (value >= 0.0f)
                {
                    _sens = value;
                    _mLook.SetSensitivity(value);
                }
            }
        }

        public static InputManager Input
        {
            get { return _im; }
            protected set { _im = value; }
        }

        // Player methods
        public abstract void Interact0();
        public abstract void Interact1();
        public abstract void Move(InputManager.Axis ax, float val);
        public abstract void SpecMove(SpecMoveType nType, InputManager.Axis ax, float val);
        public abstract void Rotate(InputManager.Axis ax, float val);

        protected override void Init()
        {
            EntType = LiveEntity.EType.PLAYER;
            Stats = EntStats.Create(EntType);
            _im = new InputManager();
            _im.Init(_pType);
            SpeedTarget = Movement.SpeedLow;
        }

        public void SetActiveSlot(int nSlot)
        {
            PlayerInv.SetActiveSlot(nSlot);
        }

        public void SetSpeedTarget(float nTar)
        {
            SpeedTarget = nTar;
        }

        /// <summary>
        /// Returns a normalized direction vector along the specified axis. Can be relative to current rotation.
        /// </summary>
        public Vector3 GetMoveAsVector(InputManager.Axis ax, float val, bool relative = false)
        {
            Vector3 dir = Vector3.zero;

            if (!relative)
            {
                if (ax == InputManager.Axis.X)
                    dir = new Vector3(val, 0, 0);
                else if (ax == InputManager.Axis.Y)
                    dir = new Vector3(0, val, 0);
                else if (ax == InputManager.Axis.Z)
                    dir = new Vector3(0, 0, val);
                else
                {
                    if (HubrisCore.Instance.Debug)
                        LocalConsole.Instance.LogError("InputManager GetMoveAsVector(): Invalid Axis Vector requested", true);
                }
            }
            else
            {
                if (ax == InputManager.Axis.X)
                    dir = _gObj.transform.right * val;
                else if (ax == InputManager.Axis.Y)
                    dir = _gObj.transform.up * val;
                else if (ax == InputManager.Axis.Z)
                    dir = _gObj.transform.forward * val;
                else
                {
                    if (HubrisCore.Instance.Debug)
                        LocalConsole.Instance.LogError("InputManager GetMoveAsVector(): Invalid Axis Vector requested", true);
                }
            }
            
            return dir.normalized;
        }

        protected virtual void ProcessGravity()
        {
            if(Active)
            {
                if (!IsGrounded)
                {
                    _gravVector = Physics.gravity * Movement.GravFactor * Time.fixedDeltaTime;
                }
                else
                {
                    _gravVector = Vector3.zero;
                    _gravVector.y = -Movement.GravBase;
                }
            }
        }

        protected virtual void ProcessDeltas()
        {
            if (Movement.UseAccel)
            {
                if (Movement.Decay < 1.0f)
                {
                    Speed -= (Speed * Movement.Decay);
                }
            }
        }

        protected virtual void ProcessState()
        {
            ProcessGravity();
            ProcessDeltas();

            _prevGrounded = _pCon.isGrounded;
        }

        protected void UpdateMouse()
        {
            if (_mLook.CursorLock)
                _mLook.LookRotation(_gObj.transform, _pCam.transform);
            else
                _mLook.UpdateCursorLock();
        }

        public void ToggleMouse()
        {
            _mLook.ToggleCursorLock();
        }

        public void SetMouse(bool nLock)
        {
            _mLook.SetCursorLock(nLock);
        }

        public bool GetMouse()
        {
            return _mLook.CursorLock;
        }

        protected virtual void Update()
        {
            /* IMPORTANT! */
            // Include InputManager.Update() in all derived/overridden Update() methods or call base.Update()
            _im.Update(); 
        }

        protected virtual void LateUpdate()
        {
            /* IMPORTANT! */
            // Include InputManager.LateUpdate() in all derived/overridden Update() methods or call base.LateUpdate()
            _im.LateUpdate();
        }

        protected virtual void FixedUpdate()
        {
            /* IMPORTANT! */
            // Include InputManager.FixedUpdate() in all derived/overridden Update() methods or call base.FixedUpdate()
            _im.FixedUpdate();
        }

        public override void CleanUp(bool full = true)
        {
            if (!this._disposed)
            {
                _disposing = true;

                if (full)
                {
                    Instance = null;
                    Input = null;
                    _mLook = null;
                    _gObj = null;
                    _pCon = null;
                    _pBod = null;
                    _pCam = null;

                    // Inherited instance vars
                    _act = false;
                    _name = null;
                }

                UnsubTick();    // Need to Unsubscribe from Tick Event to prevent errors
                _disposed = true;
            }
        }
    }
}
