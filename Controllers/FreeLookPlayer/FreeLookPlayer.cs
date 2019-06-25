using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	public class FreeLookPlayer : HubrisPlayer
	{
		// FreeLookPlayer instance vars


		// FreeLookPlayer properties


		// FreeLookPlayer methods
		void OnEnable()
		{
			if (Instance == null)
				Instance = this;
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
				PlayerType = PType.FL;

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

		}

		public override void Interact1()
		{

		}

		public override void Move(InputManager.Axis ax, float val)
		{
			if (Active)
			{
				_move = GetMoveAsVector(ax, val, true);
			}
		}

		public override void SpecMove(SpecMoveType nType, InputManager.Axis ax, float val)
		{
			// Implementation here
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
						LocalConsole.Instance.LogError("FreeLookPlayer Rotate(): Invalid Axis specified", true);
				}
			}
		}

		protected override void ProcessState()
		{
			ProcessGravity();
			ProcessDeltas();
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

				_flags = _pCon.Move((_move * Speed * Time.fixedDeltaTime) + _gravVector);

				_move = Vector3.zero;
			}
		}
	}
}
