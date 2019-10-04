using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hubris
{
	/// <summary>
	/// Provides basic RTS player functionality, to be used with an RTSGameManager
	/// </summary>
	public class RTSPlayer : HubrisPlayer
	{
		// RTSPlayer instance vars


		// RTSPlayer properties


		// RTSPlayer methods
		public override void OnEnable()
		{
			base.OnEnable();
		}

		protected override void Start()
		{
			InitRTSPlayer();
		}

		public void InitRTSPlayer()
		{ 
			if (Instance == this)
			{
				PlayerType = PType.RTS;

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
					_mLook.SetCursorLock(false);
				}
			}
		}

		public override void Escape()
		{
			// Implement escape behavior, like open a menu or step backward
		}

		public override void Interact0()   // For RTSPlayer, Interact0 is contextually select or move
		{
			if(_pCam != null)
			{
				RaycastHit hit;

				if(Physics.Raycast(_pCam.ScreenPointToRay(UnityEngine.Input.mousePosition), out hit))
				{
					if (!RTSGameManager.Instance.CheckSelected)
					{
						RTSUnit chkUnit = hit.collider.GetComponent<RTSUnit>();
						if (chkUnit != null)
						{
							RTSGameManager.Instance.SetSelected(chkUnit);
						}
					}
					else
					{

					}
				}
			}
		}

		public override void Interact1()   // For RTSPlayer, Interact1 is deselect
		{
			RTSGameManager.Instance.Deselect();
		}

		public override void Interact2()
		{
			
		}

		public override void Interact3()
		{
			// Override for unique behavior
		}

		// Basic Movement
		public override void Move(InputManager.Axis ax, float val)
		{
			if(Active)
			{
				_gObj.transform.Translate(GetMoveAsVector(ax, val * Speed, true), Space.World);
				// PhysForce(_move * _speed);
			}
		}

		// Movement with additional criteria or effects
		public override void SpecMove(SpecMoveType nType, InputManager.Axis ax, float val)
		{
			// Implement here
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
					if (HubrisCore.Instance.Debug)
						LocalConsole.Instance.LogError("RTSPlayer Rotate(): Invalid Axis specified", true);
				}
			}
		}

		protected override void ProcessState()
		{

		}

		protected override void Update()
		{
			if (Active)
			{
				base.Update();
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
			}
		}
	}
}
