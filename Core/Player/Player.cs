using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hubris
{
    public abstract class Player : MonoBehaviour
    {
        // Player constants
        public const float DIST_CHK_GROUND = 1f;

        // Player type enum, whether First Person, Free Look, or others
        public enum PType : byte { NONE = 0, FPS, FL, NUM_TYPES };

        // Player state enum, see bool states below
        public enum PState : byte { NONE = 0, STAND, GRAV, GROUND, DEMIGOD, ROTATE, NUM_STATES };


        // Singleton instance, to be populated by the derived class
        private static Player _i = null;

        private static object _lock = new object();
        private static bool _disposing = false; // Check if we're in the process of disposing this singleton

        public static Player Instance
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
                    if (_i == null)
                        _i = value;
                }
            }
        }


        // Player states, use properties to interact
        private bool _stand = true;     // Is the player standing?
        private bool _grav = true;      // Is the player affected by gravity
        private bool _ground = true;    // Is the player grounded?
        private bool _demigod = false;  // Is the player invulnerable?
        private bool _rotate = false;   // Is the player currently rotating (keypress)?
        private bool _canJump = true;   // Is the player allowed to jump?

        // Player instance variables
        [SerializeField]
        protected PType _type = PType.FPS;
        [SerializeField]
        protected bool _active = false;
        [SerializeField]
        protected GameObject _gObj = null;
        [SerializeField]
        protected Rigidbody _pBod = null;
        [SerializeField]
        protected Collider _pCol = null;
        [SerializeField]
        protected Camera _pCam = null;
        protected float _sens = 2.0f;
        protected float _jumpSpd = 10.0f;
        protected float _fallAng = 59.0f;   // The angle of the slope at which the player can no longer walk forward or jump
        protected float _fallSpd = 20.0f;
        protected float _baseSpd = 0.1f;
        protected float _spd = 2.0f;
        protected MouseLook _mLook = null;

        // Player properties
        public byte Type
        {
            get { return (byte)_type; }
            protected set { if (value > (byte)PType.NONE && value < (byte)PType.NUM_TYPES) { _type = (PType)value; } else { _type = PType.FPS; } }
        }

        public float JumpSpd
        {
            get { return _jumpSpd; }
            protected set { _jumpSpd = value; }
        }

        public bool Standing
        {
            get { return _stand; }
            protected set { _stand = value; }
        }

        public bool Gravity
        {
            get { return _grav; }
            protected set { _grav = value; }
        }

        public bool Grounded
        {
            get { return _ground; }
            protected set { _ground = value;}
        }

        public bool Demigod
        {
            get { return _demigod; }
            protected set { _demigod = value; }
        }

        public bool Rotating
        {
            get { return _rotate; }
            protected set { _rotate = value; }
        }

        public bool CanJump
        {
            get { return _canJump; }
            protected set { _canJump = value; }
        }

        // Player methods
        public abstract void Move(InputManager.Axis ax, float val);
        public abstract void Rotate(InputManager.Axis ax, float val);
        protected abstract void ProcessState();

        public void ChangeSpeed(float fSpdNew)
        {
            _spd = fSpdNew;
        }

        public void PhysAccel(Vector3 dir)
        {
            _pBod.AddForce(dir, ForceMode.Acceleration);
        }

        public void PhysForce(Vector3 dir)
        {
            _pBod.AddForce(dir, ForceMode.Force);
        }

        public void PhysImpulse(Vector3 dir)
        {
            _pBod.AddForce(dir, ForceMode.Impulse);
        }

        public void PhysTorque(Vector3 dir)
        {
            _pBod.AddTorque(dir, ForceMode.Acceleration);
        }


        protected Vector3 GetMoveAsVector(InputManager.Axis ax, float val, bool relative = false)
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
                    Debug.LogError("InputManager GetMoveAsVector(): Invalid Axis Vector requested");
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
                    Debug.LogError("InputManager GetMoveAsVector(): Invalid Axis Vector requested");

                dir.y = 0;
            }
            
            return dir;
        }

        protected void CheckCollisions()
        {
            CheckGround();
        }

        private void CheckGround()
        {
            RaycastHit hit;

            if (Physics.Raycast(_gObj.transform.position, Vector3.down, out hit, DIST_CHK_GROUND))
            {
                float angle = Vector3.Angle(Vector3.up, hit.normal);

                if (angle > _fallAng)
                {
                    CanJump = false;
                    PhysImpulse(Vector3.down * _fallSpd);
                }
                else
                {
                    Grounded = true;
                    CanJump = true;
                }
            }
            else
            {
                Grounded = false;
                CanJump = false;
            }
        }

        protected void ProcessGravity()
        {
            if (!Grounded)
                PhysForce(Vector3.down * _fallSpd);
        }

        protected void UpdateMouse()
        {
            _mLook.LookRotation(_gObj.transform, _pCam.transform);
        }

        void OnCollisionEnter(Collision collision)
        {
            // Debug.Log("Col detected");
        }

        // CheckState(PState) returns 0 for false, 1 for true, and 2 for error
        public byte CheckState(PState ps)
        {
            byte check;

            switch (ps)
            {
                case PState.STAND:
                    if (_stand)
                        check = 1;
                    else
                        check = 0;
                    break;
                case PState.GRAV:
                    if (_grav)
                        check = 1;
                    else
                        check = 0;
                    break;
                case PState.GROUND:
                    if (_ground)
                        check = 1;
                    else
                        check = 0;
                    break;
                case PState.DEMIGOD:
                    if (_demigod)
                        check = 1;
                    else
                        check = 0;
                    break;
                case PState.ROTATE:
                    if (_rotate)
                        check = 1;
                    else
                        check = 0;
                    break;
                default:
                    check = 2; // Error
                    break;
            }

            return check;
        }

        public void SetState(PState ps, bool bVal)
        {
            switch (ps)
            {
                case PState.STAND:
                    _stand = bVal;
                    break;
                case PState.GRAV:
                    _grav = bVal;
                    break;
                case PState.GROUND:
                    _ground = bVal;
                    break;
                case PState.DEMIGOD:
                    _demigod = bVal;
                    break;
                case PState.ROTATE:
                    _rotate = bVal;
                    break;
                default:
                    Debug.LogError("Player SetState(): Invalid PState specified...");
                    break;
            }
        }
    }
}
