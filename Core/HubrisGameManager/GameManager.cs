using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


namespace Hubris
{
    /// <summary>
    /// Manages high level game logic, including calling Tick() and LateTick() methods on Tickables subscribed to the defined Actions
    /// </summary>
    public class GameManager : Entity
    {
        // Singleton instance
        private static GameManager _inst = null;

        private static bool _disposing = false;
        private static object _lock = new object();

        public static GameManager Instance
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

        // GameManager instance vars
        [SerializeField]
        protected bool _debug = false;
        [SerializeField]
        protected float _tick = 1.5f;     // Tick time in seconds
        protected float _timer;
        protected bool _willCall = false; // Will call Tick() and LateTick() next Update and LateUpdate(), respectively
        protected List<Event> _evList = null;

        // GameManager actions
        public Action AcTick;
        public Action AcLateTick;

        // GameManager properties
        public bool Debug
        {
            get { return _debug; }
            protected set { _debug = value; }
        }

        public bool Disposing
        {
            get { return _disposing; }
            protected set { _disposing = value; }
        }

        public float TickTime   // Tick time in seconds;
        {
            get { return _tick; }
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

        public bool ToggleDebug()
        {
            Debug = !Debug;
            return Debug;   // Return the current state of Debug
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
                _willCall = false;  // Set back to false here in LateUpdate, after Update is finished

                OnLateTick(); // Broadcast LateTick() event
            }
        }

        protected virtual void OnTick()
        {
            AcTick();
        }

        protected virtual void OnLateTick()
        {
            AcLateTick();
        }

        protected virtual void Dispose()
        {
            Disposing = true;

            if (Instance == this)
                Instance = null;

            Destroy(this.gameObject);
        }
    }
}
