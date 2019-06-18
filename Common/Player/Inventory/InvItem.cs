using System;
using UnityEngine;

namespace Hubris
{
    /// <summary>
    /// Represents any item that can interact with the Inventory system
    /// </summary>
    [Serializable]
    public class InvItem : ScriptableObject, IItem
    {
        [SerializeField]
        private float _weight;

        public float Weight { get { return _weight; } protected set { _weight = value; } }

        public virtual void Interact0()
        {
            // Override with unique implementation
        }

        public virtual void Interact1()
        {
            // Override with unique implementation
        }
    }
}
