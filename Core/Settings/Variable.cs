namespace Hubris
{
	/// <summary>
	/// Represents a variable which can be accessed and modified through the console
	/// </summary>
	public sealed class Variable
	{
		///---------------------------------------------------------------------
		/// Variable Instance variables
		///--------------------------------------------------------------------- 

		private object _data;

		///---------------------------------------------------------------------
		/// Variable properties
		///--------------------------------------------------------------------- 

		public string Name { get; }
		public VarData DataType { get; }
		public VarType Type { get; }
		public bool Dirty { get; private set; }
		public object InitData { get; private set; }
		public object Data
		{
			get { return _data; }
			set
			{
				if (value is string temp)
				{
					switch (DataType)
					{
						case VarData.BOOL:
							if (bool.TryParse(temp, out bool b))
							{
								_data = b;
								Dirty = true;
							}
							else if (int.TryParse(temp, out int iB))
							{
								if (iB > 0)
									_data = true;
								else
									_data = false;
								Dirty = true;
							}
							else if (float.TryParse(temp, out float fB))
							{
								if (fB > 0.0f)
									_data = true;
								else
									_data = false;
								Dirty = true;
							}
							break;
						case VarData.FLOAT:
							if (float.TryParse(temp, out float f))
							{
								_data = f;
								Dirty = true;
							}
							break;
						case VarData.INT:
							if (int.TryParse(temp, out int i))
							{
								_data = i;
								Dirty = true;
							}
							break;
						case VarData.STRING:
						case VarData.OBJECT:
						default:
							_data = value;
							Dirty = true;
							break;
					}
				}
				else
				{
					// Check if value matches DataType before boxing
					switch (DataType)
					{
						case VarData.BOOL:
							if (value is bool b)
							{
								_data = b;
								Dirty = true;
							}
							else if (value is int iB)
							{
								if (iB > 0)
									_data = true;
								else
									_data = false;
								Dirty = true;
							}
							else if (value is float fB)
							{
								if (fB > 0.0f)
									_data = true;
								else
									_data = false;
								Dirty = true;
							}
							break;
						case VarData.FLOAT:
							if (value is float f)
							{
								_data = f;
								Dirty = true;
							}
							break;
						case VarData.INT:
							if (value is int i)
							{
								_data = i;
								Dirty = true;
							}
							break;
						case VarData.STRING:
							if (value is string s)
							{
								_data = s;
								Dirty = true;
							}
							break;
						case VarData.OBJECT:
						default:
							_data = value;
							Dirty = true;
							break;
					}
				}
			}
		}

		public Variable()
		{
			Name = "DefaultVarName";
			DataType = VarData.BOOL;
			Type = VarType.None;
			Dirty = true;
			Data = false;
			InitData = Data;
		}

		public Variable(string sName = "DefaultVarName", VarData eData = VarData.BOOL, VarType eType = VarType.None, object oData = null)
		{
			Name = sName;
			DataType = eData;
			Type = eType;
			Dirty = true;
			Data = oData;
			InitData = Data;
		}
   
		/// <summary>
		/// Set Dirty flag to false
		/// </summary>
		public void CleanVar()
		{
			Dirty = false;
		}

		/// <summary>
		/// Reset a variable to its default value
		/// </summary>
		public void ResetVar()
		{
			Data = InitData;
			Dirty = true;
		}

		public static void DisplayVarHelp( Variable nVar )
		{
			string helpStr = "";

			switch ( nVar.Type )
			{
				case VarType.Sens:
					helpStr += "Mouse sensitivity, both X and Y";
					break;
				case VarType.Useaccel:
					helpStr += "Enable acceleration values for player movement";
					break;
				case VarType.Debug:
					helpStr += "Enable debug mode and verbose console logging";
					break;
				default:
					helpStr += "Enter a variable name and value (e.g. \"setvar sens 1.5\")";
					break;
			}

			if ( nVar.Type != VarType.None )
			{
				helpStr += " "; // Add space before data type

				switch ( nVar.DataType )
				{
					case VarData.BOOL:
						helpStr += "[Boolean]";
						break;
					case VarData.FLOAT:
						helpStr += "[Float]";
						break;
					case VarData.INT:
						helpStr += "[Integer]";
						break;
					case VarData.STRING:
						helpStr += "[String]";
						break;
					case VarData.OBJECT:
						helpStr += "[Object]";
						break;
				}
			}

			LocalConsole.Instance.Log( helpStr );
		}
	}
}
