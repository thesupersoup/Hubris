using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Represents a location for an Entity to spawn. Self-reports to the GameManager.
	/// </summary>
	[DisallowMultipleComponent]
	public class SpawnPoint : MonoBehaviour
	{
		private bool _reported = false;

		public bool Reported => _reported;

		void OnEnable()
		{
			Report();
		}

		// Start is called before the first frame update
		void Start()
		{
			if ( !Reported )
				Report();
		}

		void Report()
		{
			if ( HubrisCore.Instance != null )
			{
				HubrisCore.Instance.GM.SpawnPts.Add( this );
				_reported = true;
			}
		}
	}
}
