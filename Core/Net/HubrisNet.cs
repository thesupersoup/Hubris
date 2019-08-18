using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hubris
{
	public class HubrisNet : MonoBehaviour
	{
		public const int SUBHEADER_SIZE = 4;    // Subheader size in bytes
		public const char DELIM = '&';          // Delimiter for data
		public const int TYPE_INDEX = 0;        // Position in the subheader for type info
		public const int SIZE_INDEX = 1;        // Number of data elements within the packet
		// RESERVED_INDEX = 2;
		// RESERVED_INDEX = 3;

		public const int MAX_PACKET_SIZE = 64000;

		public enum MsgType : byte
		{
			HANDSHAKE = 0,
			STATUS = 1,
			MSG = 2,
			POS = 3,
			STATE = 4,
			SESSION = 5,
			MAX_VAL = 127   // Max for a signed byte, use as max MsgType for ease of writing multiplatform software
		}

		// Singleton instance
		private static HubrisNet _i = null;

		private static object _lock = new object();
		private static bool _disposing = false;

		public static HubrisNet Instance
		{
			get
			{ return _i; }
			protected set 
			{
				lock (_lock)
				{
					if (_i == null)
						_i = value;
					else if (_disposing)
						_i = null;
				}
			}
		}

		private NetState _state = NetClosed.Instance;

		[SerializeField]
		private bool _ready;
		[SerializeField]
		private bool _isServer = false;
		[SerializeField]
		private bool _local = false;
		[SerializeField]
		private bool _connected;
		[SerializeField]
		private string _session;
		private UdpClient _udpClient = null;
		private TcpClient _tcpClient = null;
		private TcpListener _tcpListen;
		private Socket _udpSock, _tcpSock;
		private Coroutine _udpCo, _tcpCo, _serverCo;
		private NetworkStream _udpStream = null, _tcpStream = null;
		private byte[] _udpSend = new byte[MAX_PACKET_SIZE], 
						_tcpSend = new byte[MAX_PACKET_SIZE], 
						_udpRead = new byte[MAX_PACKET_SIZE],
						_tcpRead = new byte[MAX_PACKET_SIZE];

		private IPAddress _localIp;
		private IPAddress _remoteIp;
		[SerializeField]
		private PortPool _udpPortPool = new PortPool(new int[]
		{ 27700, 27701, 27702, 27703, 27704, 27705, 27706, 27707, 27708, 27709, 27710, 27711, 27712, 27713, 27714, 27715 });
		private PortPool _tcpPortPool = new PortPool(new int[]
		{ 27800, 27801, 27802, 27803, 27804, 27805, 27806, 27807, 27808, 27809, 27810, 27811, 27812, 27813, 27814, 27815 });
		[SerializeField]
		private int _localUdpPort;
		[SerializeField]
		private int _localTcpPort;
		[SerializeField]
		private int _remoteUdpPort = 27770;
		[SerializeField]
		private int _remoteTcpPort = 27870;

		#region Properties
		public NetState State
		{
			get { return _state; }
			protected set { _state = value; }
		}

		public bool Ready
		{
			get
			{ return _ready; }
			protected set
			{ _ready = value; }
		}

		public bool IsServer
		{
			get
			{ return _isServer; }
			protected set
			{ _isServer = value; }
		}
        
		public bool Local
		{
			get
			{ return _local; }
			protected set
			{ _local = value; }
		}

		public bool Connected
		{
			get
			{ return _connected; }
			protected set
			{ _connected = value; }
		}

		public string SessionId {  get { return _session; } }

		public UdpClient Udp
		{
			get
			{
				if (_disposing)
					return null;
				else
					return _udpClient;
			}
			protected set
			{ _udpClient = value; }
		}

		public TcpClient Tcp
		{
			get
			{
				if (_disposing)
					return null;
				else
					return _tcpClient;
			}
			protected set
			{ _tcpClient = value; }
		}

		public TcpListener TcpListen
		{
			get
			{
				if (_disposing)
					return null;
				else
					return _tcpListen;
			}
			protected set
			{ _tcpListen = value; }
		}

		public Socket UdpSock
		{
			get
			{ return _udpSock; }
			protected set
			{ _udpSock = value; }
		}

		public Socket TcpSock
		{
			get
			{ return _tcpSock; }
			protected set
			{ _tcpSock = value; }
		}

		public NetworkStream UdpStream
		{
			get
			{ return _udpStream; }
			protected set
			{ _udpStream = value; }
		}

		public NetworkStream TcpStream
		{
			get
			{ return _tcpStream; }
			protected set
			{ _tcpStream = value; }
		}

		public IPAddress LocalIp
		{
			get
			{ return _localIp; }
			protected set
			{ _localIp = value; }
		}

		public IPAddress RemoteIp
		{
			get
			{ return _remoteIp; }
			protected set
			{ _remoteIp = value; }
		}

		public PortPool UdpPortPool
		{
			get
			{ return _udpPortPool; }
		}

		public PortPool TcpPortPool
		{
			get
			{ return _tcpPortPool; }
		}

		public int LocalUdpPort
		{
			get
			{ return _localUdpPort; }
			protected set
			{ _localUdpPort = value; }
		}

		public int LocalTcpPort
		{
			get
			{ return _localTcpPort; }
			protected set
			{ _localTcpPort = value; }
		}

		public int RemoteUdpPort
		{
			get
			{ return _remoteUdpPort; }
			protected set
			{ _remoteUdpPort = value; }
		}

		public int RemoteTcpPort
		{
			get
			{ return _remoteTcpPort; }
			protected set
			{ _remoteTcpPort = value; }
		}

		public byte[] UdpSend
		{
			get
			{ return _udpSend; }
			protected set
			{ _udpSend = value; }
		}

		public byte[] TcpSend
		{
			get
			{ return _tcpSend; }
			protected set
			{ _tcpSend = value; }
		}

		public byte[] UdpRead
		{
			get
			{ return _udpRead; }
			protected set
			{ _udpRead = value; }
		}

		public byte[] TcpRead
		{
			get
			{ return _tcpRead; }
			protected set
			{ _tcpRead = value; }
		}
		#endregion Properties


		public void OnEnable()
		{
			if (Instance == null)
				Instance = this;
			else
				CleanUp();

			LocalUdpPort = UdpPortPool.GetPort();
			Udp = new UdpClient(LocalUdpPort);

			if (!Local) // Connect to Google DNS to find favored IPEndPoint fast and dirty
				Udp.Connect("8.8.8.8", RemoteUdpPort);
			else        // Establish a connection to localhost
				Udp.Connect("127.0.0.1", RemoteUdpPort);

			UdpSock = Udp.Client;

			if (UdpSock != null)
			{
				IPEndPoint ep = (IPEndPoint)UdpSock.LocalEndPoint;
				LocalIp = ep.Address;
				Ready = true;

				if (IsServer)
				{
					IPEndPoint server = new IPEndPoint(LocalIp, RemoteTcpPort);
					TcpListen = new TcpListener(server);
					_serverCo = StartCoroutine(ServerRoutine());
				}

				// _udpCo = StartCoroutine(ClientUdpRoutine());
				// _tcpCo = StartCoroutine(ClientTcpRoutine());
			}
		}

		public bool ConnectToRemote(string nIp)
		{
			if (IPAddress.TryParse(nIp, out IPAddress ip))
			{
				if (Connected)
					Disconnect();

				// Connect with Tcp first for the handshake
				// Tcp will be used for all KeepAlive and heartbeat communications
				LocalTcpPort = TcpPortPool.GetPort();
				Tcp = new TcpClient(new IPEndPoint(LocalIp, LocalTcpPort));
				LocalUdpPort = UdpPortPool.GetPort();
				Udp = new UdpClient(LocalUdpPort);

				// Set TCP connection to immediately close 
				LingerOption closeImmediate = new LingerOption(true, 0);
				Tcp.LingerState = closeImmediate;
				Tcp.Connect(ip, RemoteTcpPort);

				if(Tcp.Connected)
				{
					RemoteIp = ip;
					TcpStream = new NetworkStream(Tcp.Client);

					// No actual active connection with Udp, but we get the party started
					Udp.Connect(RemoteIp, RemoteUdpPort);
					Udp.BeginReceive(new AsyncCallback(ReceiveUdp), null);

					// Prepare to receive Tcp data
					StartTcpRoutine();

					Connected = true;

					// We have liftoff
					return true;
				}
				else
				{
					LocalConsole.Instance.Log("Could not connect to " + ip);
					return false;
				}
			}
			else
			{
				LocalConsole.Instance.Log("Unable to parse IP [" + nIp + "]");
				return false;
			}
		}

		/// <summary>
		/// Close all connections and stop all coroutines
		/// </summary>
		public void Disconnect()
		{
			if (Connected)
			{
				// Null check to prevent errors on closing application without disconnecting
				if(Tcp != null && Tcp.Connected)
					Tcp.Close();

				if (_tcpCo != null)
				{
					StopCoroutine(_tcpCo);
					_tcpCo = null;
				}

				// Null check to prevent errors on closing application without disconnecting
				if (Udp != null)
					Udp.Close();

				Connected = false;
			}
			else
				LocalConsole.Instance.Log("Not currently connected");
		}

		/// <summary>
		/// Send a message to an address via UDP
		/// </summary>
		/// <returns>Returns client-side success as bool</returns>
		public bool SendMsgUdp(string nMsg, MsgType nType = MsgType.MSG)
		{
			if (Ready)
			{
				if (Connected)
				{ 
					byte[] arr = Encoding.ASCII.GetBytes(nMsg);
					UdpSend = AssemblePacket(arr, nType);

					if (UdpSend != null && UdpSend.Length > 0)
					{
						int numBytes = Udp.Send(UdpSend, UdpSend.Length);
						// LocalConsole.Instance.Log("Sent " + numBytes + " bytes");
						return true;
					}
					else
					{
						LocalConsole.Instance.Log("Invalid message data to send");
						return false;
					}
				}
				else
				{
					LocalConsole.Instance.Log("Not currently connected to a server");
					return false;
				}
			}
			else
			{
				LocalConsole.Instance.Log("HubrisNet is not ready");
				return false;
			}
		}

		/// <summary>
		/// Send a message to an address via TCP
		/// </summary>
		/// <returns>Returns client-side success as bool</returns>
		public bool SendMsgTcp(string nMsg, MsgType nType = MsgType.MSG)
		{
			if (Ready)
			{
				if (Connected)
				{
					TcpSend = AssemblePacket(Encoding.ASCII.GetBytes(nMsg), nType);

					if (TcpSend != null && TcpSend.Length > 0)
					{
						TcpStream.Write(TcpSend, 0, TcpSend.Length);
						LocalConsole.Instance.Log("Sent " + nMsg + " (length:" + TcpSend.Length + ")", true);
						return true;
					}
					else
					{
						LocalConsole.Instance.Log("Invalid message data to send");
						return false;
					}
				}
				else
				{
					LocalConsole.Instance.Log("Not currently connected to a server");
					return false;
				}
			}
			else
			{
				LocalConsole.Instance.Log("HubrisNet is not ready");
				return false;
			}
		}

		private void ReceiveTcp(IAsyncResult packet)
		{

		}

		private void ReceiveUdp(IAsyncResult packet)
		{
			IPEndPoint remote = (IPEndPoint)Udp.Client.RemoteEndPoint;
			byte[] received = Udp.EndReceive(packet, ref remote);
			LocalConsole.Instance.Log("Received a packet", true);
			MsgType type = GetMsgType(received);
			string msg;

			LocalConsole.Instance.Log("type is " + type, true);
			LocalConsole.Instance.Log("msg is " + Encoding.ASCII.GetString(ScrubSubheader(received)), true);

			switch (type)
			{
				case MsgType.POS:
				case MsgType.STATE:
				case MsgType.STATUS:
					msg = Encoding.ASCII.GetString(ScrubSubheader(received));
					if (msg.Equals("0"))
					{
						LocalConsole.Instance.Log("Server reporting shutdown, disconnecting...", true);
						Disconnect();
					}
					break;
				case MsgType.MSG:   // Default to MSG
				default:
					msg = Encoding.ASCII.GetString(ScrubSubheader(received));
					LocalConsole.Instance.Log("Received [" + type + "] " + msg + " from " + remote.Address + " on port " + remote.Port);
					break;
			}
		}

		public void ChangeState(HubrisNet net, NetState nState)
		{
			if (net == this)
			{
				State = nState;
			}
		}

		public static byte[] ScrubSubheader(byte[] arr)
		{
			byte[] scrubbed = new byte[arr.Length - SUBHEADER_SIZE];
			for (int i = 0; i < scrubbed.Length; i++)
				scrubbed[i] = arr[i + SUBHEADER_SIZE];
			return scrubbed;
		}

		public static byte[] AssemblePacket(byte[] arr, MsgType type)
		{
			// Build the subheader
			byte[] subheader = new byte[SUBHEADER_SIZE];
			subheader[TYPE_INDEX] = (byte)type;

			byte[] packet = new byte[arr.Length + SUBHEADER_SIZE];

			for (int i = 0; i < packet.Length; i++)
			{
				if (i >= SUBHEADER_SIZE)
					packet[i] = arr[i - SUBHEADER_SIZE];
				else
					packet[i] = subheader[i];
			}

			return packet;
		}

		public static MsgType GetMsgType(byte[] arr)
		{
			byte[] subheader = new byte[SUBHEADER_SIZE];

			for (int i = 0; i < SUBHEADER_SIZE; i++)
				subheader[i] = arr[i];

			MsgType mtType;

			switch(subheader[TYPE_INDEX])
			{
				case (byte)MsgType.HANDSHAKE:
					mtType = MsgType.HANDSHAKE;
					break;
				case (byte)MsgType.STATUS:
					mtType = MsgType.STATUS;
					break;
				case (byte)MsgType.POS:
					mtType = MsgType.POS;
					break;
				case (byte)MsgType.STATE:
					mtType = MsgType.STATE;
					break;
				case (byte)MsgType.SESSION:
					mtType = MsgType.SESSION;
					break;
				case (byte)MsgType.MSG:  // Default to MSG
				default:
					mtType = MsgType.MSG;
					break;
			}

			return mtType;
		}

		public void StartTcpRoutine()
		{
			// Start the Coroutine to read data from the socket
			_tcpCo = StartCoroutine(ClientTcpRoutine());
		}

		IEnumerator ClientTcpRoutine()
		{
			bool run = true;

			if (Tcp == null || !Tcp.Connected)
				yield return null;

			TcpStream = new NetworkStream(Tcp.Client);
			IPEndPoint remote = (IPEndPoint)Tcp.Client.RemoteEndPoint;
			int dataLen;
			string msg;
			MsgType type;

			// Function like a thread; run until terminated
			while (run)
			{
				try
				{
					if (TcpStream.DataAvailable)
					{
						dataLen = TcpStream.Read(TcpRead, 0, TcpRead.Length);
						byte[] tempBuffer = new byte[dataLen];

						for (int i = 0; i < tempBuffer.Length; i++)
							tempBuffer[i] = TcpRead[i + SUBHEADER_SIZE];

						msg = Encoding.ASCII.GetString(tempBuffer);
						type = GetMsgType(TcpRead);

						LocalConsole.Instance.Log("Received [" + type + "] from " + remote.Address + " on port " + remote.Port, true);

						switch(type)
						{
							// Handshake means server is challenging for session id
							case MsgType.HANDSHAKE:
								SendMsgTcp(_session, MsgType.HANDSHAKE);
								break;
							// Status generally means a heartbeat check from the server
							case MsgType.STATUS:
								if (dataLen == SUBHEADER_SIZE)   // Empty Status packet is a heartbeat
									SendMsgTcp("", MsgType.STATUS);
								else
									LocalConsole.Instance.Log("Status msg received");
								break;
							// Contains session data
							case MsgType.SESSION:
								string[] msgSplit = msg.Split(DELIM);
								if(msgSplit.Length == 3)    // SessionId, Udp port, and Tcp port
								{
									if (Int32.TryParse(msgSplit[1], out int nUdp) && Int32.TryParse(msgSplit[2], out int nTcp))
									{
										_session = msgSplit[0];
										_remoteUdpPort = nUdp;
										_remoteTcpPort = nTcp;

										LocalConsole.Instance.Log("Successfully obtained session data, reconnecting...");

										ConnectToRemote(RemoteIp.ToString());
									}
									else
									{
										LocalConsole.Instance.Log("Error parsing ports from session data");
									}
								}
								else
								{
									LocalConsole.Instance.Log("Error parsing session info from data");
								}
								break;
							case MsgType.MSG:
							default:
								LocalConsole.Instance.Log("Msg received");
								break;
						}
					}
				}
				catch (SocketException s)
				{
					LocalConsole.Instance.LogError("SocketException in ClientTcpRoutine: Code " + s.ErrorCode, true);
				}
				catch (IOException io)
				{
					LocalConsole.Instance.LogError("IOException in ClientTcpRoutine: " + io.Message, true);
				}
				catch (ObjectDisposedException o)
				{
					LocalConsole.Instance.LogError("ObjectDisposedException in ClientTcpRoutine: " + o.Message + "," + o.ObjectName, true);
				}

				// yield return null must be in while loop
				yield return null;
			}
		}

		public void StopTcpRoutine()
		{
			if (_tcpCo != null)
				StopCoroutine(_tcpCo);
		}

		IEnumerator ServerRoutine()
		{
			yield return null;
		}

		public void CleanUp(bool stopCo = false)
		{
			if (!_disposing)
			{
				_disposing = true;
				Instance = null;

				if (stopCo)
				{
					if (IsServer)
						StopCoroutine(_serverCo);

					StopCoroutine(_udpCo);
					StopCoroutine(_tcpCo);
				}

				Disconnect();
				TcpListen = null;
			}
		}
	}
}
