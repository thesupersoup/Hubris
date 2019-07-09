using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Enumerator for behavior branches
	/// </summary>
	public enum BhvEnum
	{
		IDLE = 0,
		MOVING,
		ALERT,
		WARY,
		HUNT,
		AGGRO,
		ATK,
		FLEE,
		ASLEEP,
		DEAD,
		NUM_BRANCHES
	}
}
