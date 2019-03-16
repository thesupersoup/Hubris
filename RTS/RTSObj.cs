using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
    /// <summary>
    /// Abstract class for deriving in-game objects that can be selected and used with access points
    /// </summary>
    public abstract class RTSObj : Entity, ISelectable, IUsable
    {
        // RTSObj instance vars
        [SerializeField]
        protected Mesh _model = null;
        [SerializeField]
        protected Material[] _matArr = null;
        [SerializeField]
        protected SkinnedMeshRenderer _mRen = null;
        [SerializeField]
        protected GameObject _ap;         // Access point, where a unit needs to stand to use the object

        // RTSObj properties
        public Mesh Model
        {
            get { return _model; }
            set { _model = value; }
        }

        public Material[] Materials
        {
            get { return _matArr; }
            set { _matArr = value; }
        }

        public SkinnedMeshRenderer MRenderer
        {
            get { return _mRen; }
            set { _mRen = value; }
        }

        public Vector3 AccPnt
        {
            get { if (_ap != null) { return _ap.transform.position; } else { return this.transform.position; } }
        }

        // PGameObjectVis methods
        protected virtual void SetModel()
        {
            if (MRenderer != null)
            {
                MRenderer.sharedMesh = Model;
                MRenderer.materials = Materials;
            }
        }

        public abstract void DrawSelectionEffect();
        public abstract void OnSelect();
        public abstract void OnDeselect();
        public abstract bool TryUse();
        public abstract void OnUse();
    }
}
