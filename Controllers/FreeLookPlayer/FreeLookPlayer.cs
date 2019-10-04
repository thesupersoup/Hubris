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

		public override void OnEnable()
		{
			base.OnEnable();
		}

		protected override void Start()
		{
			InitFreeLookPlayer();
		}

		public void InitFreeLookPlayer()
		{
			if (Instance == this)
			{
				PlayerType = PType.FL;				

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
			// Implement escape behavior, like open a menu or step backward
		}

		/// <summary>
		/// Primary
		/// </summary>
		public override void Interact0()
		{

		}

		/// <summary>
		/// Alt 
		/// </summary>
		public override void Interact1()
		{

		}

		/// <summary>
		/// Tertiary
		/// </summary>
		public override void Interact2()
		{
			// Override for unique behavior
		}

		public override void Interact3()
		{
			// Override for unique behavior
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
