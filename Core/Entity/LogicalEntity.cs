using System;
using Hubris;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Class which represents logical in-game objects, with virtual Tick() and LateTick() implementation
	/// </summary>
	public class LogicalEntity : IActivatable, ITickable
	{
		// LogicalEntity instance vars 

		/*  We want a seperate Active boolean instance var, so we can enable/disable Entities 
			without enabling/disabling corresponding GameObjects    */
		[SerializeField]
		protected bool _act = true;
		[SerializeField]
		protected string _name;

		protected bool _disposed = false; // Has this LogicalEntity had Dispose() called?

		// LogicalEntity properties
		public bool Active
		{
			get { return _act; }
			set { _act = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		///--------------------------------------------------------------------
		/// LogicalEntity methods
		///--------------------------------------------------------------------

		public virtual void Init()
		{
			/* IMPORTANT! */
			// Include SubTick() in all derived/overridden Init() (or similar) methods for Hubris Tick-based behavior
			// and include UnsubTick() in CleanUp()
			SubTick();
		}

		/// <summary>
		/// Enable or disable Hubris functionality; virtual for unique functionality in derived classes
		/// </summary>
		public virtual void SetHubrisActive(bool nActive)
		{
			_act = nActive;
		}

		/// <summary>
		/// Subscribe to Hubris Tick-based Actions
		/// Once subscribed, an object must ALWAYS unsubscribe in the process of being cleaned up or destroyed
		/// </summary>
		protected virtual void SubTick()
		{
			if ( HubrisCore.Instance != null )
			{
				HubrisCore.Instance.AcTick += Tick;
				HubrisCore.Instance.AcLateTick += LateTick;
				HubrisCore.Instance.AcFixedTick += FixedTick;
				HubrisCore.Instance.AcCleanUp += CleanUp;
			}
		}

		/// <summary>
		/// Unsubscribe to Hubris Tick-based Actions
		/// Once subscribed, an object must ALWAYS unsubscribe in the process of being cleaned up or destroyed
		/// </summary>
		protected virtual void UnsubTick()
		{
			if ( HubrisCore.Instance != null )
			{
				HubrisCore.Instance.AcTick -= Tick;
				HubrisCore.Instance.AcLateTick -= LateTick;
				HubrisCore.Instance.AcFixedTick -= FixedTick;
				HubrisCore.Instance.AcCleanUp -= CleanUp;
			}
		}

		public virtual void Tick()
		{
			// To be called in response to GameManager event
			// Override in derived class with unique implementation
		}

		public virtual void LateTick()
		{
			// To be called in response to GameManager event
			// Override in derived class with unique implementation
		}

		public virtual void FixedTick()
		{
			// To be called in response to GameManager event
			// Override in derived class with unique implementation
		}

		public virtual void CleanUp(bool full = true)
		{
			if (!this._disposed)
			{
				if (full)
				{
					_act = false;
					_name = null;
				}

				UnsubTick();    // Need to Unsubscribe from Tick Event to prevent errors
				_disposed = true;
			}
		}
	}
}
