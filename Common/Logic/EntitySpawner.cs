
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Hubris;

namespace Apex
{
	/// <summary>
	/// Finds valid spawnpoints and spawns the specified number of a provided NPC prefab
	/// </summary>
	public class EntitySpawner : Entity
	{
		// Children and npc num constants
		const uint CHILD_MAX_DEF = 8,
					SPAWN_MAX = 16;

		// Spawner timer constants
		const float SPAWN_CHECK_COEFF = 0.5f,
					SPAWN_CHECK_VAR = 10.0f,
					SPAWN_CHECK_TIME = 30.0f;

		// Spawner distance and radius constants
		const float SPAWN_CHECK_DIST = 100.0f,
					SPAWN_RAD_MIN = 10.0f,
					SPAWN_RAD_MAX = 1000.0f,
					SPAWN_RAD_DEF = 100.0f;

		///--------------------------------------------------------------------
		/// EntitySpawner instance vars
		///--------------------------------------------------------------------

		private bool _canSpawn;                     // Can we spawn NPCs?
		private Vector3[] _spawnPoints;             // Where can the NPCs spawn around the spawner?
		private float _spawnTime = 0.0f;            // Time threshold for attempting spawns
		private float _spawnPointTime = 0.0f;       // Time threshold for finding new spawnpoints
		private float _timer = 0.0f;

		private uint _tryCount = 0;

		private List<Npc> _childList;               // List of any active children

		[SerializeField]
		[Range( SPAWN_RAD_MIN, SPAWN_RAD_MAX )]
		private float _spawnRad = SPAWN_RAD_DEF;    // Radius for spawning and checking if a spawn can occur
		[SerializeField]
		[Range( 0.5f, 2.0f )]
		private float _navMeshSampleDist = 1.0f;
		[SerializeField]
		private GameObject _npcPrefab = null;       // What Npc will we be spawning? Must be defined for Fixed Spawn
		[SerializeField]
		private Npc _npcScript = null;
		[SerializeField]
		private LayerMask _maskCheck;               // LayerMask for Entity/other layers to check when determining whether something is too close to spawn
		[SerializeField]
		private uint _npcNum;                       // How many of the Npc will we be spawning?
		[SerializeField]
		private uint _childMax;                     // The maximum number of this Spawner's children that can be active at a given time
		[SerializeField]
		private bool _randomNum = true;             // Should the number to spawn be randomized?
		[SerializeField]
		private bool _randomRotation = true;        // Will the NPCs spawned be assigned a random rotation?


		///--------------------------------------------------------------------
		/// EntitySpawner properties
		///--------------------------------------------------------------------

		public bool CanSpawn => _canSpawn;
		public List<Npc> ChildList => _childList;
		public int ChildNum => _childList.Count;
		public float SpawnRad => _spawnRad;
		public GameObject NpcPrefab => _npcPrefab;
		public uint NpcNum => _npcNum;
		public uint ChildMax => _childMax;
		public bool RandomNum => _randomNum;
		public bool RandomRotation => _randomRotation;


		///--------------------------------------------------------------------
		/// EntitySpawner methods
		///--------------------------------------------------------------------

		public override void OnEnable ()
		{
			base.OnEnable();

			_childList = new List<Npc>();
		}

		private void Start ()
		{
			if ( HubrisCore.Instance == null )
			{
				SetHubrisActive( false );
				return;
			}

			_canSpawn = false;

			// If we have a null prefab at this point, we have a problem
			if ( _npcPrefab == null )
			{
				LocalConsole.Instance.LogWarning( $"Spawner {this.gameObject.name} at {transform.position} has a null Npc prefab, disabling spawner...", true );
				SetHubrisActive( false );
				return;
			}

			// If we have a prefab but no ApexNpc script, check for it
			if ( _npcScript == null )
			{
				_npcScript = _npcPrefab.GetComponent<Npc>();

				if ( _npcScript == null )
				{
					LocalConsole.Instance.LogError( $"Spawner {this.gameObject.name} at {transform.position} could not find Npc script, disabling spawner...", true );
					SetHubrisActive( false );
					return;
				}
			}

			// If ChildMax has not been set by the level designer
			if ( _childMax == 0 )
				_childMax = CHILD_MAX_DEF;

			// If the RandomNum flag is enabled
			if ( _randomNum )
				_npcNum = (uint)Random.Range( 0, _childMax ) + 1; // + 1 because Random.Range is exclusive when getting an integer range
		}

		/// <summary>
		/// Sets spawn time and spawn point check time for next attempt using default variance and coefficient
		/// </summary>
		public void SetSpawnTime ( float baseTime )
		{
			_spawnTime = baseTime + Random.Range( -SPAWN_CHECK_VAR, SPAWN_CHECK_VAR );
			_spawnPointTime = _spawnTime * SPAWN_CHECK_COEFF;
		}

		public void SetSpawnTimeAndActivate ( float baseTime )
		{
			SetSpawnTime( baseTime );
			SetHubrisActive( true );
		}

		public void SetNpcToSpawn ( GameObject prefab, Npc npc )
		{
			_npcPrefab = prefab;
			_npcScript = npc;
		}

		public void SetNumToSpawn ( uint num )
		{
			_npcNum = num;
		}

		public void SetChildMax ( uint max )
		{
			_childMax = max;
		}

		/// <summary>
		/// Randomize the number of NPCs to spawn
		/// </summary>
		public void RandomizeNpcNum ()
		{
			_npcNum = (uint)Random.Range( 0, _childMax ) + 1; // + 1 because Random.Range is exclusive when getting an integer range
		}

		private void FindSpawnPoints ()
		{
			if ( _npcNum == 0 )
			{
				LocalConsole.Instance.LogWarning( $"Spawner {this.gameObject.name} at {transform.position} has NpcNum set to 0 so nothing will spawn, disabling spawner...", true );
				SetHubrisActive( false );
				return;
			}

			if ( _npcNum > SPAWN_MAX )
			{
				LocalConsole.Instance.LogWarning( $"Spawner {this.gameObject.name} at {transform.position} is trying to spawn too many Npcs ({_npcNum}), setting to max ({SPAWN_MAX})", true );
				_npcNum = SPAWN_MAX;
			}

			_spawnPoints = new Vector3[_npcNum];

			bool debug = HubrisCore.Instance.Debug;
			Vector3 ray = transform.TransformDirection( Vector3.down );
			RaycastHit coord;

			DrawSpawnRad();

			for ( int i = 0; i < _spawnPoints.Length; i++ )
			{
				ray = transform.TransformDirection( Random.Range( -0.8f, 0.8f ), Random.Range( -1.0f, 0 ), Random.Range( -0.8f, 0.8f ) );

				if ( debug )
					Debug.DrawRay( transform.position, ray * _spawnRad, Color.white, 300f, false );

				if ( Physics.Raycast( transform.position, ray, out coord, _spawnRad ) )
				{
					if ( !NavMesh.SamplePosition( coord.point, out NavMeshHit hit, _navMeshSampleDist, NavMesh.AllAreas ) )
					{
						if ( debug )
							LocalConsole.Instance.LogWarning( $"Spawner {this.gameObject.name} at {transform.position}: Discovered point at " + coord.point + ", but it's not on the NavMesh, refusing spawn location", true );
						_spawnPoints[i] = Vector3.zero;
						continue;
					}

					if ( !_canSpawn )
						_canSpawn = true;

					_spawnPoints[i] = coord.point;

					// LocalConsole.Instance.Log( "Spawnpoint found! " + coord.point, true );
				}
				else
				{
					Vector3 panicCoord = transform.position + ( ray * _spawnRad );

					if ( debug )
						Debug.DrawRay( panicCoord, Vector3.down * _spawnRad, Color.magenta, 300f, false );

					if ( Physics.Raycast( panicCoord, Vector3.down, out coord, _spawnRad ) )
					{
						if ( !_canSpawn )
							_canSpawn = true;

						_spawnPoints[i] = coord.point;

						if ( debug )
							LocalConsole.Instance.Log( $"Spawner {this.gameObject.name} at {transform.position}: No spawnpoint found in the direction of " + ray + ", but found terrain; redirecting to spawn at " + coord.point, true );
					}
					else
					{
						_spawnPoints[i] = Vector3.zero;

						if ( debug )
							LocalConsole.Instance.LogWarning( $"Spawner {this.gameObject.name} at {transform.position}: No spawnpoint found in the direction of " + ray + ", and no terrain found below " + panicCoord + ", refusing to spawn", true );
					}
				}
			}

			// Didn't find any spawn points, despite best efforts
			if ( !_canSpawn )
			{
				LocalConsole.Instance.LogError( $"Spawner {this.gameObject.name} at {transform.position} was unable to locate any valid spawn points", true );

				_tryCount++;

				if ( _tryCount >= 3 )
				{
					LocalConsole.Instance.LogError( $"Spawner {this.gameObject.name} at {transform.position} exceeded number of spawn tries, disabling...", true );
					SetHubrisActive( false );
				}

				return;
			}
		}

		private void DrawSpawnRad ()
		{
			Debug.DrawRay( transform.position, transform.TransformDirection( -0.8f, 0, 0 ) * _spawnRad, Color.red, SPAWN_CHECK_TIME * 2.0f, false ); // X, red, min
			Debug.DrawRay( transform.position, transform.TransformDirection( 0.8f, 0, 0 ) * _spawnRad, Color.red, SPAWN_CHECK_TIME * 2.0f, false ); // X, red, max
			Debug.DrawRay( transform.position, transform.TransformDirection( 0, -1.0f, 0 ) * _spawnRad, Color.green, SPAWN_CHECK_TIME * 2.0f, false ); // Y, green, min
			Debug.DrawRay( transform.position, transform.TransformDirection( 0, 0, 0 ) * _spawnRad, Color.green, SPAWN_CHECK_TIME * 2.0f, false ); // Y, green, max
			Debug.DrawRay( transform.position, transform.TransformDirection( 0, 0, -0.8f ) * _spawnRad, Color.blue, SPAWN_CHECK_TIME * 2.0f, false ); // Z, blue, min
			Debug.DrawRay( transform.position, transform.TransformDirection( 0, 0, 0.8f ) * _spawnRad, Color.blue, SPAWN_CHECK_TIME * 2.0f, false ); // Z, blue, max
		}

		public void CheckDetails ( bool force = false )
		{
			if ( HubrisCore.Instance?.Debug ?? force )
				Debug.Log( $"Spawner at {transform.position}: {_spawnPoints?.Length ?? 0} spawn points found, {_npcNum} {( _npcPrefab != null ? _npcPrefab.name : "nullent" )} set to spawn " );
		}

		/// <summary>
		/// Returns True if spawn is obstructed by an Npc, False otherwise
		/// </summary>
		/// <returns></returns>
		private bool IsObstructed ()
		{
			Collider[] colArray = Physics.OverlapSphere( this.transform.position, SPAWN_CHECK_DIST, _maskCheck );

			if ( colArray != null && colArray.Length > 0 )
			{
				foreach ( Collider col in colArray )
				{
					if ( col.gameObject.tag == "Npc" )
					{
						if ( HubrisCore.Instance.Debug )
							LocalConsole.Instance.LogWarning( $"Spawner {this.gameObject.name} at {transform.position} is obstructed by NPC {col.transform.root.gameObject.name}", true );
						return true;
					}

					if ( col.gameObject.tag == "Player" )
					{
						if ( HubrisCore.Instance.Debug )
							LocalConsole.Instance.LogWarning( $"Spawner {this.gameObject.name} at {transform.position} is obstructed by Player", true );
						return true;
					}

					if ( HubrisCore.Instance.Debug )
						LocalConsole.Instance.Log( $"IsObstructed for {this.gameObject.name} at {transform.position} found {col.gameObject.name} with tag {col.gameObject.tag} attached to {col.transform.root.gameObject} with tag {col.transform.root.gameObject.tag}", true );
				}
			}

			if ( HubrisCore.Instance.Debug )
				LocalConsole.Instance.LogWarning( $"Spawner {this.gameObject.name} at {transform.position} is not obstructed", true );

			return false;
		}

		public void Spawn ()
		{
			if ( !_canSpawn )
			{
				if ( HubrisCore.Instance.Debug )
					LocalConsole.Instance.LogWarning( $"Spawner {this.gameObject.name} at {transform.position} is unable to spawn", true );
				CheckDetails( true );
				return;
			}

			// We're spawning successfully, so reset try count
			_tryCount = 0;

			if ( _childList == null )
				_childList = new List<Npc>();

			if ( _childList.Count >= _childMax )
			{
				if ( HubrisCore.Instance.Debug )
					LocalConsole.Instance.LogWarning( $"Spawner {this.gameObject.name} at {transform.position} has too many active children to spawn", true );
				return;
			}

			if ( HubrisCore.Instance.Debug )
				LocalConsole.Instance.LogWarning( $"Spawner {this.gameObject.name} at {transform.position} is spawning {_npcNum} {_npcPrefab.name}(s)" );

			for ( int i = 0; i < _npcNum; i++ )
			{
				if ( _childList.Count >= _childMax )
					break;

				if ( _spawnPoints[i] != Vector3.zero )
				{
					Quaternion rotation = Quaternion.identity;

					if ( RandomRotation )
						rotation = Quaternion.Euler( 0.0f, Random.Range( 0.0f, 360.0f ), 0.0f );

					Npc npc = Instantiate( _npcPrefab, _spawnPoints[i], rotation ).GetComponent<Npc>();

					if ( npc == null )
					{
						if ( HubrisCore.Instance.Debug )
							LocalConsole.Instance.LogError( $"Spawner {this.gameObject.name} at {transform.position} tried to spawn an Npc without an ApexNpc script", true );
						break;
					}

					_childList.Add( npc );
				}
			}

			// Must refresh spawn points with FindSpawnPoints() before respawning
			_canSpawn = false;
		}

		public void TriggerSpawn ()
		{
			FindSpawnPoints();
			Spawn();
		}

		private void RefreshChildren ()
		{
			if ( _childList == null )
			{
				if ( HubrisCore.Instance.Debug )
					LocalConsole.Instance.LogWarning( $"Spawner {this.gameObject.name} at {transform.position} _childList is null, can't check children...", true );
				return;
			}

			if ( _childList.Count == 0 )
			{
				if ( HubrisCore.Instance.Debug )
					LocalConsole.Instance.LogWarning( $"Spawner {this.gameObject.name} at {transform.position} has no children, can't check children...", true );
				return;
			}

			List<Npc> keepList = new List<Npc>();

			foreach ( Npc npc in _childList )
			{
				if ( npc == null )
					continue;

				keepList.Add( npc );
			}

			_childList = keepList;
		}

		public void PurgeChildren ()
		{
			if ( _childList == null )
			{
				if ( HubrisCore.Instance.Debug )
					LocalConsole.Instance.LogWarning( $"Spawner {this.gameObject.name} at {transform.position} _childList is null, can't purge...", true );
				return;
			}

			if ( _childList.Count == 0 )
			{
				if ( HubrisCore.Instance.Debug )
					LocalConsole.Instance.LogWarning( $"Spawner {this.gameObject.name} at {transform.position} has no children, can't purge...", true );
				return;
			}

			Npc[] _children = _childList.ToArray();

			for ( int i = 0; i < _children.Length; i++ )
			{
				if ( _children[i] == null )
					continue;

				_children[i].Stats.Kill();
			}
		}

		public void FixedUpdate ()
		{
			// If inactive, quick return
			if ( !_act )
				return;

			_timer += Time.deltaTime;

			// If can spawn is false, generally meaning we need to update spawn points
			if ( !_canSpawn && _timer >= _spawnPointTime )
				FindSpawnPoints();

			if ( _timer >= _spawnTime )
			{
				_timer = 0.0f;

				// Update list of spawned children 
				RefreshChildren();

				// If not obstructed by an Npc or a Player, spawn
				if ( !IsObstructed() )
					Spawn();

				SetSpawnTime( SPAWN_CHECK_TIME );
			}
		}
	}
}