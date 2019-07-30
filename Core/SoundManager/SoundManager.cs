using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Stores and plays sounds for entities. Allows for random or specific sound clip choice, and automatically chooses the appropriate source
	/// </summary>
	public class SoundManager : MonoBehaviour
	{
		public const float SND_PITCH_VAR = 0.2f;	// Amount to randomize (-/+) pitch for SoundRandom and SoundManager scripts 
		public const float SND_PITCH_BASE = 1.0f;	// Base sound pitch (default in Unity is 1)		
		public const float SND_DIST_MOD = 0.8f;		// Coefficient of AudioSource.maxDistance for owning Npc's EmitSoundEvent

		private bool init = false;

		/* AudioClip arrays to store sounds for either random or index play, and variables to store the last AudioClip played for each */
		/* Holders are the GameObjects on which the AudioSources are attached */
		/* Queues are for queuing up AudioClips to play sequentially without interrupting one another */

		#region SoundManager instance vars
		///--------------------------------------------------------------------
		/// SoundManager instance vars
		///--------------------------------------------------------------------
		[SerializeField]
		LiveEntity owner = null;
		[SerializeField]
		GameObject ownerObj = null;

		[SerializeField]
		private bool randomizePitch = false;
		private float pitchBase = SND_PITCH_BASE;

		[SerializeField]
		private AudioClip[] foot = null;
		private AudioClip lastFootSnd = null;
		[SerializeField]
		private AudioClip[] fall = null;
		[SerializeField]
		private GameObject footHolder = null;
		[SerializeField]
		private AudioSource footSource = null;
		List<AudioClip> footQueue;

		[SerializeField]
		private AudioClip[] idle = null;
		private AudioClip lastIdleSnd = null;
		[SerializeField]
		private AudioClip[] alert = null;
		[SerializeField]
		private AudioClip[] hunt = null;
		[SerializeField]
		private AudioClip[] aggro = null;
		[SerializeField]
		private AudioClip[] hurt = null;
		private AudioClip lastHurtSnd = null;
		[SerializeField]
		private AudioClip[] die = null;
		[SerializeField]
		private GameObject voxHolder = null;
		[SerializeField]
		private AudioSource voxSource = null;
		List<AudioClip> voxQueue;

		[SerializeField]
		private AudioClip[] atk = null;
		[SerializeField]
		private GameObject atkHolder = null;
		[SerializeField]
		private AudioSource atkSource = null;
		List<AudioClip> atkQueue;
		#endregion SoundManager instance vars

		#region SoundManager properties
		///--------------------------------------------------------------------
		/// SoundManager methods
		///--------------------------------------------------------------------
		
		public Vector3 FootPos => footHolder != null ? footHolder.transform.position : this.transform.position;

		public Vector3 VoxPos => voxHolder != null ? voxHolder.transform.position : this.transform.position;

		public Vector3 AtkPos => atkHolder != null ? atkHolder.transform.position : this.transform.position;

		/// <summary>
		/// Audio source for footsteps and ground-level sfx
		/// </summary>
		public AudioSource FootSrc
		{
			get
			{ return footSource; }
			protected set
			{ footSource = value; }
		}

		/// <summary>
		/// Audio source for voice sfx
		/// </summary>
		public AudioSource VoxSrc
		{
			get
			{ return voxSource; }
			protected set
			{ voxSource = value; }
		}

		/// <summary>
		/// Audio source for attack sfx
		/// </summary>
		public AudioSource AtkSrc
		{
			get
			{ return atkSource; }
			protected set
			{ atkSource = value; }
		}

		#endregion SoundManager properties

		#region SoundManager methods
		///--------------------------------------------------------------------
		/// SoundManager methods
		///--------------------------------------------------------------------

		public bool InitSoundManager( LiveEntity a, GameObject obj = null )
		{
			owner = a;
			ownerObj = obj;

			if ( !init )
			{
				init = true;

				if ( footSource != null && voxSource != null && atkSource != null )
					return init;

				if ( footHolder != null )
				{
					footSource = footHolder.GetComponent<AudioSource>();
					if ( footSource == null )
						Debug.LogWarning( this.gameObject + " SoundManager InitSoundManager(): footSource is null" );
				}
					Debug.LogWarning( this.gameObject + " SoundManager InitSoundManager(): footHolder is null, likely needs to be assigned in Editor" );

				if ( voxHolder != null )
				{
					voxSource = voxHolder.GetComponent<AudioSource>();
					if ( voxSource == null )
						Debug.LogWarning( this.gameObject + " SoundManager InitSoundManager(): voxSource is null" );
				}
				else
					Debug.LogWarning( this.gameObject + " SoundManager InitSoundManager(): voxHolder is null, likely needs to be assigned in Editor" );

				if ( atkHolder != null )
				{
					atkSource = atkHolder.GetComponent<AudioSource>();
					if ( atkSource == null )
						Debug.LogWarning( this.gameObject + " SoundManager InitSoundManager(): atkSource is null" );
				}
				else
					Debug.LogWarning( this.gameObject + " SoundManager InitSoundManager(): atkHolder is null, likely needs to be assigned in Editor" );
			}
			else
				Debug.LogWarning( this.gameObject + " SoundManager InitSoundManager(): Attempting to initialize SoundManager when it has already been initialized..." );

			footQueue = new List<AudioClip>();
			voxQueue = new List<AudioClip>();
			atkQueue = new List<AudioClip>();

			return init;
		}

		public bool GetInit()
		{
			return init;
		}

		public AudioSource GetSource( SndT t )
		{
			AudioSource src;

			switch ( t )
			{
				case SndT.FOOT:
				case SndT.FALL:
					src = footSource;
					break;
				case SndT.IDLE:
				case SndT.ALERT:
				case SndT.HUNT:
				case SndT.AGGRO:
				case SndT.HURT:
				case SndT.DIE:
					src = voxSource;
					break;
				case SndT.ATK:
					src = atkSource;
					break;
				case SndT.NONE:
					Debug.LogWarning( this.gameObject + " SoundManager GetSource(): SoundTrigger (SndT) NONE passed to GetSource(), mistake?" );
					src = null;
					break;
				default:
					Debug.LogError( this.gameObject + " SoundManager GetSource(): Invalid SoundTrigger (SndT) passed to GetSource()" );
					src = null;
					break;

			}

			return src;
		}

		public AudioClip GetRandomSound( SndT t )
		{
			AudioClip snd;

			switch( t )
			{
				case SndT.FOOT:
					snd = RandFoot();
					break;
				case SndT.FALL:
					snd = RandFall();
					break;
				case SndT.IDLE:
					snd = RandIdle();
					break;
				case SndT.ALERT:
					snd = RandAlert();
					break;
				case SndT.HUNT:
					snd = RandHunt();
					break;
				case SndT.AGGRO:
					snd = RandAggro();
					break;
				case SndT.HURT:
					snd = RandHurt();
					break;
				case SndT.DIE:
					snd = RandDie();
					break;
				case SndT.ATK:
					snd = RandAtk();
					break;
				case SndT.NONE:
				default:
					snd = null;
					break;
			}

			return snd;
		}

		public AudioClip GetSound( SndT t, int index )
		{
			AudioClip snd;

			switch ( t )
			{
				case SndT.FOOT:
					snd = Foot( index );
					break;
				case SndT.FALL:
					snd = Fall( index );
					break;
				case SndT.IDLE:
					snd = Idle( index );
					break;
				case SndT.ALERT:
					snd = Alert( index );
					break;
				case SndT.HUNT:
					snd = Hunt( index );
					break;
				case SndT.AGGRO:
					snd = Aggro( index );
					break;
				case SndT.HURT:
					snd = Hurt( index );
					break;
				case SndT.DIE:
					snd = Die( index );
					break;
				case SndT.ATK:
					snd = Atk( index );
					break;
				case SndT.NONE:
				default:
					snd = null;
					break;
			}

			return snd;
		}

		public bool CheckPlaying( SndT nSndT )
		{
			AudioSource src = GetSource( nSndT );

			if ( src != null )
				return src.isPlaying;
			else
				return false;
		}

		public void StopSound( SndT t )
		{
			AudioSource src = GetSource( t );

			if ( src != null )
				src.Stop();
			else
				Debug.LogWarning( this.gameObject + " SoundManager StopSound(): src is null" );
		}

		public AudioClip RandFoot()
		{
			if ( foot != null )
			{
				if ( foot.Length > 0 )
				{
					int index = Random.Range( 0, foot.Length );
					return foot[index];
				}
				else
					return null;
			}
			else
				return null;
		}

		public AudioClip Foot( int nIndex )
		{
			if ( foot != null )
			{
				if ( foot.Length > 0 )
				{
					if ( nIndex < foot.Length && nIndex >= 0 )
						return foot[nIndex];
					else
						return null;
				}
				else
					return null;
			}
			else
				return null;
		}

		public AudioClip RandFall()
		{
			if ( fall != null )
			{
				if ( fall.Length > 0 )
				{
					int index = Random.Range( 0, fall.Length );
					return fall[index];
				}
				else
					return null;
			}
			else
				return null;
		}

		public AudioClip Fall( int nIndex )
		{
			if ( fall != null )
			{
				if ( fall.Length > 0 )
				{
					if ( nIndex < fall.Length && nIndex >= 0 )
						return fall[nIndex];
					else
						return null;
				}
				else
					return null;
			}
			else
				return null;
		}

		public AudioClip RandIdle()
		{
			if ( idle != null )
			{
				if ( idle.Length > 0 )
				{
					int index = Random.Range( 0, idle.Length );
					return idle[index];
				}
				else
					return null;
			}
			else
				return null;
		}

		public AudioClip Idle( int nIndex )
		{
			if ( idle != null )
			{
				if ( idle.Length > 0 )
				{
					if ( nIndex < idle.Length && nIndex >= 0 )
						return idle[nIndex];
					else
						return null;
				}
				else
					return null;
			}
			else
				return null;
		}

		public AudioClip RandAlert()
		{
			if ( alert != null )
			{
				if ( alert.Length > 0 )
				{
					int index = Random.Range( 0, alert.Length );
					return alert[index];
				}
				else
					return null;
			}
			else
				return null;
		}

		public AudioClip Alert( int nIndex )
		{
			if ( alert != null )
			{
				if ( alert.Length > 0 )
				{
					if ( nIndex < alert.Length && nIndex >= 0 )
						return alert[nIndex];
					else
						return null;
				}
				else
					return null;
			}
			else
				return null;
		}

		public AudioClip RandHunt()
		{
			if ( hunt != null )
			{
				if ( hunt.Length > 0 )
				{
					int index = Random.Range( 0, hunt.Length );
					return hunt[index];
				}
				else
					return null;
			}
			else
				return null;
		}

		public AudioClip Hunt( int nIndex )
		{
			if ( hunt != null )
			{
				if ( hunt.Length > 0 )
				{
					if ( nIndex < hunt.Length && nIndex >= 0 )
						return hunt[nIndex];
					else
						return null;
				}
				else
					return null;
			}
			else
				return null;
		}

		public AudioClip RandAggro()
		{
			if ( aggro != null )
			{
				if ( aggro.Length > 0 )
				{
					int index = Random.Range( 0, aggro.Length );
					return aggro[index];
				}
				else
					return null;
			}
			else
				return null;
		}

		public AudioClip Aggro( int nIndex )
		{
			if ( aggro != null )
			{
				if ( aggro.Length > 0 )
				{
					if ( nIndex < aggro.Length && nIndex >= 0 )
						return aggro[nIndex];
					else
						return null;
				}
				else
					return null;
			}
			else
				return null;
		}

		public AudioClip RandHurt()
		{
			if ( hurt != null )
			{
				if ( hurt.Length > 0 )
				{
					int index = Random.Range( 0, hurt.Length );
					return hurt[index];
				}
				else
					return null;
			}
			else
				return null;
		}

		public AudioClip Hurt( int nIndex )
		{
			if ( hurt != null )
			{
				if ( hurt.Length > 0 )
				{
					if ( nIndex < hurt.Length && nIndex >= 0 )
						return hurt[nIndex];
					else
						return null;
				}
				else
					return null;
			}
			else
				return null;
		}

		public AudioClip RandAtk()
		{
			if ( atk != null )
			{
				if ( atk.Length > 0 )
				{
					int index = Random.Range( 0, atk.Length );
					return atk[index];
				}
				else
					return null;
			}
			else
				return null;
		}

		public AudioClip Atk( int nIndex )
		{
			if ( atk != null )
			{
				if ( atk.Length > 0 )
				{
					if ( nIndex < atk.Length && nIndex >= 0 )
						return atk[nIndex];
					else
						return null;
				}
				else
					return null;
			}
			else
				return null;
		}

		public AudioClip RandDie()
		{
			if ( die != null )
			{
				if ( die.Length > 0 )
				{
					int index = Random.Range( 0, die.Length );
					return die[index];
				}
				else
					return null;
			}
			else
				return null;
		}

		public AudioClip Die( int nIndex )
		{
			if ( die != null )
			{
				if ( die.Length > 0 )
				{
					if ( nIndex < die.Length && nIndex >= 0 )
						return die[nIndex];
					else
						return null;
				}
				else
					return null;
			}
			else
				return null;
		}

		/// <summary>
		/// Trigger a sound to play; will automatically choose correct source and return success boolean
		/// </summary>
		public bool TriggerSound( SndT t, float delay = 0.0f, bool rand = true, int index = 0 ) // Trigger an AudioClip to play directly
		{
			AudioClip clip;
			AudioSource src = GetSource( t );

			if ( rand )
				clip = GetRandomSound( t );
			else
				clip = GetSound( t, index );

			if ( clip == null )
			{ 
				// Debug.LogWarning( this.gameObject + " SoundManager TriggerSound(): Audio clip is null" );
				return false;
			}

			if ( src == null )
			{
				Debug.LogWarning( this.gameObject + " SoundManager TriggerSound(): Audio source is null" );
				return false;
			}

			if ( randomizePitch )
					src.pitch = Random.Range( pitchBase - SND_PITCH_VAR, pitchBase + SND_PITCH_VAR );

			if ( delay != 0.0f )
			{
				StartCoroutine( TriggerDelay( src, clip, delay ) );
				return true;
			}

			src.clip = clip;
			if ( src.gameObject.activeInHierarchy )
			{
				src.Play();
				CreateSoundEvent( owner, src, SoundIntensity.NOTEWORTHY );
				return true;
			}
			// else
			// Debug.LogWarning(this.gameObject + " SoundManager TriggerSound(): Attempting to play a null AudioClip through a valid AudioSource, nothing will play...");


			return false;
		}

		/// <summary>
		/// Creates a sound event and calls EmitSoundEvent on the specified ISoundEmitter; applies SND_DIST_MOD to the AudioSource.maxDistance
		/// </summary>
		public void CreateSoundEvent( LiveEntity ent, AudioSource src, SoundIntensity type = SoundIntensity.NORMAL )
		{
			if ( ent == null )
				return;

			if ( src == null )
				return;

			ent.EmitSoundEvent( new SoundEvent( ent, src.transform.position, src.maxDistance * SND_DIST_MOD, type ) );
		}

		IEnumerator TriggerDelay( AudioSource src, AudioClip clip, float delay )
		{
			yield return new WaitForSeconds( delay );
			src.clip = clip;
			src.Play();
			CreateSoundEvent( owner, src, SoundIntensity.NOTEWORTHY );
		}

		void StopSndCoroutine( string nName )
		{
			StopCoroutine( nName );
		}

		void StopAllSndCoroutines()
		{
			StopAllCoroutines();
		}

		void AnimEventHandler( int nEvent )
		{
			switch ( nEvent )
			{
				case 0: // Footstep
					TriggerSound( SndT.FOOT );
					break;
				case 1: // Body fall
					TriggerSound( SndT.FALL );
					break;
			}
		}
		#endregion SoundManager methods
	}
}
