using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Hubris
{
	/// <summary>
	/// Base class for all AI-driven Npcs with associated stats
	/// </summary>
	public class Npc : LiveEntity
	{
		#region Npc instance vars
		///--------------------------------------------------------------------
		/// Npc instance vars
		///--------------------------------------------------------------------

		[SerializeField]
		private AIParams _aiParams = null;
		[SerializeField]
		private Animator _anim = null;
		[SerializeField]
		private NavMeshAgent _navAgent = null;

		[SerializeField]
		protected LayerMask _entMask;					// What other entities should this Npc be aware of?
		[SerializeField]
		protected LayerMask _sightMask;				// What should this Npc be aware of for line-of-sight checks?
		[SerializeField]
		private GameObject _targetObj = null;
		[SerializeField]
		private LiveEntity _targetEnt = null;
		[SerializeField]
		private EntStats _targetStats = null;
		[SerializeField]
		private Vector3 _movePos = Vector3.zero;	// Where the Npc is trying to move
		[SerializeField]
		private Vector3 _targetPos = Vector3.zero;	// Where the Npc is trying to look 

		[SerializeField]
		private BhvTree _bhvTree = new BhvTree( BNpcIdle.Instance );
		[SerializeField]
		private string _bhvBranch;		// Visual feedback purposes only

		private FOV _fov;
		[SerializeField]
		private Transform _fovOrigin = null;
		[SerializeField]
		[Tooltip( "Will cause the viewcone to be shown in the Unity editor scene view" )]
		private bool _debugShowViewCone = false;
		[SerializeField]
		private Rigidbody _rigidBody = null;
		[SerializeField]
		private SphereCollider _areaScanCol = null;

		private Dictionary<ulong, LiveEntity> _trackDict = new Dictionary<ulong, LiveEntity>();
		private List<ulong> _removeList = new List<ulong>();
		#endregion Npc instance vars

		#region Npc properties
		///--------------------------------------------------------------------
		/// Npc properties
		///--------------------------------------------------------------------

		public BhvTree Behavior
		{
			get
			{ return _bhvTree; }
			protected set
			{ _bhvTree = value; }
		}

		public string BhvBranch { get { return _bhvBranch; } set { _bhvBranch = value; } }

		public FOV ViewCone
		{
			get
			{ return _fov; }
			protected set
			{ _fov = value; }
		}

		public FOVDegrees ViewConeAngle
		{
			get
			{ return ViewCone.Degrees; }
			protected set
			{ ViewCone.Degrees = value; }
		}

		public Transform ViewConeOrigin => _fovOrigin;

		/// <summary>
		/// Get the appropriate viewcone origin position vector depending on whether the origin transform is set
		/// </summary>
		public Vector3 ViewConeOriginPos => _fovOrigin != null ? _fovOrigin.position : this.transform.position;

		/// <summary>
		/// Will cause the viewcone to be shown in the Unity editor scene view
		/// </summary>
		public bool DebugShowViewCone => _debugShowViewCone;

		public Animator Anim
		{
			get { return _anim; }
			protected set { _anim = value; }
		}

		public AIParams Params
		{
			get { return _aiParams; }
			protected set { _aiParams = value; }
		}

		public NpcType Type => Params != null ? Params.Type : NpcType.BASE;

		public NavMeshAgent NavAgent
		{
			get { return _navAgent; }
			protected set { _navAgent = value; }
		}

		public virtual LayerMask EntMask => _entMask;

		public virtual LayerMask SightMask => _sightMask;

		public GameObject TargetObj
		{
			get { return _targetObj; }
			protected set { _targetObj = value; }
		}

		public LiveEntity TargetEnt
		{
			get { return _targetEnt; }
			protected set { _targetEnt = value; }
		}

		public EntStats TargetStats
		{
			get { return _targetStats; }
			protected set { _targetStats = value; }
		}

		public Vector3 TargetPos
		{
			get { if ( TargetObj != null ) { return TargetObj.transform.position; } else { return _targetPos; } }
			protected set { _targetPos = value; }
		}

		public float TargetDistSqr => Util.CheckDistSqr( this.transform.position, TargetPos );

		public Vector3 MovePos
		{
			get { return _movePos; }
			protected set { _movePos = value; }
		}

		public float MoveDistSqr => Util.CheckDistSqr( this.transform.position, MovePos );

		public SphereCollider AreaScanCol => _areaScanCol;
		public Rigidbody RigidBody => _rigidBody;
		public Dictionary<ulong, LiveEntity> TrackDict => _trackDict;
		public List<ulong> RemoveList => _removeList;

		#endregion Npc Properties

		#region Npc methods
		///--------------------------------------------------------------------
		/// Npc methods
		///--------------------------------------------------------------------

		public override void OnEnable()
		{
			EntityEnable();
			LiveEntityEnable();
			NpcEnable();
		}

		public void NpcEnable()
		{
			if ( Params == null )
			{
				LocalConsole.Instance.Log( $"Npc {this.name} has no associated AIParams object, instantiating default...", true );
				Params = Instantiate( AIParams.GetDefault() );
			}

			if ( NavAgent == null )
			{
				NavAgent = GetComponent<NavMeshAgent>();
				if (NavAgent == null)
					NavAgent = this.gameObject.AddComponent<NavMeshAgent>();
			}

			InitNavAgent();

			if ( Anim == null )
			{
				Anim = GetComponent<Animator>();
				if (Anim == null)
					Debug.Log( $"{this.gameObject.name} doesn't have a doesn't have an Animator attached!" );
			}

			if( RigidBody == null )
			{
				Rigidbody rb = this.gameObject.AddComponent<Rigidbody>();
				rb.isKinematic = true;
				_rigidBody = rb;
			}

			InitRigidBody();

			if ( AreaScanCol == null )
			{
				Debug.LogError( $"{this.gameObject.name} is missing its AreaScanCol, assign in editor! Attempting to recover..." );

				GameObject child = Instantiate( new GameObject() );
				child.transform.parent = this.transform.root;
				child.layer = LayerMask.NameToLayer( "Ignore Raycast" );

				SphereCollider newCol = child.AddComponent<SphereCollider>();
				_areaScanCol = newCol;

				if( AreaScanCol == null )
					Debug.LogError( $"{this.gameObject.name} AreaScanCol is still null after attempt!" );
			}

			InitAreaScanCol();

			ViewCone = new FOV( (ViewConeOrigin != null ? ViewConeOrigin.position + this.transform.forward : this.transform.forward), _aiParams.FOV );

			EntType = EntityType.NPC;
		}

		protected override void Start()
		{
			InitLiveEntity();
			InitTrackDict();
		}

		/// <summary>
		/// Initialize NavMeshAgent variables; auto repath set as parameter which defaults to false
		/// </summary>
		/// <param name="repath"></param>
		protected virtual void InitNavAgent( bool repath = false )
		{
			NavAgent.stoppingDistance = Params.StopDist;
			NavAgent.acceleration = Params.AccelBase;

			NavAgent.updateRotation = true;

			// Default to false, so we can handle situations where a path can't be found in behaviors
			NavAgent.autoRepath = repath;
		}

		protected virtual void InitRigidBody()
		{
			if( RigidBody != null )
			{
				RigidBody.isKinematic = true;
			}
		}

		protected virtual void InitAreaScanCol()
		{
			// Null check again, in case an attempt to recover fails
			if ( AreaScanCol != null )
			{
				// Should be on a child GameObject
				Transform t = AreaScanCol.transform;
				Transform root = this.transform.root;

				// Set the scale of the child appropriately
				t.localScale = new Vector3( (1.0f / root.localScale.x), (1.0f / root.localScale.y), (1.0f / root.localScale.z) );

				AreaScanCol.radius = Params.AwareMax;
			}
		}

		/// <summary>
		/// Performs a SphereCast for the initial population of the TrackDict
		/// </summary>
		protected virtual void InitTrackDict()
		{
			Collider[] localEnts = Physics.OverlapSphere( this.transform.position, Params.AwareMax, EntMask );

			if ( localEnts != null && localEnts.Length > 0 )
			{
				foreach ( Collider col in localEnts )
				{
					GameObject obj = col.transform.root.gameObject;
					LiveEntity ent = obj.GetComponent<LiveEntity>();

					if ( ent == null )
					{
						Debug.LogWarning( $"{this.gameObject.name} couldn't find a LiveEntity script for a detected object ({obj.name}), can't add to TrackDict" );
						continue;
					}

					// Check if we found ourselves
					if ( ent.UniqueId == UniqueId )
						continue;

					// Ignore if the entity is dead or invisible
					if ( ent.Stats.IsDead || ent.Stats.Invisible )
						continue;

					TrackEntity( ent );
				}
			}
		}

		/// <summary>
		/// Adds the specified LiveEntity to the track list if it's not null, and if we're not already tracking it
		/// </summary>
		public void TrackEntity( LiveEntity ent )
		{
			// Null check
			if ( ent == null )
				return;

			// Check if we're already tracking this entity
			if ( TrackDict.ContainsKey( ent.UniqueId ) )
				return;

			TrackDict.Add( ent.UniqueId, ent );
		}

		public void SetTargetObj( GameObject obj )
		{
			TargetObj = obj;

			if ( TargetObj == null )
				return;

			// TargetPos is now the TargetObj's position
			TargetPos = Vector3.zero;
			TargetEnt = TargetObj.GetComponent<LiveEntity>();

			if ( TargetEnt != null )
				TargetStats = TargetEnt.Stats;
		}

		public void SetTargetObj( GameObject obj, LiveEntity ent )
		{
			TargetObj = obj;
			TargetEnt = ent;

			// TargetPos is now the TargetObj's position
			if ( TargetObj != null )
				TargetPos = Vector3.zero;

			if ( TargetEnt != null )
				TargetStats = TargetEnt.Stats;
		}

		public void ResetTargetObj()
		{
			// Debug.Log( $"{this.name} resetting target object" );

			TargetObj = null;
			TargetEnt = null;
			TargetStats = null;
		}

		public void SetTargetPos( Vector3 nPos )
		{
			TargetPos = nPos;
		}

		public void SetMovePos( Vector3 nPos )
		{
			MovePos = nPos;
		}

		public void MoveTo( Vector3 nPos )
		{
			SetMovePos( nPos );
			NavAgent.SetDestination( MovePos );
		}

		/// <summary>
		/// Check whether the target is in the Npc's field-of-view and line-of-sight
		/// </summary>
		public bool SightCheck( GameObject target, float dist )
		{
			if ( target == null )
				return false;

			// Vector3 dir = ( target.transform.position - ViewConeOriginPos ).normalized;
			Vector3 dir = target.transform.position - ViewConeOriginPos;
			Vector3 viewChk = dir;
			viewChk.y = 0.0f;

			if ( ViewCone.IsInView( this.transform.forward, viewChk ) )
				return Util.RayCheckTarget( ViewConeOriginPos, dir, TargetDistSqr, target, SightMask );

			return false;
		}

		/// <summary>
		/// Do something with received sound events
		/// </summary>
		protected override void ProcessSoundEvents()
		{
			if ( SoundEventList != null && SoundEventList.Count > 0 )
			{
				for ( int i = 0; i < SoundEventList.Count; i++ )
				{
					Vector3 origin = SoundEventList[i].Origin;
					float rad = SoundEventList[i].Radius;

					// Check if it's in range; if not, continue
					if ( !Util.CheckDistSqr( origin, this.transform.position, rad ) )
						continue;

					LiveEntity ent = SoundEventList[i].Source;
					GameObject src = SoundEventList[i].Source?.gameObject;
					SoundIntensity intensity = SoundEventList[i].Intensity;

					// Predators investigate sounds if they don't currently have a target and the source did not come from an object
					if ( TargetObj == null && Params.Predator )
					{
						if ( src == null )
						{
							if ( intensity >= SoundIntensity.NOTEWORTHY || (MovePos == Vector3.zero && TargetPos == Vector3.zero) )
								SetTargetPos( origin );

							continue;
						}
					}

					// If the heard entity is another Npc
					if ( ent is Npc npc )
					{
						// Prey ignore other prey
						if ( !Params.Predator && !npc.Params.Predator )
							continue;
					}

					if( ent != null )
					{
						TrackEntity( ent );

						if ( intensity >= SoundIntensity.NOTEWORTHY )
						{
							Behavior.SetIgnoreSightCheck( true );
							SetTargetObj( src, ent );
						}
					}
				}

				SoundEventList.Clear();
			}
		}

		/// <summary>
		/// Handles taking damage from another entity; use nDirect true to directly damage health or false to obey restrictions
		/// </summary>
		public override bool TakeDmg( LiveEntity damageEnt, int nType, int nDmg, bool nDirect )
		{
			bool wasDamaged = base.TakeDmg( damageEnt, nType, nDmg, nDirect );

			if ( wasDamaged )
			{
				PlaySound( SndT.HURT );

				if ( damageEnt != null )
				{
					GameObject damageObj = damageEnt.transform.root.gameObject;

					if ( TargetObj == null )
						SetTargetObj( damageObj, damageEnt );
					else
					{
						if ( damageObj != TargetObj )
						{
							if ( Util.IsCloser( this.transform.position, damageObj.transform.position, TargetPos ) )
								SetTargetObj( damageObj, damageEnt );
						}
					}
				}
			}

			return wasDamaged;
		}

		protected virtual void OnTriggerEnter( Collider col )
		{
			GameObject obj = col.transform.root.gameObject;

			// Bitshift 1 left by obj.layer ( for 1 * 2^obj.Layer, if obj.layer = 8 then 1 * 2^8 = binary 100000000 = decimal 256 ) 
			// to check against the EntMask.value (Only Entity (User Layer 8), equivalent to binary 100000000 = decimal 256)
			if ( 1 << obj.layer != EntMask.value )
				return;

			LiveEntity ent = obj.GetComponent<LiveEntity>();

			if ( ent == null )
			{
				Debug.LogWarning( $"{this.gameObject.name} couldn't find a LiveEntity script for a detected object ({obj.name}), can't add to trackList" );
				return;
			}

			// Check if we found ourselves
			if ( ent.UniqueId == UniqueId )
				return;

			// Ignore if the entity is dead or invisible
			if ( ent.Stats.IsDead || ent.Stats.Invisible )
				return;

			TrackEntity( ent );
		}

		void FixedUpdate()
		{
			Behavior.Invoke( this );
			ProcessSoundEvents();
			ViewCone.UpdateVectors( this.transform.forward );

			if ( DebugShowViewCone )
				ViewCone.DebugDrawVectors( this, true );
		}

		void LateUpdate()
		{
			if(AlignToSurface)
			{
				if(Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit hit, NavAgent.height))
				{
					this.transform.rotation = Quaternion.FromToRotation(this.transform.up, hit.normal);
				}
			}
		}
		#endregion Npc methods
	}
}
