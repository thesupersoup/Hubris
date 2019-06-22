using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
    /// <summary>
    /// Class to derive RTS-style units, with animation, selection visuals, and generic behaviours
    /// </summary>
    public abstract class RTSUnit : Npc, IUsable, ISelectable, IPointClick
    {
        // RTSUnit instance vars
        [SerializeField]
        protected Animator _anim = null;
        [SerializeField]
        protected MeshRenderer _select = null;    // Object to visually show selection, change to desired type
        protected bool _moving = false;           // Is the unit currently moving?

        // RTSUnit properties
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

        public virtual void SetTarget(GameObject nObj)
        {
            TargetObj = nObj;
        }

        public virtual void SetMovePos(Vector3 nPos)
        {
            MovePos = nPos;
        }

        public virtual void CheckMove()
        {
            if (MovePos != Vector3.zero)
            {
                _moving = true;
            }
        }

        public virtual void CancelMove()
        {
            MovePos = Vector3.zero;
            _moving = false;
        }

        public virtual bool TryUse()
        {
            if (Active)
                return true;
            else
                return false;
        }

        public virtual void OnUse()
        {

        }
    }
}
