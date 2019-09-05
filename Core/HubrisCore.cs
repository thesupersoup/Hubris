using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

namespace Hubris
{
	/// <summary>
	/// The central hub for all Hubris-related behavior. Must be present in every scene using Hubris Entities.
	/// </summary>
	[RequireComponent( typeof( NetworkIdentity ) )]
	public class HubrisCore : NetworkBehaviour
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
				if ( _disposing )
					return null;
				else
					return _i;
			}

			protected set
			{
				lock ( _lock )  // Thread safety
				{
					if ( (_i == null && !_disposing) || (_i != null && value == null) ) // Only set if _i is already null or we're disposing of this instance
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
		private string _menuSceneName = "menu";
		[SerializeField]
		protected float _tick = 1.5f;       // Tick time in seconds
		protected float _timer;
		protected bool _willCall = false;   // Will call Tick() and LateTick() next Update and LateUpdate(), respectively
		protected bool _ingame = false;
		[SerializeField]
		[Tooltip( "Template prefab for instanting UI GameObject" )]
		private GameObject _ui = null;

		private GameManager _gm = new GameManager();        // "new GameManager()" required to prevent null errors
		private LocalConsole _con = new LocalConsole();     // "new LocalConsole()" required to prevent null errors

		private ulong _uId = 1;	// Start at 1, so 0 can represent unassigned
		private Dictionary<ulong, LiveEntity> _entDict = new Dictionary<ulong, LiveEntity>();
		private Dictionary<LiveEntity, GameObject> _rootObjDict = new Dictionary<LiveEntity, GameObject>();
		private Dictionary<GameObject, LiveEntity> _objToEntDict = new Dictionary<GameObject, LiveEntity>();
		private Dictionary<ulong, HubrisPlayer> _playerDict = new Dictionary<ulong, HubrisPlayer>();

		[SerializeField]
		[Tooltip( "Set to false if providing own input manager" )]
		private bool _enableInputMgr = true;

		private InputManager _im = null;

		///--------------------------------------------------------------------
		/// HubrisCore actions
		///--------------------------------------------------------------------

		public Action AcTick;
		public Action AcLateTick;
		public Action AcFixedTick;
		public Action<SoundEvent> AcSoundEvent;
		public Action<bool> AcCleanUp;

		///--------------------------------------------------------------------
		/// HubrisCore properties
		///--------------------------------------------------------------------

		public string MenuSceneName => _menuSceneName;

		// Moved to SettingsHub
		public bool Debug => (bool)_con?.Settings?.Debug.Data;

		// Tick time in seconds
		public float TickTime => _tick;

		public bool Ingame => _ingame;

		public GameManager GM => _gm;

		public LocalConsole Console => _con;

		public bool UseInputManager => _enableInputMgr;

		public InputManager Input => _im;


		///--------------------------------------------------------------------
		/// HubrisCore methods
		///--------------------------------------------------------------------

		void OnEnable()
		{
			if ( Instance == null )
			{
				Instance = this;
				DontDestroyOnLoad( this );
			}
			else if ( Instance != this )
			{
				// Enforce Singleton pattern 
				Destroy( this.gameObject );
				return;
			}

			if ( Instance == this )
			{
				_timer = 0.0f;

				// Init Console before other objects
				_con.Init();
				_gm.Init();

				if ( UseInputManager )
				{
					_im = new InputManager();
					_im.Init();
				}

				// Initialize UI object
				if ( _ui != null )
				{
					GameObject temp = Instantiate( _ui );
					temp.name = _ui.name;   // None of that "(Clone)" nonsense
				}

				SceneManager.sceneUnloaded += OnSceneUnloaded;
			}
		}

		public void VersionPrint()
		{
			Console.Log( "Current Hubris Build: v" + Version.GetString() );
		}

		public void NetInfoPrint()
		{
			Console.Log( "No Hubris specific networking info to report" );
		}

		public void DebugToggle()
		{
			// Console.Settings.Debug.Data = !(bool)_con.Settings.Debug.Data;

			Console.Log( "Debug mode " + (Debug ? "activated" : "deactivated") );

			if ( UIManager.Instance != null )
			{
				UIManager.Instance.DevSet( Debug );
			}
		}

		public void SetIngame( bool game )
		{
			_ingame = game;
			Console.Log( "HubrisCore switching to " + (Ingame ? "ingame" : "not ingame") + " mode");
		}

		/// <summary>
		/// Pull the current unique Id then increment
		/// </summary>
		private ulong PullUniqueId()
		{
			ulong id = _uId;

			_uId++;

			return id;
		}

		/// <summary>
		/// Register the LiveEntity and its root GameObject in their respective dictionaries and return the unique Id assigned
		/// </summary>
		public ulong RegisterEnt( LiveEntity ent, GameObject root = null )
		{
			ulong id = PullUniqueId();
			GameObject obj = root;

			if ( obj == null && ent != null )
				obj = ent.transform.root.gameObject;

			_entDict.Add( id, ent );
			_rootObjDict.Add( ent, obj );
			_objToEntDict.Add( obj, ent );

			return id;
		}

		/// <summary>
		/// Attempt to unregister a LiveEntity/root GameObject combo by unique Id
		/// </summary>
		public bool UnregisterEnt( ulong id )
		{
			LiveEntity ent = null;
			GameObject obj = null;

			if ( _entDict.TryGetValue( id, out ent ) )
				_rootObjDict.TryGetValue( ent, out obj );

			return _entDict.Remove( id ) && _rootObjDict.Remove( ent ) && _objToEntDict.Remove( obj );
		}

		/// <summary>
		/// Returns null if no entity is found
		/// </summary>
		public LiveEntity TryGetEnt( ulong id )
		{
			if ( _entDict.TryGetValue( id, out LiveEntity ent ) )
				return ent;

			return null;
		}

		/// <summary>
		/// Returns null if no entity is found
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public LiveEntity TryGetEnt( GameObject obj )
		{
			if ( _objToEntDict.TryGetValue( obj, out LiveEntity ent ) )
				return ent;

			return null;
		}

		/// <summary>
		/// Returns null if no object is found
		/// </summary>
		public GameObject TryGetRootObj( LiveEntity ent )
		{
			if ( _rootObjDict.TryGetValue( ent, out GameObject obj ) )
				return obj;

			return null;
		}

		/// <summary>
		/// Returns null if no object is found
		/// </summary>
		public GameObject TryGetRootObj( ulong id )
		{
			return TryGetRootObj( TryGetEnt( id ) );
		}

		/// <summary>
		/// Attempts to send damage to the specified entity; use if you only have the GameObject of the target
		/// </summary>
		public virtual bool TrySendDmg( GameObject target, LiveEntity src, int nType, int nDmg, bool nDirect )
		{
			if ( target == null )
				return false;

			LiveEntity ent = TryGetEnt( target );

			if ( ent == null )
				return false;

			return ent.TakeDmg( src, nType, nDmg, nDirect );
		}

		/// <summary>
		/// Attempts to send damage to the specified entity; use if you have the LiveEntity UID
		/// </summary>
		public virtual bool TrySendDmg( ulong id, int nType, int nDmg, bool nDirect )
		{
			LiveEntity ent = TryGetEnt( id );

			if ( ent == null )
				return false;

			return ent.TakeDmg( null, nType, nDmg, nDirect );
		}

		public Dictionary<ulong, LiveEntity> GetEntDict()
		{
			return _entDict;
		}

		public Dictionary<LiveEntity, GameObject> GetRootObjDict()
		{
			return _rootObjDict;
		}

		/// <summary>
		/// Register the HubrisPlayer in the dictionary and return the unique Id assigned
		/// </summary>
		public ulong RegisterPlayer( HubrisPlayer player, GameObject obj = null )
		{
			if ( player == null )
				return 0;

			ulong id = PullUniqueId();
			GameObject root = player.transform.root.gameObject;

			_playerDict.Add( id, player );
			_entDict.Add( id, player );
			_rootObjDict.Add( player, root );
			_objToEntDict.Add( root, player );

			return id;
		}

		/// <summary>
		/// Attempt to unregister a HubrisPlayer by unique Id
		/// </summary>
		public bool UnregisterPlayer( ulong id )
		{
			LiveEntity ent = TryGetEnt( id );

			if ( ent == null )
				return false;

			GameObject root = TryGetRootObj( id );

			if ( root == null )
				return false;

			return _playerDict.Remove( id ) && _entDict.Remove( id ) && _objToEntDict.Remove( root ) && _rootObjDict.Remove( ent );
		}

		/// <summary>
		/// Returns null if no player is found
		/// </summary>
		public HubrisPlayer TryGetPlayer( ulong id )
		{
			if ( _playerDict.TryGetValue( id, out HubrisPlayer player ) )
				return player;

			return null;
		}

		public Dictionary<ulong, HubrisPlayer> GetPlayerDict()
		{
			return _playerDict;
		}

		public void BroadcastSoundEvent( SoundEvent ev )
		{
			AcSoundEvent?.Invoke( ev );
		}

		public virtual void Escape()
		{
			if ( Ingame )
				HubrisPlayer.Instance.Escape();
			else
				UIManager.Instance.Escape();
		}

		public virtual void ClearDicts()
		{
			_entDict.Clear();
			_rootObjDict.Clear();
			_objToEntDict.Clear();
			_playerDict.Clear();
		}

		public virtual void ResetUid()
		{
			_uId = 1;	// Uid should start at 1
		}

		public virtual void LoadScene( string name )
		{
			SceneManager.LoadScene( name );
			ClearDicts();
			ResetUid();
		}

		void FixedUpdate()
		{
			_im?.FixedUpdate();

			OnFixedTick();

			_timer += Time.deltaTime;

			if ( _timer > _tick )
			{
				_willCall = true;
				_timer = 0.0f;
			}

			if ( _willCall )
			{
				OnTick(); // Broadcast Tick() event
			}
		}

		void Update()
		{
			_im?.Update();
		}

		void LateUpdate()
		{
			_im?.LateUpdate();

			if ( _willCall )
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

		void OnSceneUnloaded( Scene s )
		{
			GM.ClearInfo();
		}

		void OnApplicationQuit()
		{
			AcCleanUp?.Invoke( true );
		}

		void OnDestroy()
		{
			AcCleanUp?.Invoke( true );

			SceneManager.sceneUnloaded -= OnSceneUnloaded;
		}
	}

}
