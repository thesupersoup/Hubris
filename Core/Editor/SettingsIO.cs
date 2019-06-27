using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Class for instanting objects to read/write Hubris settings
	/// </summary>
	internal class SettingsIO
	{
		public const string FILE_EXT = ".hub";
		public const string DEFAULT_PATH = "\\Hubris\\Settings";
		private string _path;

		public string Path
		{
			get { return _path; }
			set { _path = value; }
		}


		public SettingsIO()
		{
			Path = DEFAULT_PATH;
		}

		public SettingsIO(string nPath, bool nRelative = false, string nRoot = "\\Hubris")
		{
			if (!nRelative)
			{
				Path = nPath;
			}
			else
			{
				Path = nRoot + nPath;
			}
		}

		public void ReadSettings()
		{
			if(File.Exists(Path))
			{

			}
			else
			{
				Debug.Log("SettingsIO ReadSettings(): ERROR file not found in directory [" + Path + "]");
			}
		}

	}
}
