using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Empty behavior
	/// </summary>
	public class BNpcNone : BNpcBase
	{
		public static BNpcNone Instance = new BNpcNone();

		public override BhvStatus Invoke( BhvTree b, Npc a )
		{
			SetAnimTrigger( a, "Idle" );

			Debug.Log( $"Npc {a.gameObject.name} has invoked an empty behavior branch" );

			b.SetStatus( BhvStatus.SUCCESS );
			return b.Status;
		}
	}
}
