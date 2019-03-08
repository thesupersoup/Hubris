using UnityEngine;

namespace Hubris
{
    public class Core : MonoBehaviour
    {
        // Singleton instance
        private static Core _i = null;

        private static object _lock = new object();
        private static bool _disposing = false; // Check if we're in the process of disposing this singleton

        public static Core Instance
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
                lock (_lock)
                {
                    if (_i == null)
                        _i = value;
                }
            }
        }

        // Core instance vars
        [SerializeField]
        private string _version = "v0.0.8";
        [SerializeField]
        private string _netLibType = "Telepathy.Client";    // Fully qualified networking class name
        [SerializeField]
        private string _netSendMethod = "Send";             // Method name to send data
        [SerializeField]
        private bool _debug = false;

        private LocalConsole _con = new LocalConsole();     // "new LocalConsole()" required to prevent null errors

        // Core properties
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

        public LocalConsole Console
        {
            get { return _con; }
            protected set { _con = value; }
        }

        // Core methods
        void OnEnable()
        {
            if (Instance == null)
                Instance = this;
            else if(Instance != this)
            {
                Destroy(this.gameObject);
            }

            _con.Init();
        }

        public void VersionPrint()
        {
            Console.Log("Current Hubris Build: " + Core.Instance.Version);
        }

        public void NetInfoPrint()
        {
            Console.Log("NetLibType: " + Core.Instance.NetLibType);
            Console.Log("NetSendMethod: " + Core.Instance.NetSendMethod);
        }

        public void DebugToggle()
        {
            Debug = !Debug;
            Console.Log("Debug mode " + (Debug ? "activated" : "deactivated"));
        }

        void Update()
        {
            _con.Tick();
        }

        void LateUpdate()
        {
            _con.LateTick();
        }
    }

}
