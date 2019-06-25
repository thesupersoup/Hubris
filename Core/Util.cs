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
		public static float CheckDistSqr(Vector3 pos1, Vector3 pos2) 
		{

			Vector3 offset = pos1 - pos2;
			float dist = offset.sqrMagnitude;

			return dist;
		}
	}
}
