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
		private LayerMask _mask; 	                // What other entities should this Npc be aware of?
		[SerializeField]
		private GameObject _targetObj = null;
		[SerializeField]
		private Vector3 _movePos = Vector3.zero;    // Where the Agent is trying to move

		[SerializeField]
		private BhvTree _bhvTree = new BhvTree( BNpcIdle.Instance );
		[SerializeField]
		private FOV _fov;

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

		public Animator Anim
		{
			get { return _anim; }
			protected set { _anim = value; }
		}

		public AIParams Params
		{
			get { return _aiParams; }
		}

		public NpcType Type => Params?.Type ?? NpcType.BASE;

		public NavMeshAgent NavAgent
		{
			get { return _navAgent; }
			protected set { _navAgent = value; }
		}

		public LayerMask EntMask
		{
			get { return _mask; }
		}

		public GameObject TargetObj
		{
			get { return _targetObj; }
			protected set { _targetObj = value; }
		}

		public Vector3 TargetPos
		{
			get { return _targetObj.transform.position; }
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
			base.OnEnable();
			base.Init();

			if (NavAgent == null)
			{
				NavAgent = GetComponent<NavMeshAgent>();
				if (NavAgent == null)
					NavAgent = this.gameObject.AddComponent<NavMeshAgent>();
			}

			if(Anim == null)
			{
				Anim = GetComponent<Animator>();
				if (Anim == null)
					Debug.Log(this.gameObject.name + " doesn't have an Animator!");
			}

			ViewCone = new FOV( this.transform.forward, _aiParams.FOV );

			EntType = EntityType.NPC;
		}

		public void SetTargetObj(GameObject obj)
		{
			_targetObj = obj;
		}

		public void SetMovePos(Vector3 nPos)
		{
			_movePos = nPos;
		}

		public void MoveTo(Vector3 nPos)
		{
			MovePos = nPos;
			NavAgent.SetDestination(MovePos);
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
					LiveEntity src = SoundEventList[i].Source.GetComponent<LiveEntity>();

					if ( src != null )
					{
						SetTargetObj( src.gameObject );
					}
				}

				SoundEventList.Clear();
			}
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
