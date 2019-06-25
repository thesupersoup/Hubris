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

		private static bool _jumping = false;

		[Header("FPSPlayer variables")]
		[SerializeField]
		[Tooltip("Airborne speed acceleration per fixed update")]
		[Range(0.0f, MoveParams.MAX_RANGE)]
		protected float _airAccel = 0.15f;
		[SerializeField]
		[Tooltip("Airborne speed decay per fixed update")]
		[Range(0.0f, MoveParams.MAX_RANGE)]
		protected float _airDecay = 0.05f;
		[SerializeField]
		[Tooltip("Coefficient of speed lost when landing")]
		[Range(0.0f, MoveParams.MAX_RANGE)]
		protected float _speedLoss = 0.4f;
		[SerializeField]
		[Tooltip("Coefficient of stickiness when landing while still moving")]
		[Range(0.0f, MoveParams.MAX_RANGE)]
		protected float _stickiness = 0.55f;


		// Test variables
		[SerializeField]
		protected LayerMask _weaponMask = 0;

		///--------------------------------------------------------------------
		/// FPSPlayer properties
		///--------------------------------------------------------------------

		public bool Jumping
		{
			get { return _jumping; }
			protected set { _jumping = value; }
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

		///--------------------------------------------------------------------
		/// FPSPlayer methods
		///--------------------------------------------------------------------

		void OnEnable()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else if (Instance != null)
			{
				Deactivate();
				Destroy(this.gameObject);
			}
		}

		void Start()
		{
			if (Instance == this)
			{
				PlayerType = PType.FPS;

				base.Init();

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
					Activate();
					_mLook = new MouseLook(_gObj.transform, _pCam.transform, _sens, _sens, _mSmooth, _mSmoothAmt);
				}
			}
		}

		public override void Interact0()
		{
			if (PlayerInv.Slots[PlayerInv.ActiveIndex] != null && PlayerCam != null)
			{
				if (PlayerInv.Slots[PlayerInv.ActiveIndex] is WeaponBase nWeapon)
				{
					if (Physics.Raycast(PlayerCam.transform.position, PlayerCam.transform.forward, out RaycastHit hit, nWeapon.Range, _weaponMask))
					{
						GameObject target = hit.transform.root.gameObject;  // Grab root because hitboxes
						IDamageable ent = target.GetComponent<IDamageable>();

						if (ent != null)
						{
							ent.TakeDmg(nWeapon.DamageType, nWeapon.Damage, false);
						}
					}
				}
			}
		}

		public override void Interact1()
		{

		}

		public override void Move(InputManager.Axis ax, float val)
		{
			if (Active)
			{
				_moving = true;

				if (_fastMove)
					SpeedTarget = Movement.SpeedHigh;
				else
					SpeedTarget = Movement.SpeedLow;

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
						Debug.Log("DOT_THRESHOLD reached: " + dotAmt);
						Speed = 0.0f;
					}
				}

				_move += newMove;
			}
		}

		public override void SpecMove(SpecMoveType nType, InputManager.Axis ax, float val)
		{
			switch (nType)
			{
				case SpecMoveType.JUMP:

					if (IsGrounded)
					{
						if (_slopeChk < _pCon.slopeLimit)
						{
							// Move this flag to a Jump/Air state
							_jumping = true;
							SpeedTarget = Movement.SpeedLow;
							_canModSpdTgt = false;

							Move(ax, val);
						}
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
			if (Active)
			{
				if (!IsGrounded)
				{
					// Move this flag to a Jump/Air state
					_canModSpdTgt = false;
					_gravVector += Physics.gravity * Movement.GravFactor * Time.fixedDeltaTime;
				}
				else
				{
					_gravVector = Vector3.zero;

					// Move this flag to a Jump/Air state
					_canModSpdTgt = true;

					// If we were previously airborne
					if (!_prevGrounded)
					{
						if (!_moving)
						{
							Speed *= (MoveParams.MAX_RANGE - SpeedLoss);
						}
						else
						{
							Speed *= ((MoveParams.MAX_RANGE - SpeedLoss) * (MoveParams.MAX_RANGE - Stickiness));
						}
					}

					if (!_jumping)
					{
						if (_slopeChk < _pCon.slopeLimit)
						{
							_gravVector.y = -Movement.GravBase;
						}
						else
						{
							_gravVector += _grndChk.normal;
							_gravVector.y = 0.0f;
						}
					}
					else
					{
						_gravVector.y = Movement.JumpSpd;

						// Move this flag to a Jump/Air state
						_jumping = false;
					}
				}
			}
		}

		protected override void ProcessDeltas()
		{
			if (_moving)
			{
				// If no movement key is active
				if (!InputManager.Instance.MoveKey)
				{
					_moving = false;
					SpeedTarget = 0.0f;
				}

				// If the player's speed and speed target are both 0.0f
				if (Speed == SpeedTarget && SpeedTarget == 0.0f)
				{
					_moving = false;    // Ensure _moving is false
					_move = Vector3.zero;
					_prevMove = Vector3.zero;
					return;             // Quick return
				}
			}
			else
			{
				SpeedTarget = 0.0f; // If we're no longer moving, speed target should be 0.0f
			}

			if (Movement.UseAccel)
			{
				if (IsGrounded) // Use base acceleration and decay values when player is grounded
				{
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
			else    // If the player isn't using acceleration, speed immediately changes to target
			{
				Speed = SpeedTarget;
			}
		}

		protected override void ProcessState()
		{
			Physics.Raycast(this.transform.position, Vector3.down, out _grndChk, _pCon.height);
			_slopeChk = Vector3.Angle(Vector3.up, _grndChk.normal);

			ProcessGravity();
			ProcessDeltas();

			_prevGrounded = _pCon.isGrounded;
		}

		// Update is called once per frame
		protected override void Update()
		{
			if (Active)
			{
				base.Update();
				UpdateMouse();
			}
		}

		protected override void LateUpdate()
		{
			if (Active)
			{
				base.LateUpdate();
			}
		}

		protected override void FixedUpdate()
		{
			if (Active)
			{
				base.FixedUpdate();
				ProcessState();

				_move = _move.normalized;

				if (_slopeChk < _pCon.slopeLimit)
				{
					_flags = _pCon.Move(((_move * Speed) + _gravVector) * Time.fixedDeltaTime);
				}
				else
				{
					// _move += _gravVector.normalized; // (Adds rebound when jumping off slope at max speed)
					_move.y = 0.0f;     // No jump while falling down slope
					_flags = _pCon.Move(((_move * Speed) + (_gravVector * Movement.GravFactor)) * Time.fixedDeltaTime);
				}

				_prevMove = _move;
			}
		}
	}
}
