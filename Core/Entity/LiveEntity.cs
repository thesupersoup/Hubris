using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	public class LiveEntity : Entity, IDamageable, ISoundListener, ISoundEmitter
	{
		public const float ARMOR_ABSORB = 0.75f;

		#region LiveEntity instance vars
		///--------------------------------------------------------------------
		/// LiveEntity instance vars
		///--------------------------------------------------------------------

		[SerializeField]
		protected ulong _uId = 0;
		[SerializeField]
		EntityType _type;
		[SerializeField]
		EntStats _stats;
		[SerializeField]
		protected bool _alignSurface = true;		// Should this NPC align itself to the surface normal?
		[SerializeField]
		protected LayerMask _soundEventLayer;		// What layer should sound events check when emitting

		protected List<SoundEvent> _soundEventList = new List<SoundEvent>();
		#endregion LiveEntity instance vars

		#region LiveEntity properties
		///--------------------------------------------------------------------
		/// LiveEntity properties
		///--------------------------------------------------------------------

		public ulong UniqueId => _uId;

		public EntityType EntType
		{
			get
			{ return _type; }
			protected set
			{ _type = value; }
		}

		public EntStats Stats
		{
			get
			{ return _stats; }
			protected set
			{ _stats = value; }
		}

		public bool AlignToSurface
		{
			get
			{ return _alignSurface; }
			protected set
			{ _alignSurface = value; }
		}

		public LayerMask SoundEventLayer => _soundEventLayer;
		public List<SoundEvent> SoundEventList => _soundEventList;
		#endregion LiveEntity properties

		#region LiveEntity methods
		///--------------------------------------------------------------------
		/// LiveEntity methods
		///--------------------------------------------------------------------
		
		public override void OnEnable()
		{
			base.OnEnable();

			if( HubrisCore.Instance == null )
			{
				Debug.LogError( $"LiveEntity {this.gameObject.name} could not locate HubrisCore, fatal error" );
				return;
			}

			SubActions();
		}

		protected virtual void Start()
		{
			Init();
		}

		// Start is called before the first frame update
		protected virtual void Init()
		{
			_uId = HubrisCore.Instance.RegisterEnt( this );
			this.gameObject.name += "_" + _uId;
		}

		private void SubActions()
		{
			HubrisCore.Instance.AcSoundEvent += AddSoundEvent;
			HubrisCore.Instance.AcCleanUp += CleanUp;
		}

		private void UnsubActions()
		{
			if ( HubrisCore.Instance == null )
			{
				Debug.LogError( $"LiveEntity {this.gameObject.name} can't unsub actions due to null HubrisCore" );
				return;
			}

			HubrisCore.Instance.AcSoundEvent -= AddSoundEvent;
			HubrisCore.Instance.AcCleanUp -= CleanUp;
		}

		/// <summary>
		/// Emit the sound event passed as an arguement
		/// </summary>
		public virtual void EmitSoundEvent( SoundEvent ev )
		{
			if ( HubrisCore.Instance == null )
			{
				Debug.LogError( $"LiveEntity {this.gameObject.name} can't emit an event, HubrisCore is null" );
				return;
			}

			// Shouldn't be emitting sounds if we're dead or invisible
			if ( Stats.IsDead || Stats.Invisible )
				return;

			HubrisCore.Instance.BroadcastSoundEvent( ev );

			/*Collider[] colArr = Physics.OverlapSphere( ev.Origin, ev.Radius, SoundEventLayer );
			if ( colArr != null && colArr.Length > 0 )
			{
				for( int i = 0; i < colArr.Length; i++ )
				{
					if ( colArr[i].gameObject.layer != SoundEventLayer )
						continue;

					LiveEntity ent = colArr[i].gameObject.GetComponent<LiveEntity>();

					if ( ent != null && ent != this )
					{
						// Debug.Log( $"Adding sound event to {ent.gameObject.name}" );
						ent.AddSoundEvent( ev );
					}
				}
			}*/
		}

		/// <summary>
		/// Handler for animation triggered events
		/// </summary>
		public virtual void AnimEventHandler( int ev )
		{
			// Add event handling
			// Integers:
			// 0 = Footstep
			// 1 = BodyFall
		}

		/// <summary>
		/// Public interface for a sound emitter to make this entity aware of a sound event
		/// </summary>
		public virtual void AddSoundEvent( SoundEvent ev )
		{
			_soundEventList.Add( ev );
		}

		/// <summary>
		/// Do something with received sound events
		/// </summary>
		protected virtual void ProcessSoundEvents( )
		{
			Debug.Log( "Base ProcessSoundEvent called, accident?" );
			/*if ( SoundEventList != null && SoundEventList.Count > 0 )
			{
				for ( int i = 0; i < SoundEventList.Count; i++ )
				{
					LiveEntity src = SoundEventList[i].Source.GetComponent<LiveEntity>();

					if ( src != null )
					{
						// Do something
					}
				}

				SoundEventList.Clear();
			}*/
		}

		/// <summary>
		/// Handles taking damage from another entity; use nDirect true to directly damage health or false to obey restrictions
		/// </summary>
		public virtual bool TakeDmg( LiveEntity damageEnt, DmgType nType, int nDmg, bool nDirect )
		{
			// Handle damageEnt specific code like changing targets on damage in derived classes

			if ( nType == DmgType.STAMINA)
				return Stats.ReduceStamina( nDmg );
			else if ( nType == DmgType.ARMOR )
				return Stats.ReduceArmor( nDmg );


			int modDmg = nDmg; // modDmg will change if armor takes any of the hit, leaving less to hit health
			bool armorCheck;

			if ( !nDirect )
			{
				if ( Stats.Armor > 0 )
				{
					int armorLast = Stats.Armor;
					armorCheck = Stats.ReduceArmor( (int)(modDmg * ARMOR_ABSORB) );
					if ( armorCheck )
						modDmg = modDmg - (armorLast - Stats.Armor);
				}
			}

			return Stats.ReduceHealth( modDmg );
		}


		// Update is called once per frame
		void Update()
		{

		}

		public override void CleanUp( bool full = true )
		{
			base.CleanUp( full );
			UnsubActions();
		}

		public override void OnDestroy()
		{
			if ( !_disposed )
				CleanUp();
		}
		#endregion LiveEntity methods
	}
}
