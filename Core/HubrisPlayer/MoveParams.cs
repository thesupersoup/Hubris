using System;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Collection of player movement parameters
	/// </summary>
	[Serializable]
	public class MoveParams
	{
		///--------------------------------------------------------------------
		/// Player movement  constants 
		///--------------------------------------------------------------------

		public const float DEF_SPD_LOW = 4.0f, DEF_SPD_HIGH = 8.0f,
							DEF_ACCEL_VAL = 0.35f, DEF_DECAY_VAL = 0.5f,
							DEF_GRAV_BASE = 10.0f, DEF_GRAV_FACTOR = 2.0f, DEF_JUMP_SPD = 10.0f;

		public const float MAX_RANGE = 1.0f;

		///--------------------------------------------------------------------
		/// Serialized instance variables for Editor
		///--------------------------------------------------------------------

		[Header("Movement parameters")]
		[SerializeField]
		[Tooltip("Low speed value (walking, etc.)")]
		private float _speedLow;
		[SerializeField]
		[Tooltip("High speed value (running, etc.)")]
		private float _speedHigh;
		[SerializeField]
		[Tooltip("Use speed acceleration and decay")]
		private bool _useAccel;
		[SerializeField]
		[Tooltip("Grounded speed acceleration per fixed update")]
		[Range(0.0f, MAX_RANGE)]
		private float _accel;
		[SerializeField]
		[Tooltip("Grounded speed decay per fixed update")]
		[Range(0.0f, MAX_RANGE)]
		private float _decay;
		[Header("Gravity and jump parameters")]
		[SerializeField]
		[Tooltip("Downward force when grounded")]
		private float _gravBase;
		[SerializeField]
		[Tooltip("Coefficient of Physics.gravity when airborne")]
		private float _gravFactor;
		[SerializeField]
		[Tooltip("Upward speed when jumping")]
		private float _jumpSpd;

		///--------------------------------------------------------------------
		/// Speed and acceleration properties
		///--------------------------------------------------------------------

		/// <summary>
		/// Low speed value (walking, etc.)
		/// </summary>
		public float SpeedLow { get { return _speedLow; } protected set { _speedLow = value; } }

		/// <summary>
		/// High speed value (running, etc.)
		/// </summary>
		public float SpeedHigh { get { return _speedHigh; } protected set { _speedHigh = value; } }

		/// <summary>
		/// Use speed acceleration and decay
		/// </summary>
		public bool UseAccel { get { return _useAccel; } protected set { _useAccel = value; } }

		/// <summary>
		/// Grounded speed acceleration per fixed update
		/// </summary>
		public float Accel { get { return _accel; } protected set { _accel = value; } }

		/// <summary>
		/// Grounded speed decay per fixed update
		/// </summary>
		public float Decay { get { return _decay; } protected set { _decay = value; } }

		/// <summary>
		/// Downward force when grounded
		/// </summary>
		public float GravBase { get { return _gravBase; } protected set { _gravBase = value; } }

		/// <summary>
		/// Coefficient of Physics.gravity when airborne
		/// </summary>
		public float GravFactor { get { return _gravFactor; } protected set { _gravFactor = value; } }

		/// <summary>
		/// Upward speed when jumping
		/// </summary>
		public float JumpSpd { get { return _jumpSpd; } protected set { _jumpSpd = value; } }

		///--------------------------------------------------------------------
		/// Set methods
		///--------------------------------------------------------------------
        
		/// <summary>
		/// [Speed, float] Should be positive
		/// </summary>
		public void SetSpeedLow(float nLow)
		{
			if (nLow >= 0.0f)
				SpeedLow = nLow;
		}

		/// <summary>
		/// [Speed, float] Should be positive
		/// </summary>
		public void SetSpeedHigh(float nHigh)
		{
			if (nHigh >= 0.0f)
				SpeedHigh = nHigh;
		}

		/// <summary>
		/// [bool] Set as flag
		/// </summary>
		public void SetUseAccel(bool nAccel)
		{
			UseAccel = nAccel;
		}

		/// <summary>
		/// [Coefficient, float] Should be positive
		/// </summary>
		public void SetAccel(float nAccel)
		{
			if(nAccel >= 0.0f)
				Accel = nAccel;
		}

		/// <summary>
		/// [Coefficient, float] Should be positive
		/// </summary>
		public void SetDecay(float nDecay)
		{
			if(nDecay >= 0.0f)
				Decay = nDecay;
		}

		/// <summary>
		/// [Coefficient, float] Can be any value
		/// </summary>
		public void SetGravBase(float nBase)
		{
			GravBase = nBase;
		}

		/// <summary>
		/// [Coefficient, float] Can be any value
		/// </summary>
		public void SetGravFactor(float nFactor)
		{
			GravFactor = nFactor;
		}

		/// <summary>
		/// [Speed, float] Can be any value
		/// </summary>
		public void SetJumpSpd(float nJump)
		{
			JumpSpd = nJump;
		}
	}
}
