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
					if(_i == null || value == null)
						_i = value;
				}
			}
		}

		// Player states, use properties to interact
		protected bool _canModSpdTgt = true;
		protected bool _prevGrounded = false;
		protected bool _moving = false;

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

		[SerializeField]
		[Tooltip( "Should the player move quickly by default?" )]
		private bool _alwaysRun;

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
		protected static MouseLook _mLook = null;

		// Player properties
		public Camera PlayerCam => _pCam;
		public MoveParams Movement => _moveParams;
		public virtual int ActiveSlot => PlayerInv.ActiveIndex;

		public bool AlwaysRun
		{
			get { return _alwaysRun; }
			set { _alwaysRun = value; }
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

		public virtual bool IsGrounded => _pCon.isGrounded;

		public Vector3 Velocity
		{
			get {  if(_pBod != null) { return _pBod.velocity; } else { return Vector3.zero; } }
		}

		public PType PlayerType
		{
			get { return _pType; }
			protected set
			{
				if (value > PType.NONE && value < PType.NUM_TYPES)
					_pType = value;
				else
					_pType = PType.FPS;
			}
		}

		public MouseLook MLook => _mLook;

		public bool MSmooth
		{
			get { return _mSmooth; }
			set
			{
				_mSmooth = value;
				if ( _mLook != null )
					_mLook.EnableMouseSmooth(value);
			}
		}

		public float MSmoothAmt
		{
			get { return _mSmoothAmt; }
			set
			{
				_mSmoothAmt = value;
				if ( _mLook != null )
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
					if ( _mLook != null )
						_mLook.SetSensitivity(value);
				}
			}
		}

		// Player methods
		public abstract void Escape();
		public abstract void Interact0();
		public abstract void Interact1();
		public abstract void Interact2();
		public abstract void Interact3();
		public abstract void Move(InputManager.Axis ax, float val);
		public abstract void SpecMove(SpecMoveType nType, InputManager.Axis ax, float val);
		public abstract void Rotate(InputManager.Axis ax, float val);

		public override void OnEnable()
		{
			HubrisPlayerEnable();

			EntityEnable();
			LiveEntityEnable();
		}

		protected override void Start()
		{
			InitHubrisPlayer();
		}

		public void HubrisPlayerEnable()
		{
			if ( Instance == null )
			{
				Instance = this;
				_disposing = false;
			}
			else if ( Instance != null && Instance != this )
			{
				Deactivate();
				Destroy( this.gameObject );
				return;
			}
		}

		protected void InitHubrisPlayer()
		{
			_uId = HubrisCore.Instance.RegisterPlayer( this, this.gameObject );
			this.gameObject.name += "_" + _uId;

			if ( !HubrisCore.Instance?.Ingame ?? false )
			{
				HubrisCore.Instance.SetIngame( true );
				HubrisCore.Instance.Input.SetLite( false );
			}

			_mLook = new MouseLook( _gObj.transform, _pCam.transform, Sensitivity, Sensitivity, MSmooth, MSmoothAmt );

			// Initiate mouselook 
			MLook.SetCursorLock( true );

			EntType = EntityType.PLAYER;

			SpeedTarget = Movement.SpeedLow;
		}

		/// <summary>
		/// Sets the mouse lock and InputManager appropriately for the boolean arguement
		/// </summary>
		public virtual void EnablePlayerInput( bool enable = true )
		{
			SetMouse( enable );
			HubrisCore.Instance.Input.SetLite( !enable );
		}

		public virtual void SetActiveSlot( int nSlot, bool trySkip = false )
		{
			PlayerInv.SetActiveSlot(nSlot);
		}

		public void SetSpeedTarget( float nTar )
		{
			// Only change the speed target when a movement key is being pressed
			if ( !_moving )
				return;

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

		public virtual void UpdateUI()
		{
			// UI update code goes here
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
			
		}

		protected virtual void LateUpdate()
		{
			
		}

		protected virtual void FixedUpdate()
		{
			
		}

		public override void OnDestroy()
		{
			if ( HubrisCore.Instance != null )
				HubrisCore.Instance.SetIngame( false );

			CleanUp();
		}

		public override void CleanUp(bool full = true)
		{
			if (!this._disposed)
			{
				_disposing = true;

				base.CleanUp();

				if (full)
				{
					Instance = null;
					_mLook = null;
					_gObj = null;
					_pCon = null;
					_pBod = null;
					_pCam = null;

					// Inherited instance vars
					_act = false;
					_name = null;
				}
			}
		}
	}
}
