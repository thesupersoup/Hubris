﻿namespace Hubris
{
	/// <summary>
	/// Hubris version info
	/// </summary>
	public static class Version
	{
		public const int major = 0,
							minor = 7,
							patch = 0;

		public static int Major => major;
		public static int Minor => minor;
		public static char Patch => (char)(patch + 'a');

		/// <summary>
		/// Fetch the Hubris version as a string
		/// </summary>
		public static string GetString() =>
			Major + "." + Minor + Patch;
	}
}