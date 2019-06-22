using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hubris;

namespace Hubris
{
	/// <summary>
	/// Manages all client-side player visual effects, like screen shake
	/// </summary>
	public class PlayerEffects
	{
		///--------------------------------------------------------------------
		/// PlayerEffects instance vars
		///--------------------------------------------------------------------

		private Camera _pCam = null;
		private float _shakeAmt = 0.0f;

		///--------------------------------------------------------------------
		/// PlayerEffects properties
		///--------------------------------------------------------------------

		public Camera PlayerCam
		{ get { return _pCam; } protected set { _pCam = value; } }

		public float ShakeAmt
		{ get { return _shakeAmt; } protected set { _shakeAmt = value; } }

		///--------------------------------------------------------------------
		/// PlayerEffects methods
		///--------------------------------------------------------------------

		public PlayerEffects(Camera nCam)
		{
			PlayerCam = nCam;
		}

		public void SetShakeAmt(float nAmt)
		{
			ShakeAmt = nAmt;
		}
	}
}
