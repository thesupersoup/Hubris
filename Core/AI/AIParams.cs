using System;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Represents specific parameters for AI behavior to use
	/// </summary>
	[Serializable]
	[CreateAssetMenu(fileName = "NewAIParams", menuName = "Hubris/Params/AIParams", order = 0)]
	public class AIParams : ScriptableObject
	{
		/// All Distances are in default Unity units unless otherwise specified
		/// It seems to be approximately 1 meter per unit, but shouldn't be assumed

		/// TAKE NOTE:
		/// When applicable, public properties return the parameter with any
		/// pertinent modifiers applied

		///--------------------------------------------------------------------
		/// AI constants 
		///--------------------------------------------------------------------

		public const float LOOK_ANGLE_MAX = 70.0f;   // Maximum amount (in degrees) of variance when using IK to look at a target

		/// Constants for the [Range] Attribute maximums below
		public const float SMALL_RANGE = 1.0f, LOW_MED_RANGE = 5.0f, HIGH_MED_RANGE = 10.0f, LARGE_RANGE = 100.0f, ROT_ANGLE_MAX = 179.0f;
		public const int MIN_PTS = 1, MAX_PTS = 10;

		/// Constants for default values
		public const float DEF_AWARE_MAX = 10.0f, DEF_AWARE_MED = 5.0f, DEF_AWARE_CLOSE = 2.5f, DEF_ATK_DIST = 2.0f, DEF_STOP_DIST = 1.0f,
							DEF_CHK_IDLE = 0.5f, DEF_CHK_ALERT = 0.33f, DEF_MOVE_SPD = 14.0f, DEF_MOVE_WALK = 0.36f,
							DEF_ROAM_DIST = 10.0f, DEF_ROAM_TIME = 10.0f, DEF_PROX_PCT = 0.8f, DEF_ROT_ANGLE = 40.0f, DEF_ROT_SPD = 0.15f,
							DEF_FLEE_DIST = 10.0f, FLEE_DIVISOR = 2.0f, WARY_DIVISOR = 4.0f;

		/// Constants for modifiers
		public const float MOD_MAX = 1.0f, DEF_MOD = 1.0f, 
							AWARE_MOD_IMPAIRED = 0.6f,
							ROT_ANGLE_MOD_NARROW = 0.01f;

		///--------------------------------------------------------------------
		/// Serialized instance variables for Editor
		///--------------------------------------------------------------------

		[SerializeField]
		[Tooltip( "What type of Npc is this? Used for squad and predatory behavior" )]
		private NpcType _type;
		[SerializeField]
		[Tooltip( "Max awareness distance (Npc will ignore objects of interest beyond it, unless specifically handled) [with AwareMod coefficient]" )]
		[Range( 0.0f, LARGE_RANGE )]
		private float _awareMax;
		[SerializeField]
		[Tooltip( "Medium awareness distance for certain state changes [with AwareMod coefficient]" )]
		[Range( 0.0f, LARGE_RANGE )]
		private float _awareMed;
		[SerializeField]
		[Tooltip( "Close awareness distance for certain state changes [with AwareMod coefficient]" )]
		[Range( 0.0f, LARGE_RANGE )]
		private float _awareClose;
		[SerializeField]
		[Tooltip( "Attack range" )]
		[Range( 0.0f, LARGE_RANGE )]
		private float _atkDist;
		[SerializeField]
		[Tooltip( "Stop moving toward target when at or closer than this distance" )]
		[Range( 0.0f, LARGE_RANGE )]
		private float _stopDist;
		[SerializeField]
		[Tooltip( "Area scan frequency when idle, in seconds; set higher to increase performance" )]
		[Range( 0.0f, LOW_MED_RANGE )]
		private float _chkIdle;
		[SerializeField]
		[Tooltip( "Area scan frequency when alert, in seconds; set higher to increase performance" )]
		[Range( 0.0f, LOW_MED_RANGE )]
		private float _chkAlert;
		[SerializeField]
		[Tooltip( "Base movement speed value" )]
		[Range( 0.0f, LARGE_RANGE )]
		private float _moveSpd;
		[SerializeField]
		[Tooltip( "Coefficient of base movement speed when walking" )]
		[Range( 0.0f, MOD_MAX )]
		private float _moveWalk;
		[SerializeField]
		[Tooltip( "Should this Agent roam when bored?" )]
		private bool _roam;
		[SerializeField]
		[Tooltip( "Maximum radius to search for a position when roaming, patrolling, or fleeing" )]
		[Range( 0.0f, LARGE_RANGE )]
		private float _roamDist;
		[SerializeField]
		[Tooltip( "Time to wait between position searches while roaming or patrolling, in seconds" )]
		[Range( 0.0f, LARGE_RANGE )]
		private float _roamTime;
		[SerializeField]
		[Tooltip( "Number of possible roam points to consider when roaming; set lower to increase performance" )]
		[Range( MIN_PTS, MAX_PTS )]
		private int _roamPts;
		[SerializeField]
		[Tooltip( "Will this Npc hunt targets?" )]
		private bool _predator;
		[SerializeField]
		[Tooltip( "Will this Npc attack if target gets too close, regardless of predatory status?" )]
		private bool _territorial;
		[SerializeField]
		[Tooltip( "Will this Npc coordinate with others of the same type?" )]
		private bool _squad;
		[SerializeField]
		[Tooltip( "Should this Npc retarget when one threat is closer than another?" )]
		private bool _retarget;
		[SerializeField]
		[Tooltip( "When threatened, if another threat moves closer than this coefficient of the distance between the Npc and it's current threat, it may consider retargeting" )]
		[Range( 0.0f, SMALL_RANGE )]
		private float _proxPct;
		[SerializeField]
		[Tooltip( "Speed coefficient for this Npc's rotations" )]
		[Range( 0.0f, LOW_MED_RANGE )]
		private float _rotSpd;
		[SerializeField]
		[Tooltip( "Maximum angle between an Npc's forward vector and the vector to target's position [with RotAngleMod coefficient]" )]
		[Range( 0.0f, ROT_ANGLE_MAX )]
		private float _rotAngle;
		[SerializeField]
		[Tooltip( "Represents the default angle of the Npc's field-of-view cone" )]
		private FOVDegrees _fov;
		[Tooltip( "Animation delay for better synchronization with actions (seconds)" )]
		[Range( 0.0f, LOW_MED_RANGE )]
		private float _animDelay;
		[SerializeField]
		[Tooltip( "Time between the beginning of an attack animation and the damage check (seconds)" )]
		[Range( 0.0f, LOW_MED_RANGE )]
		private float _atkInit;
		[SerializeField]
		[Tooltip( "Time after the damage check to wait before another attack can begin (seconds)" )]
		[Range( 0.0f, LOW_MED_RANGE )]
		private float _atkEnd;

		///--------------------------------------------------------------------
		/// Parameter modifiers
		/// e.g. awareness modifier can be adjusted when compromised
		/// (smoke, flashbang)
		///--------------------------------------------------------------------

		[SerializeField]
		[Tooltip( "[Coefficient, float] Modifier for awareness distances" )]
		[Range( 0.0f, MOD_MAX )]
		private float _awareMod;

		///--------------------------------------------------------------------
		/// Npc Type
		///--------------------------------------------------------------------

		public NpcType Type { get { return _type; } protected set { _type = value; } }

		///--------------------------------------------------------------------
		/// Distances for behavior state changes and area scan variables
		///--------------------------------------------------------------------

		/// <summary>
		/// Max awareness distance (Npc will ignore objects of interest beyond it, unless specifically handled)
		/// [Returns AwareMax with AwareMod applied]
		/// </summary>
		public float AwareMax { get { return _awareMax * _awareMod; } protected set { _awareMax = value; } }

		/// <summary>
		/// Medium awareness distance for certain state changes 
		/// [Returns AwareMed with AwareMod applied]
		/// </summary>
		public float AwareMed { get { return _awareMed * _awareMod; } protected set { _awareMed = value; } }

		/// <summary>
		/// Close awareness distance for certain state changes
		/// [Returns AwareClose with AwareMod applied]
		/// </summary>
		public float AwareClose { get { return _awareClose * _awareMod; } protected set { _awareClose = value; } }
        
		/// <summary>
		/// Attack range
		/// </summary>
		public float AtkDist { get { return _atkDist; } protected set { _atkDist = value; } }
        
		/// <summary>
		/// Stop moving toward target when at or closer than this distance
		/// </summary>
		public float StopDist { get { return _stopDist; } protected set { _stopDist = value; } }

		/// <summary>
		/// Area scan frequency when idle, in seconds; set higher to increase performance
		/// </summary>
		public float ChkIdle { get { return _chkIdle; } protected set { _chkIdle = value; } }

		/// <summary>
		/// Area scan frequency when alert, in seconds; set higher to increase performance
		/// </summary>
		public float ChkAlert { get { return _chkAlert; } protected set { _chkAlert = value; } }

		///--------------------------------------------------------------------
		/// Movement speed properties
		///--------------------------------------------------------------------

		/// <summary>
		/// Base movement speed value
		/// </summary>
		public float MoveSpd { get { return _moveSpd; } protected set { _moveSpd = value; } }

		/// <summary>
		/// Coefficient of base movement speed when walking
		/// </summary>
		public float MoveWalk { get { return _moveWalk; } protected set { _moveWalk = value; } }

		///--------------------------------------------------------------------
		/// Roaming behavior properties
		///--------------------------------------------------------------------

		/// <summary>
		/// Should this Agent roam when bored?
		/// </summary>
		public bool Roam { get { return _roam; } protected set { _roam = value; } }
        
		/// <summary>
		/// Maximum radius to search for a position when roaming or patrolling
		/// </summary>
		public float RoamDist { get { return _roamDist; } protected set { _roamDist = value; } }

		/// <summary>
		/// Time to wait between position searches while roaming or patrolling, in seconds
		/// </summary>
		public float RoamTime { get { return _roamTime; } protected set { _roamTime = value; } }

		/// <summary>
		/// Number of possible roam points to consider when roaming; set lower to increase performance
		/// </summary>
		public int RoamPts { get { return _roamPts; } protected set { _roamPts = value; } }

		///--------------------------------------------------------------------
		/// Additional behavior properties
		///--------------------------------------------------------------------

		/// <summmary>
		/// Will this Npc hunt targets?
		/// </summmary>
		public bool Predator { get { return _predator; } protected set { _predator = value; } }

		/// <summary>
		/// Will this Npc attack if target gets too close, regardless of predatory status?
		/// </summary>
		public bool Territorial { get { return _territorial; } protected set { _territorial = value; } }

		/// <summary>
		/// Will this Npc coordinate with others of the same type?
		/// </summary>
		public bool Squad { get { return _squad; } protected set { _squad = value; } }

		/// <summary>
		/// Should this Npc retarget when one threat is closer than another?
		/// </summary>
		public bool Retarget { get { return _retarget; } protected set { _retarget = value; } }
        
		/// <summary>
		/// When threatened, if another threat moves closer than this percentage (as a float, max 1.0f)
		/// of the distance between the Agent and it's current threat, it may consider retargeting
		/// </summary>
		public float ProxPct { get { return _proxPct; } protected set { _proxPct = value; } }

		/// <summary>
		/// Speed coefficient for this Npc's rotations
		/// </summary>
		public float RotSpd { get { return _rotSpd; } protected set { _rotSpd = value; } }

		/// <summary>
		/// Maximum angle between an Npc's forward vector and the vector to target's position
		/// [Returns RotAngle with RotAngleMod applied]
		/// </summary>
		public float RotAngle { get { return _rotAngle; } protected set { _rotAngle = value; } }

		/// <summary>
		/// Represents the default angle of the Npc's field-of-view cone (degrees, enum)
		/// </summary>
		public FOVDegrees FOV { get { return _fov; } protected set { _fov = value; } }

		/// <summary>
		/// Animation delay for better synchronization with actions (seconds)
		/// </summary>
		public float AnimDelay { get { return _animDelay; } protected set { _animDelay = value; } }
        
		/// <summary>
		/// Time between the beginning of an attack animation and the damage check (seconds)
		/// </summary>
		public float AtkInit { get { return _atkInit; } protected set { _atkInit = value; } }

		/// <summary>
		/// Time after the damage check to wait before another attack can begin (seconds)
		/// </summary>
		public float AtkEnd { get { return _atkEnd; } protected set { _atkEnd = value; } }

		///--------------------------------------------------------------------
		/// Parameter modifiers
		///--------------------------------------------------------------------
        
		/// <summary>
		/// [Coefficient, float] Modifier for awareness distances
		/// </summary>
		public float AwareMod { get { return _awareMod; } protected set { _awareMod = value; } }

		///--------------------------------------------------------------------
		/// Set methods
		///--------------------------------------------------------------------

		/// <summary>
		/// What type of Npc is this? Used for squad and predatory behavior
		/// </summary>
		public void SetType( NpcType nType )
		{
			Type = nType;
		}

		/// <summary>
		/// [Distance, float] Should be greater than 0
		/// </summary>
		public void SetAwareMax( float nAware )
		{
			if ( nAware > 0.0f )
				AwareMax = nAware;
		}

		/// <summary>
		/// [Distance, float] Should be less than AwareMax
		/// </summary>
		public void SetAwareMed( float nMed )
		{
			if ( nMed < AwareMax )
				AwareMed = nMed;
		}

		/// <summary>
		/// [Distance, float] Should be less than AwareMed
		/// </summary>
		public void SetAwareClose( float nClose )
		{
			if ( nClose < AwareMed )
				AwareClose = nClose;
		}

		/// <summary>
		/// [Distance, float] Should be positive
		/// </summary>
		public void SetAtkDist( float nAtk )
		{
			if ( nAtk >= 0.0f )
				AtkDist = nAtk;
		}

		/// <summary>
		/// [Distance, float] Should be positive
		/// </summary>
		public void SetStopDist( float nStop )
		{
			if ( nStop >= 0.0f )
				StopDist = nStop;
		}

		/// <summary>
		/// [Seconds, float] Should be positive
		/// </summary>
		public void SetChkIdle( float nIdle )
		{
			if ( nIdle >= 0.0f )
				ChkIdle = nIdle;
		}

		/// <summary>
		/// [Seconds, float] Should be positive
		/// </summary>
		public void SetChkAlert( float nAlert )
		{
			if ( nAlert >= 0.0f )
				ChkAlert = nAlert;
		}

		/// <summary>
		/// [Speed, float] Should be positive
		/// </summary>
		public void SetMoveSpeed( float nSpd )
		{
			if ( nSpd >= 0.0f )
				MoveSpd = nSpd;
		}

		/// <summary>
		/// [Speed coefficient, float] Should be greater than 0 and less than max
		/// </summary>
		public void SetMoveWalk( float nWalk )
		{
			if ( nWalk >= 0.0f && nWalk < MOD_MAX )
				MoveWalk = nWalk;
		}

		/// <summary>
		/// [bool] Should this Agent roam when bored?
		/// </summary>
		public void SetRoam( bool nRoam )
		{
			Roam = nRoam;
		}

		/// <summary>
		/// [Distance, float] Should be positive
		/// </summary>
		public void SetRoamDist( float nRoam )
		{
			if ( nRoam >= 0.0f )
				RoamDist = nRoam;
		}

		/// <summary>
		/// [Seconds, float] Should be positive
		/// </summary>
		public void SetRoamTime( float nTime )
		{
			if ( nTime >= 0.0f )
				RoamTime = nTime;
		}

		/// <summary>
		/// [int] Should be greater than MIN_PTS and less than MAX_PTS
		/// </summary>
		public void SetRoamPts( int nPts )
		{
			if ( nPts >= MIN_PTS && nPts <= MAX_PTS )
				RoamPts = nPts;
			else
			{
				if ( nPts < MIN_PTS )
					RoamPts = MIN_PTS;
				else // nPts > MAX_PTS
					RoamPts = MAX_PTS;
			}
		}

		/// <summary>
		/// [bool] Will this Npc hunt targets?
		/// </summary>
		public void SetPredator( bool nPred )
		{
			Predator = nPred;
		}

		/// <summary>
		/// [bool] Will this Npc attack if target gets too close, regardless of predatory status?
		/// </summary>
		public void SetTerritorial( bool nTerr )
		{
			Territorial = nTerr;
		}

		/// <summary>
		/// [bool] Will this Npc coordinate with others of the same type?
		/// </summary>
		public void SetSquad( bool nSquad )
		{
			Squad = nSquad;
		}

		/// <summary>
		/// [bool] Should this Npc retarget when one threat is closer than another?
		/// </summary>
		public void SetRetarget( bool nRe )
		{
			Retarget = nRe;
		}

		/// <summary>
		/// [Percentage (as decimal), float] Should be greater than 0
		/// </summary>
		public void SetProxPct( float nProx )
		{
			if ( nProx > 0.0f )
				ProxPct = nProx;
		}

		/// <summary>
		/// [Angle, float] Should be positive and within constraints; will set accordingly otherwise
		/// </summary>
		public void SetRotAngle( float nAng )
		{
			if ( nAng >= 0.0f && nAng <= ROT_ANGLE_MAX )
				_rotAngle = nAng;
			else
			{
				if ( nAng < 0.0f )
					_rotAngle = 0.0f;
				else    // nAng > ROT_ANGLE_MAX
					_rotAngle = ROT_ANGLE_MAX;
			}
		}

		/// <summary>
		/// [Angle, degrees (enum)] Should be less than the total number of elements in the enum
		/// </summary>
		public void SetFOV( FOVDegrees nDeg )
		{
			if ( nDeg < FOVDegrees.NUM_FOVS )
				_fov = nDeg;
		}

		/// <summary>
		/// [Coefficient, float] Should be greater than 0
		/// </summary>
		public void SetRotSpd( float nSpd )
		{
			if ( nSpd > 0.0f )
				RotSpd = nSpd;
		}

		/// <summary>
		/// [Seconds, float] Should be positive
		/// </summary>
		public void SetAnimDelay( float nDelay )
		{
			if ( nDelay >= 0.0f )
				AnimDelay = nDelay;
		}

		/// <summary>
		/// [Seconds, float] Should be positive 
		/// </summary>
		public void SetAtkInit( float nInit )
		{
			if ( nInit >= 0.0f )
				AtkInit = nInit;
		}

		/// <summary>
		/// [Seconds, float] Should be positive and larger than or equal to AtkInit
		/// </summary>
		public void SetAtkEnd( float nEnd )
		{
			if ( nEnd >= AtkInit )
				AtkEnd = nEnd;
		}

		/// <summary>
		/// [Percentage (as decimal), float] Should be greater than 0 and less than max; will set accordingly otherwise
		/// </summary>
		public void SetAwareMod( float nMod )
		{
			if ( nMod >= 0.0f && nMod <= MOD_MAX )
				AwareMod = nMod;
			else
			{
				if ( nMod < 0.0f )
					AwareMod = 0.0f;
				else // nMod > MOD_MAX
					AwareMod = MOD_MAX;
			}
		}

		///--------------------------------------------------------------------
		/// AIParams methods
		///--------------------------------------------------------------------

		public AIParams()
		{
			AwareMod = DEF_MOD;
		}

		public AIParams( float dMax, float dMed, float dClose, float dAtk, float dStop, float tIdle, float tAlert, float nSpd, float nWalk, bool bRoam, float dRoam, float tRoam, int nPts, bool bRe, float dProx, float rSpd, float animDelay, float atkInit, float atkEnd )
		{
			AwareMax = dMax;
			AwareMed = dMed;
			AwareClose = dClose;
			AtkDist = dAtk;
			StopDist = dStop;
			ChkIdle = tIdle;
			ChkAlert = tAlert;
			MoveSpd = nSpd;
			MoveWalk = nWalk;
			Roam = bRoam;
			RoamDist = dRoam;
			RoamTime = tRoam;
			RoamPts = nPts;
			Retarget = bRe;
			ProxPct = dProx;
			RotSpd = rSpd;
			AnimDelay = animDelay;
			AtkInit = atkInit;
			AtkEnd = atkEnd;
			AwareMod = DEF_MOD;
		}

		// To be implemented with a fluent interface and method chaining
		public static AIParams Default()
		{
			AIParams p = new AIParams();

			p.SetAwareMax( 80.0f );
			p.SetAwareMed( 50.0f );
			p.SetAwareClose( 25.0f );
			p.SetAtkDist( 4.25f );
			p.SetStopDist( 2.5f );
			p.SetChkIdle( 0.5f );
			p.SetChkAlert( 0.33f );
			p.SetMoveSpeed( 14.0f );
			p.SetMoveWalk( 0.36f );
			p.SetRoam( true );
			p.SetRoamDist( 20.0f );
			p.SetRoamTime( 10.0f );
			p.SetRoamPts( 5 );
			p.SetRetarget( true );
			p.SetProxPct( 0.8f );
			p.SetRotSpd( 0.065f );
			p.SetRotAngle( 50.0f );
			p.SetAnimDelay( 0.45f );
			p.SetAtkInit( 0.6f );
			p.SetAtkEnd( 1.8f );

			return p;
		}
	}
}
