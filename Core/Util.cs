using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Hubris utility method class
	/// </summary>
	public static class Util
	{
		/// <summary>
		/// Check the distance between two points; returns distance as sqrMagnitude, take note!
		/// </summary>
		/// <returns>distance as float sqrMagnitude</returns>
		public static float CheckDistSqr( Vector3 pos1, Vector3 pos2 ) 
		{
			Vector3 offset = pos1 - pos2;
			float dist = offset.sqrMagnitude;

			return dist;
		}

		/// <summary>
		/// Check the distance between two points; returns true if the distance between them is less than or equal to the square of the distance argument
		/// </summary>
		public static bool CheckDistSqr( Vector3 pos1, Vector3 pos2, float dist )
		{
			return CheckDistSqr( pos1, pos2 ) <= GetSquare( dist );
		}

		/// <summary>
		/// Check if position A is closer to the source than position B
		/// </summary>
		public static bool IsCloser( Vector3 src, Vector3 posA, Vector3 posB )
		{
			return ( CheckDistSqr( src, posA ) < CheckDistSqr( src, posB ) );
		}

		/// <summary>
		/// Raycast from origin to target position and check
		/// </summary>
		public static bool RayCheckTarget( Vector3 origin, Vector3 dir, float dist, GameObject target, LayerMask mask )
		{
			if ( Physics.Raycast( origin, dir, out RaycastHit hit, dist, mask ) )
			{
				if ( hit.transform.root.gameObject != target )
					return false;

				return true;
			}

			return false;
		}

		/// <summary>
		/// Get the square of a floating point value; use this method for clarity of code and to prevent simple mistakes
		/// </summary>
		public static float GetSquare( float val )
		{
			return val * val;
		}
	}
}
