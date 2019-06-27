using System;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Represents an NPCs field-of-view cone
	/// </summary>
	public sealed class FOV
	{
		// Pre-baked constants for common unit circle sin(x)/cos(x) values
		private const float SQRT_TWO_OVER_TWO = 0.7071067812f,
							SQRT_THREE_OVER_TWO = 0.8660254038f;

		///--------------------------------------------------------------------
		/// FOV instance vars
		///--------------------------------------------------------------------

		private FOVDegrees _deg;
		private float _x;
		private float _y;
		private Vector3 _lVect, _rVect;

		///--------------------------------------------------------------------
		/// FOV properties
		///--------------------------------------------------------------------

		public FOVDegrees Degrees { get => _deg; set { _deg = value; UpdateTrigValues( _deg, ref _x, ref _y );} }
		public float X { get => _x; set => _x = value; }
		public float Y { get => _y; set => _y = value; }
		public Vector3 LeftVect { get => _lVect; }
		public Vector3 RightVect { get => _rVect; }

		///--------------------------------------------------------------------
		/// FOV methods
		///--------------------------------------------------------------------

		public FOV( Vector3 src, FOVDegrees nDeg = FOVDegrees._90)
		{
			Degrees = nDeg;
			UpdateTrigValues( Degrees, ref _x, ref _y );
			UpdateVectors( src );
		}

		/// <summary>
		/// Updates the FOV left and right vectors relative to the source vector parameter
		/// </summary>
		public void UpdateVectors( Vector3 src )
		{
			_lVect = GetVectorLeft( src );
			_rVect = GetVectorRight( src );
		}

		/// <summary>
		/// Returns whether the target position is within the field-of-view
		/// </summary>
		public bool IsInView( Vector3 src, Vector3 chk )
		{
			return (Vector3.Angle( src, chk ) <= Vector3.Angle( src, LeftVect ) || Vector3.Angle( src, chk ) <= Vector3.Angle( src, RightVect ));
		}

		private Vector3 GetVectorLeft( Vector3 src )
		{
			return new Vector3( (src.x * X - src.z * Y), 0.0f, (src.x * Y + src.z * X) );
		}

		private Vector3 GetVectorRight( Vector3 src )
		{
			return new Vector3( (src.x * X - src.z * -Y), 0.0f, (src.x * -Y + src.z * X) );
		}

		private void UpdateTrigValues( FOVDegrees deg, ref float x, ref float y )
		{
			switch ( deg )
			{
				case FOVDegrees._0:     // 0
					x = 1.0f;
					y = 0.0f;
					break;
				case FOVDegrees._60:    // Pi over 6
					x = SQRT_THREE_OVER_TWO;
					y = 0.5f;
					break;
				case FOVDegrees._90:    // Pi over 4
					x = SQRT_TWO_OVER_TWO;
					y = SQRT_TWO_OVER_TWO;
					break;
				case FOVDegrees._120:    // Pi over 3
					x = 0.5f;
					y = SQRT_THREE_OVER_TWO;
					break;
				case FOVDegrees._180:    // Pi over 2
					x = 0.0f;
					y = 1.0f;
					break;
				default:
					goto case FOVDegrees._90;   // Default to 90 degrees
			}
		}
	}
}
