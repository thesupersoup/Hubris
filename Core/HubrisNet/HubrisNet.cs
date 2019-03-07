using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

public class HubrisNet
{
    // Singleton instance
    private static UdpClient _udp;

    private static object _lock = new object();
    private static bool _disposing = false; // Check if we're in the process of disposing this singleton

    public static UdpClient Udp
    {
        get
        {
            if (_disposing)
                return null;
            else
                return _udp;
        }
        protected set
        {
            lock (_lock)
            {
                if (Udp == null)
                    _udp = value;
            }
        }
    }


}
