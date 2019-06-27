using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Represents a sound event with a radius and intensity 
	/// </summary>
	public class SoundEvent
	{
		///--------------------------------------------------------------------
		/// Sound Event instance vars
		///--------------------------------------------------------------------

		[SerializeField]
		private GameObject _source;
		[SerializeField]
		private Vector3 _origin;
		[SerializeField]
		private float _radius;
		[SerializeField]
		private SoundIntensity _intensity;

		///--------------------------------------------------------------------
		/// Sound Event properties
		///--------------------------------------------------------------------

		public GameObject Source => _source;
		public Vector3 Origin => _origin;
		public float Radius => _radius;
		public SoundIntensity Intensity => _intensity;

		///--------------------------------------------------------------------
		/// Sound Event methods
		///--------------------------------------------------------------------

		public SoundEvent( GameObject src, Vector3 pos, float rad, SoundIntensity val = SoundIntensity.NORMAL )
		{
			_source = src;
			_origin = pos;
			_radius = rad;
			_intensity = val;
		}
	}
}
