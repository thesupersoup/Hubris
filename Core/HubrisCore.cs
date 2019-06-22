using System;
using UnityEngine;

namespace Hubris
{
    public class HubrisCore : MonoBehaviour
    {
        ///--------------------------------------------------------------------
        /// HubrisCore singleton instance
        ///--------------------------------------------------------------------

        private static HubrisCore _i = null;

        private static object _lock = new object();
        private static bool _disposing = false; // Check if we're in the process of disposing this singleton

        public static HubrisCore Instance
        {
            get
            {
                if (_disposing)
                    return null;
                else
                    return _i;
            }

            protected set
            {
                lock (_lock) // Thread safety
                {
                    if ((_i == null && !_disposing) || (_i != null && value == null))  // Only set if _i is already null or we're disposing of this instance
                    {
                        _i = value;
                    }
                }
            }
        }

        ///--------------------------------------------------------------------
        /// HubrisCore instance vars
        ///--------------------------------------------------------------------

        [SerializeField]
        private string _version = "v0.1.0";
        [SerializeField]
        private string _netLibType = "Telepathy.Client";    // Fully qualified networking class name
        [SerializeField]
        private string _netSendMethod = "Send";             // Method name to send data
        [SerializeField]
        private bool _debug = false;
        [SerializeField]
        protected float _tick = 1.5f;     // Tick time in seconds
        protected float _timer;
        protected bool _willCall = false; // Will call Tick() and LateTick() next Update and LateUpdate(), respectively
        [SerializeField]
        [Tooltip("Template prefab for instanting UI GameObject")]
        private GameObject _ui = null;

        [SerializeField]
        private GameManager _gm = new GameManager();        // "new GameManager()" required to prevent null errors

        [SerializeField]
        private LocalConsole _con = new LocalConsole();     // "new LocalConsole()" required to prevent null errors

        [SerializeField]
        private HubrisNet _net;

        ///--------------------------------------------------------------------
        /// HubrisCore actions
        ///--------------------------------------------------------------------

        public Action AcTick;
        public Action AcLateTick;
        public Action AcFixedTick;

        ///--------------------------------------------------------------------
        /// HubrisCore properties
        ///--------------------------------------------------------------------

        public string Version
        {
            get { return _version; }
            protected set { _version = value; }
        }

        public string NetLibType
        {
            get { return _netLibType; }
            protected set { _netLibType = value; }
        }

        public string NetSendMethod
        {
            get { return _netSendMethod; }
            protected set { _netSendMethod = value; }
        }

        public bool Debug
        {
            get { return _debug; }
            protected set { _debug = value; }
        }

        public float TickTime   // Tick time in seconds
        {
            get { return _tick; }
        }

        public GameManager GM
        {
            get { return _gm; }
        }

        public LocalConsole Console
        {
            get { return _con; }
        }

        public HubrisNet Network
        {
            get { return _net; }
        }

        ///--------------------------------------------------------------------
        /// HubrisCore methods
        ///--------------------------------------------------------------------

        void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                // Enforce Singleton pattern 
                Destroy(this.gameObject);
                return;
            }

            if (Instance == this)
            {
                // Initialize Networking
                if(_net == null)
                {
                    GetComponent<HubrisNet>();

                    if(_net == null)
                    {
                        this.gameObject.AddComponent<HubrisNet>();
                    }
                }

                // Initialize UI object
                if (_ui != null)
                {
                    GameObject temp = Instantiate(_ui);
                    temp.name = _ui.name;   // None of that "(Clone)" nonsense
                }

                _timer = 0.0f;

                // Initialize GameManager and LocalConsole
                _gm.Init();
                _con.Init();
            }
        }

        public void VersionPrint()
        {
            Console.Log("Current Hubris Build: " + Version);
        }

        public void NetInfoPrint()
        {
            Console.Log("NetLibType: " + NetLibType);
            Console.Log("NetSendMethod: " + NetSendMethod);
        }

        public void DebugToggle()
        {
            Debug = !Debug;

            Console.Log("Debug mode " + (Debug ? "activated" : "deactivated"));

            if(UIManager.Instance != null)
            {
                UIManager.Instance.DevSet(Debug);
            }
        }

        void FixedUpdate()
        {
            OnFixedTick();

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

                OnLateTick();       // Broadcast LateTick() event
            }
        }

        protected virtual void OnTick()
        {
            AcTick?.Invoke();       // Null-conditional operator for pre-invocation null check
        }

        protected virtual void OnLateTick()
        {
            AcLateTick?.Invoke();   // Null-conditional operator for pre-invocation null check
        }

        protected virtual void OnFixedTick()
        {
            AcFixedTick?.Invoke();  // Null-conditional operator for pre-invocation null check
        }

        void OnDestroy()
        {
            if(_net != null)
            {
                _net.CleanUp();
            }

            if(_gm != null)
            {
                _gm.CleanUp();
            }

            if(_con != null)
            {
                _con.CleanUp();
            }
        }
    }

}
