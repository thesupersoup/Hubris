using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Represents a destination to satisfy an Objective
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class Destination : InterestPoint
	{
		///--------------------------------------------------------------------
		/// Destination instance vars
		///--------------------------------------------------------------------

		[SerializeField]
		private Collider _trigger = null;

		///--------------------------------------------------------------------
		/// Destination properties
		///--------------------------------------------------------------------  

		public Collider Trigger
		{
			get { return _trigger; }
		}

		///--------------------------------------------------------------------
		/// Destination methods
		///--------------------------------------------------------------------

		void OnTriggerStay(Collider other)
		{
			if (Active)
			{
				LiveEntity ent = other.gameObject.GetComponent<LiveEntity>();
				if (ent != null)
				{
					if (ent.EntType == EntityType.PLAYER)
					{
						NotifyObservers(true);
						Deactivate();
					}
				}
			}
		}
	}
}
