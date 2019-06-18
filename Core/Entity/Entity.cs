using System;
using UnityEngine;

namespace Hubris
{
    /// <summary>
    /// Abstract class for deriving tangible in-game objects, with virtual ITickable and IActivatable implementation
    /// </summary>
    public abstract class Entity : MonoBehaviour, ITickable, IActivatable
    {
        ///--------------------------------------------------------------------
        /// Entity instance vars
        ///--------------------------------------------------------------------

        /*  We want a seperate Active boolean instance var, so we can enable/disable Hubris
         *  functionality without enabling/disabling corresponding GameObjects
         */

        [SerializeField]
        protected bool _act = true;
        [SerializeField]
        protected string _name;

        protected bool _disposed = false; // Has this Entity had Dispose() called?

        ///--------------------------------------------------------------------
        /// Entity properties
        ///--------------------------------------------------------------------

        public bool Active
        {
            get { return _act; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        ///--------------------------------------------------------------------
        /// Entity methods
        ///--------------------------------------------------------------------

        /// <summary>
        /// Enables Hubris functionality
        /// </summary>
        public void Activate()
        {
            _act = true;
        }

        /// <summary>
        /// Disables Hubris functionality
        /// </summary>
        public void Deactivate()
        {
            _act = false;
        }

        /// <summary>
        /// Set whether the Entity is active or not; virtual for unique functionality in derived classes
        /// </summary>
        public virtual void SetActive(bool nActive)
        {
            if (nActive)
                Activate();
            else
                Deactivate();
        }

        public virtual void OnEnable()
        {
            if (Name != null && Name.Length > 0)
            {
                // Set the GameObject name to match the Entity name
                this.gameObject.name = Name;
            }
            else
                Name = this.gameObject.name;

            /* IMPORTANT! */
            // Include SubTick() in all derived/overridden OnEnable() (or similar) methods for Hubris Tick-based behavior
            // and include UnsubTick() in CleanUp()
            SubTick();
        }

        protected virtual void SubTick()    // Subscribe to ITickable GameManager Actions
        {
            if (HubrisCore.Instance != null)
            {
                HubrisCore.Instance.AcTick += Tick;
                HubrisCore.Instance.AcLateTick += LateTick;
                HubrisCore.Instance.AcFixedTick += FixedTick;
            }
        }

        protected virtual void UnsubTick()  // Unsubscribe to ITickable GameManager Actions
        {
            if (HubrisCore.Instance != null)
            {
                HubrisCore.Instance.AcTick -= Tick;
                HubrisCore.Instance.AcLateTick -= LateTick;
                HubrisCore.Instance.AcFixedTick -= FixedTick;
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

        public virtual void OnDestroy()
        {
            CleanUp(true);
        }
    }
}
