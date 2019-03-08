using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
    /// <summary>
    /// Class to derive RTS-style units, with animation, selection visuals, and generic behaviours
    /// </summary>
    public abstract class RTSUnit : Entity, ISelectable, IMovable
    {
        // RTSUnit instance vars
        [SerializeField]
        protected Animator _anim = null;
        [SerializeField]
        protected MeshRenderer _select = null;    // Object to visually show selection, change to desired type
        protected Vector3 _target = Vector3.zero; // Target position for Unit
        protected bool _moving = false;           // Is the unit currently moving?

        // RTSUnit properties
        public Vector3 TargetPos
        {
            get { return _target; }
        }

        public bool Moving
        {
            get { return _moving; }
        }

        // RTSUnit methods
        // Override virtual methods with desired functionality
        public virtual void OnSelect()
        {
            if (_select != null)
            {
                _select.enabled = true;
            }

            if (_anim != null)
            {
                _anim.SetTrigger("Select");
            }
        }

        public virtual void OnDeselect()
        {
            if (_select != null)
            {
                _select.enabled = false;
            }
        }

        public virtual void SetTargetPos(Vector3 nPos)
        {
            _target = nPos;
        }

        public virtual void CheckMove()
        {
            if (_target != Vector3.zero)
            {
                _moving = true;
            }
        }

        public virtual void CancelMove()
        {
            _target = Vector3.zero;
            _moving = false;
        }

    }
}
