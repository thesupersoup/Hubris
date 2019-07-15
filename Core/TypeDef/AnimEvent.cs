namespace Hubris
{
	/// <summary>
	/// Enumerator for events to be triggered by animations and sent to an AnimEventHandler
	/// </summary>
	public enum AnimEvent
	{
		/// <summary>
		/// Should not trigger anything in particular
		/// </summary>
		NONE = 0,

		/// <summary>
		/// Walking or sneaking footstep
		/// </summary>
		STEP_SOFT = 1,

		/// <summary>
		/// Running footstep
		/// </summary>
		STEP_LOUD = 2,

		/// <summary>
		/// Triggers a ShakeEmitter to spawn for client-side screen shake
		/// </summary>
		SHAKE = 3,

		/// <summary>
		/// Triggers an Idle sound
		/// </summary>
		IDLE = 4,

		/// <summary>
		/// Triggers an Attack sound
		/// </summary>
		ATK = 5,

		/// <summary>
		/// Triggers a Hurt sound
		/// </summary>
		HURT = 6,

		/// <summary>
		/// Triggers the active weapon's ReloadOut sound
		/// </summary>
		RELOAD_OUT = 7,

		/// <summary>
		/// Triggers the active weapon's ReloadIn sound
		/// </summary>
		RELOAD_IN = 8,

		/// <summary>
		/// Triggers the active weapon's ReloadSingle sound
		/// </summary>
		RELOAD_SINGLE = 9,

		/// <summary>
		/// Triggers the active weapon's ReloadCharge sound
		/// </summary>
		RELOAD_CHARGE = 10,

		/// <summary>
		/// Triggers the active weapon's WeaponHandle sound
		/// </summary>
		WEAPON_HANDLE = 11
	}
}