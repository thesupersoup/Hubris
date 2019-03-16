using System;
using Hubris;
using UnityEngine;

namespace Hubris
{
    /// <summary>
    /// Class which represents logical in-game objects, with virtual Tick() and LateTick() implementation
    /// </summary>
    public class LogicalEntity : IDisposable, ITickable
    {
        // LogicalEntity instance vars 

        /*  We want a seperate Active boolean instance var, so we can enable/disable Entities 
            without enabling/disabling corresponding GameObjects    */
        [SerializeField]
        protected bool _act = true;
        [SerializeField]
        protected string _name;

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

        // LogicalEntity methods
        protected virtual void SubTick()    // Subscribe to Tick/LateTick GameManager Actions
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AcTick += Tick;
                GameManager.Instance.AcLateTick += LateTick;
            }
        }

        protected virtual void UnsubTick()  // Unsubscribe to Tick/LateTick GameManager Actions
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AcTick -= Tick;
                GameManager.Instance.AcLateTick -= LateTick;
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

        public virtual void Dispose()
        {
            UnsubTick();
        }

        public virtual void OnDestroy()
        {
            Dispose();
        }
    }
}
