using UnityEngine;

namespace Hubris
{
    public class HubrisBase
    {
        // Base methods
        public void Tick()
        {
            // Use with new definition in derived class
        }

        public void LateTick()
        {
            // Use with new definition in derived class
        }
    }

    public class HubrisCore : MonoBehaviour
    {
        // Singleton instance
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
                lock (_lock)
                {
                    if (_i == null)
                        _i = value;
                }
            }
        }

        // Core instance vars
        private static string _version = "v0.0.5";
        private static string _netLibType = "Telepathy.Client"; // Fully qualified networking class name
        private static string _netSendMethod = "Send";          // Method name to send data
        private static LocalConsole _con = null;    

        // Core properties
        public static string Version
        {
            get { return _version; }
            protected set { _version = value; }
        }

        public static string NetLibType
        {
            get { return _netLibType; }
            protected set { _netLibType = value; }
        }

        public static string NetSendMethod
        {
            get { return _netSendMethod; }
            protected set { _netSendMethod = value; }
        }

        public static LocalConsole Console
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

            _con = new LocalConsole();
            _con.Init();
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
