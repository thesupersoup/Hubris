﻿using System.Collections;
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
		[SerializeField]
		private AIParams _aiParams = null;
		[SerializeField]
		private Animator _anim = null;
		[SerializeField]
		private NavMeshAgent _navAgent = null;
		[SerializeField]
		private LayerMask _entMask;					// What other entities should this Npc be aware of?
		[SerializeField]
		private LayerMask _sightMask;				// What should this Npc be aware of for line-of-sight checks?
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

		private FOV _fov;
		[SerializeField]
		private Transform _fovOrigin = null;
		[SerializeField]
		[Tooltip( "Will cause the viewcone to be shown in the Unity editor scene view" )]
		private bool _debugShowViewCone = false;

		private List<LiveEntity> _trackList = new List<LiveEntity>(), 
								_removeList = new List<LiveEntity>();

		#region Npc Properties
		///--------------------------------------------------------------------
		/// Npc Properties
		///--------------------------------------------------------------------

		public BhvTree Behavior
		{
			get
			{ return _bhvTree; }
			protected set
			{ _bhvTree = value; }
		}

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

		public LayerMask EntMask => _entMask;

		public LayerMask SightMask => _sightMask;

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

		public List<LiveEntity> TrackList
		{
			get { return _trackList; }
		}

		public List<LiveEntity> RemoveList
		{
			get { return _removeList; }
		}
		#endregion Npc Properties

		///--------------------------------------------------------------------
		/// Npc Methods
		///--------------------------------------------------------------------

		public override void OnEnable()
		{
			if ( Params == null )
			{
				LocalConsole.Instance.Log( $"Npc {this.name} has no associated AIParams object, instantiating default...", true );
				Params = Instantiate( AIParams.GetDefault() );
			}

			base.OnEnable();
			base.Init();

			if ( NavAgent == null )
			{
				NavAgent = GetComponent<NavMeshAgent>();
				if (NavAgent == null)
					NavAgent = this.gameObject.AddComponent<NavMeshAgent>();
			}

			InitNavAgent();

			if( Anim == null )
			{
				Anim = GetComponent<Animator>();
				if (Anim == null)
					Debug.Log(this.gameObject.name + " doesn't have an Animator!");
			}

			ViewCone = new FOV( (ViewConeOrigin != null ? ViewConeOrigin.position + this.transform.forward : this.transform.forward), _aiParams.FOV );

			EntType = EntityType.NPC;
		}

		/// <summary>
		/// Initialize NavMeshAgent variables; auto repath set as parameter which defaults to false
		/// </summary>
		/// <param name="repath"></param>
		public void InitNavAgent( bool repath = false )
		{
			NavAgent.stoppingDistance = Params.StopDist;
			NavAgent.acceleration = Params.AccelBase;

			// Default to false, so we can handle situations where a path can't be found in behaviors
			NavAgent.autoRepath = repath;
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
			Debug.Log( $"{this.name} resetting target object" );

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

			Vector3 dir = ( target.transform.position - ViewConeOriginPos ).normalized;
			Vector3 viewChk = dir;
			viewChk.y = 0.0f;

			if ( ViewCone.IsInView( this.transform.forward, viewChk ) )
			{
				if ( Physics.Raycast( ViewConeOriginPos, dir, out RaycastHit hit, TargetDistSqr, SightMask ) )
				{
					if( hit.transform.root.gameObject != target )
						return false;

					return true;
				}
			}

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
					GameObject src = SoundEventList[i].Source;
					Vector3 origin = SoundEventList[i].Origin;
					SoundIntensity intensity = SoundEventList[i].Intensity;

					if ( TargetObj == null )
					{
						if ( src == null )
						{
							if ( intensity >= SoundIntensity.NOTEWORTHY || MovePos == Vector3.zero )
								SetTargetPos( origin );

							continue;
						}

						SetTargetObj( src );
					}
					else
					{
						// Check if the sound event is closer than the current target
						if ( TargetDistSqr > Util.CheckDistSqr( this.transform.position, origin ) )
							SetTargetObj( src );
					}
				}

				SoundEventList.Clear();
			}
		}

		/// <summary>
		/// Handles taking damage from another entity; use nDirect true to directly damage health or false to obey restrictions
		/// </summary>
		public override bool TakeDmg( LiveEntity damageEnt, DmgType nType, int nDmg, bool nDirect )
		{
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

			return base.TakeDmg( damageEnt, nType, nDmg, nDirect );
		}

		void Start()
		{

		}

		public override void Tick()
		{

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
	}
}
