using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


namespace Hubris
{
    /// <summary>
    /// RTS specific implementation of the Hubris.GameManager class
    /// </summary>
    public class RTSGameManager : Hubris.GameManager
    {
        // Singleton instance
        private static RTSGameManager _inst = null;

        private static bool _disposing = false;
        private static object _lock = new object();

        new public static RTSGameManager Instance
        {
            get { if (!_disposing) { return _inst; } else { return null; } }
            set
            {
                lock (_lock) // Thread safety
                {
                    if (_inst == null || _disposing)  // Only set if _inst is already null or we're disposing of this instance
                    {
                        _inst = value;
                    }
                }
            }
        }

        // RTSGameManager instance vars
        [SerializeField]
        private RTSUnit _select = null;

        // RTSGameManager properties
        public bool CheckSelected
        {
            get { return (_select != null); }
        }

        public RTSUnit Selected
        {
            get { return _select; }
        }

        // GameManager methods
        void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Active = false;
                Destroy(this.gameObject);
            }

            if (Active)
            {
                _timer = 0.0f;
                _evList = new List<Event>();
            }
        }

        public void SetSelected(RTSUnit nUnit)
        {
            _select = nUnit;
            _select.OnSelect();
        }

        public void MoveSelected(Vector3 nPos)
        {
            if (CheckSelected)
            {
                Selected.SetTargetPos(nPos);
            }
        }

        public void Deselect()
        {
            if (CheckSelected)
            {
                _select.OnDeselect();
                _select = null;

                if (Debug)
                    UnityEngine.Debug.Log("Deselected the selected entity");
            }
        }

        void FixedUpdate()
        {
            _timer += Time.deltaTime;

            if (_timer > _tick)
            {
                _willCall = true;
                _timer = 0.0f;
            }

            if (_willCall)
            {
                OnTick(); // Broadcast Tick() event
            }
        }

        void Update()
        {

        }

        void LateUpdate()
        {
            if (_willCall)
            {
                _willCall = false;

                OnLateTick(); // Broadcast LateTick() event
            }
        }

        new public void Dispose()
        {
            Disposing = true;

            if (Instance == this)
                Instance = null;

            Destroy(this.gameObject);
        }
    }
}
