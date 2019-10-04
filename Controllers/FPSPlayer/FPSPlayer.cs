using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hubris
{
	public class FPSPlayer : HubrisPlayer
	{
		///--------------------------------------------------------------------
		/// FPSPlayer instance vars
		///--------------------------------------------------------------------

		// State and flag info
		protected static FPSState _state;
		protected static bool _jumping;

		[Header("FPSPlayer variables")]
		[SerializeField]
		[Tooltip("Airborne speed acceleration per fixed update")]
		[Range(0.0f, MoveParams.MAX_RANGE)]
		protected float _airAccel = 0.04f;
		[SerializeField]
		[Tooltip("Airborne speed decay per fixed update")]
		[Range(0.0f, MoveParams.MAX_RANGE)]
		protected float _airDecay = 0.005f;
		[SerializeField]
		[Tooltip("Coefficient of speed lost when landing")]
		[Range(0.0f, MoveParams.MAX_RANGE)]
		protected float _speedLoss = 0.1f;
		[SerializeField]
		[Tooltip("Coefficient of stickiness when landing while still moving")]
		[Range(0.0f, MoveParams.MAX_RANGE)]
		protected float _stickiness = 0.7f;

		[SerializeField]
		[Tooltip( "Should the player take falling damage?" )]
		protected bool _fallDamage = true;

		// Distance to trigger falling damage
		private float _fallDistThreshold = 5.5f;

		protected float _fallStartY = 0.0f;

		// Test variables
		[SerializeField]
		protected LayerMask _weaponMask = 0;

		///--------------------------------------------------------------------
		/// FPSPlayer properties
		///--------------------------------------------------------------------

		public FPSState State
		{
			get { return _state; }
			protected set { _state = value; }
		}

		public float AirAcceleration
		{
			get { return _airAccel; }
			protected set { _airAccel = value; }
		}

		public float AirDecay
		{
			get { return _airDecay; }
			protected set { _airDecay = value; }
		}

		public float SpeedLoss
		{
			get { return _speedLoss; }
			protected set { _speedLoss = value; }
		}

		public float Stickiness
		{
			get { return _stickiness; }
			protected set { _stickiness = value; }
		}

		public LayerMask WeaponMask => _weaponMask;

		///--------------------------------------------------------------------
		/// FPSPlayer methods
		///--------------------------------------------------------------------

		public override void OnEnable()
		{
			base.OnEnable();
		}

		protected override void Start()
		{
			InitFPSPlayer();
		}

		public void InitFPSPlayer()
		{ 
			if (Instance == this)
			{
				_pType = PType.FPS;
				_state = FPSState.DEFAULT;

				if (_gObj == null)
					_gObj = this.gameObject;

				if (_pCon == null)
					_pCon = GetComponent<CharacterController>();

				if (_pBod == null)
					_pBod = GetComponent<Rigidbody>();

				if (_pCam == null)
					_pCam = GetComponent<Camera>();

				if (_gObj != null && _pCon != null && _pBod != null && _pCam != null)
				{
					InitHubrisPlayer();
					SetActive( true );
				}
			}
		}

		public override void Escape()
		{
			if ( UIManager.Instance.IsConsoleActive )
				UIManager.Instance.ConsoleToggle();
		}

		/// <summary>
		/// Primary fire
		/// </summary>
		public override void Interact0()
		{
			if (PlayerInv.Slots[PlayerInv.ActiveIndex] != null && PlayerCam != null)
			{
				if (PlayerInv.Slots[PlayerInv.ActiveIndex] is WeaponBase nWeapon)
				{
					nWeapon.Interact0( PlayerCam, WeaponMask, this );
				}
			}
		}

		/// <summary>
		/// Alt fire
		/// </summary>
		public override void Interact1()
		{
			if ( PlayerInv.Slots[PlayerInv.ActiveIndex] != null && PlayerCam != null )
			{
				if ( PlayerInv.Slots[PlayerInv.ActiveIndex] is WeaponBase nWeapon )
				{
					nWeapon.Interact1( PlayerCam, WeaponMask, this );
				}
			}
		}

		/// <summary>
		/// Reload
		/// </summary>
		public override void Interact2()
		{
			// Override for unique behavior
		}

		/// <summary>
		/// Use
		/// </summary>
		public override void Interact3()
		{
			// Override for unique behavior
		}

		public virtual void TryGetWeapon( string name )
		{
			Debug.LogWarning( "Base FPSPlayer TryGetWeapon called" );
		}

		public virtual void TrySpawn( string name )
		{
			Debug.LogWarning( "Base FPSPlayer TrySpawn called" );
		}

		public override void Move(InputManager.Axis ax, float val)
		{
			if (Active)
			{
				_moving = true;

				SpeedTarget = CheckSpeedTarget();

				// Isolate the new movement vector requested
				Vector3 newMove = GetMoveAsVector(ax, val, true);
				// Only consider horizontal movement
				newMove.y = 0.0f;

				// If our current move isn't a jump and we were previously moving 
				if (newMove != Vector3.zero && _prevMove != Vector3.zero)
				{
					float dotAmt = Vector3.Dot(newMove.normalized, _prevMove.normalized);
					if (dotAmt < DOT_THRESHOLD)
					{
						// Debug.Log("DOT_THRESHOLD reached: " + dotAmt);
						Speed = 0.0f;
					}
				}

				_move += newMove;
			}
		}

		public float CheckSpeedTarget()
		{
			float mod = _crouched ? CROUCH_SLOW : 1.0f;

			if ( !AlwaysRun )
				return Movement.SpeedLow * mod;
			else
				return Movement.SpeedHigh * mod;
		}

		public override void SpecMove(SpecMoveType nType, InputManager.Axis ax, float val)
		{
			switch (nType)
			{
				case SpecMoveType.JUMP:
					if ( _slopeChk >= _pCon.slopeLimit )
						return;

					if (IsGrounded)
					{
						SpeedTarget = CheckSpeedTarget();
						_jumping = true;
						Move(ax, val);
					}
					break;
				default:    // Default to regular move
					Move(ax, val);
					break;
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
					if (HubrisCore.Instance.Debug)
						LocalConsole.Instance.LogError("FPSPlayer Rotate(): Invalid Axis specified", true);
				}
			}
		}

		protected override void ProcessGravity()
		{
			if ( Active )
			{
				if ( !IsGrounded )
				{
					_gravVector += Physics.gravity * Movement.GravFactor * Time.fixedDeltaTime;

					if ( _prevGrounded )
						_fallStartY = this.transform.position.y;
				}
				else
				{
					if ( _slopeChk >= _pCon.slopeLimit )
					{
						_gravVector += _grndChk.normal * Movement.GravBase * Time.fixedDeltaTime;
						return;
					}

					// Set gravVector definitively when grounded
					_gravVector = new Vector3( 0.0f, -Movement.GravBase, 0.0f );

					// If we were previously airborne
					if ( !_prevGrounded )
					{
						// Emit landing sound
						EmitSoundEvent( new SoundEvent( this, this.transform.position, 60.0f, SoundIntensity.NOTEWORTHY ) );
						_state = FPSState.DEFAULT;
						FallDmgCheck();
					}
				}

				if( _jumping )
				{
					_gravVector.y = Movement.JumpSpd;

					_state = FPSState.AIRBORNE;
					_jumping = false;

					return;
				}
			}
		}

		protected virtual void FallDmgCheck ()
		{
			float fallDist = _fallStartY - this.transform.position.y;
			if ( fallDist > 0.0f && fallDist >= _fallDistThreshold )
				TakeDmg( null, (int)DmgType.BASE, (int)( fallDist * 0.75f ), true );  // Fall damage is direct; should bypass armor
		}

		protected override void ProcessDeltas()
		{
			if ( _moving )
			{
				// If no movement key is active
				if ( !InputManager.Instance.MoveKey )
				{
					_moving = false;
					SpeedTarget = 0.0f;
				}
			}

			// If the player's speed and speed target are both 0.0f
			if ( Speed == SpeedTarget && SpeedTarget == 0.0f )
			{
				_moving = false;	// Ensure _moving is false
				_move = Vector3.zero;
				_prevMove = Vector3.zero;
				return;				// Quick return
			}

			// If the player isn't using acceleration, speed immediately changes to target
			if ( !Movement.UseAccel )
			{
				Speed = SpeedTarget;
				return;
			}

			if (IsGrounded) // Use base acceleration and decay values when player is grounded
			{
				// If we landed last update, reduce player speed appropriately
				if( !_prevGrounded )
				{
					if ( !_moving )
						Speed *= (MoveParams.MAX_RANGE - SpeedLoss);
					else
						Speed *= (MoveParams.MAX_RANGE - SpeedLoss) * (MoveParams.MAX_RANGE - Stickiness);
				}

				if (Speed < SpeedTarget)
				{
					if (Movement.Accel < 1.0f)
					{
						Speed += Movement.Accel;

						if (!(Speed < SpeedTarget - ACCEL_THRESHOLD))
						{
							Speed = SpeedTarget;
						}
					}
					else
					{
						Speed = SpeedTarget;
					}
				}
				else if (Speed > SpeedTarget)
				{
					if (Movement.Decay < 1.0f)
					{
						Speed -= Movement.Decay;

						if (!(Speed > SpeedTarget + ACCEL_THRESHOLD))
						{
							Speed = SpeedTarget;
						}
					}
					else
					{
						Speed = SpeedTarget;
					}
				}
			}
			else // If airborne
			{
				if (Speed < SpeedTarget)
				{
					if (AirAcceleration < 1.0f)
					{
						Speed += AirAcceleration;

						if (!(Speed < SpeedTarget - ACCEL_THRESHOLD))
						{
							Speed = SpeedTarget;
						}
					}
					else
					{
						Speed = SpeedTarget;
					}
				}
				else if (Speed > SpeedTarget)
				{
					if (AirDecay < 1.0f)
					{
						Speed -= AirDecay;

						if (!(Speed > 0.0f + ACCEL_THRESHOLD))
						{
							Speed = 0.0f;
						}
					}
					else
					{
						Speed = 0.0f;
					}
				}
			}
		}

		protected override void ProcessState()
		{
			bool foundNormal = false;

			// Check for a normal downward
			if ( Physics.Raycast( this.transform.position, Vector3.down, out _grndChk, _pCon.height ) )
				foundNormal = true;
			else
			{
				// Check cardinal directions
				for( int i = 0; i < 4; i++ )
				{
					switch( i )
					{
						// Check forward
						case 0:
							if ( Physics.Raycast( this.transform.position, _pCon.transform.forward, out _grndChk, _pCon.height ) )
								foundNormal = true;
							break;
						// Check backward
						case 1:
							if ( Physics.Raycast( this.transform.position, -_pCon.transform.forward, out _grndChk, _pCon.height ) )
								foundNormal = true;
							break;
						// Check right
						case 2:
							if ( Physics.Raycast( this.transform.position, _pCon.transform.right, out _grndChk, _pCon.height ) )
								foundNormal = true;
							break;
						// Check left
						case 3:
							if ( Physics.Raycast( this.transform.position, -_pCon.transform.right, out _grndChk, _pCon.height ) )
								foundNormal = true;
							break;
						// Something went wrong
						default:
							break;
					}

					// If we found a normal, break out of the for loop
					if ( foundNormal )
						break;
				}
			}

			if ( foundNormal )
				_slopeChk = Vector3.Angle( Vector3.up, _grndChk.normal );
			else
				_slopeChk = 0.0f;

			ProcessGravity();
			ProcessDeltas();

			_prevGrounded = _pCon.isGrounded;
		}

		protected virtual void ResetMoveVector()
		{
			_prevMove = _move;
			_move = Vector3.zero;
		}

		// Update is called once per frame
		protected override void Update()
		{
			if (Active)
			{
				UpdateMouse();

				if ( HubrisCore.Instance.Debug )
					UIManager.Instance.DevSetGrnd( _grndChk.normal );
			}
		}

		protected override void LateUpdate()
		{
			if (Active)
			{

			}
		}

		protected override void FixedUpdate()
		{
			if (Active)
			{
				ProcessState();

				if ( _move != Vector3.zero )
					_move = _move.normalized;
				else
				{
					if ( _prevMove != Vector3.zero )
						_move = _prevMove;
					else
						_move = _pCon.velocity.normalized;
					_move.y = 0.0f;	// Let ProcessGravity handle the y-value
				}

				_flags = _pCon.Move( ((_move * Speed) + _gravVector) * Time.fixedDeltaTime );

				ResetMoveVector();
			}
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			CleanUp();
		}
	}
}
