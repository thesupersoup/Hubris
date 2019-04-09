using UnityEngine;
using System.Collections;

namespace Hubris
{
    /// <summary>
    /// Abstract class for deriving logical or real in-game objects, with optional Tick() and LateTick() implementation
    /// </summary>
    public abstract class Entity : MonoBehaviour, ITickable
    {
        // PGameObject instance vars 
        [SerializeField]
        protected bool _act = true;
        [SerializeField]
        protected string _name;

        // PGameObject properties
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

        // GameObject methods
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
