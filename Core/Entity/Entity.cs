﻿using UnityEngine;
using System.Collections;

namespace Hubris
{
    /// <summary>
    /// Abstract class for deriving tangible in-game objects, with virtual Tick() and LateTick() implementation
    /// </summary>
    public abstract class Entity : MonoBehaviour, ITickable
    {
        // Entity instance vars 

        /*  We want a seperate Active boolean instance var, so we can enable/disable Entities 
            without enabling/disabling corresponding GameObjects    */
        [SerializeField]
        protected bool _act = true;
        [SerializeField]
        protected string _name;

        // Entity properties
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

        // Entity methods
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
    }
}
