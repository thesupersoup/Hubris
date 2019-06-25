namespace Hubris
{
	/// <summary>
	/// A wrapper for messages logged to the console, along with the issuing commands
	/// </summary>
	public class Msg 
	{
		// Msg instance variables
		private int _id;        // ID # of Msg, for referencing specific Msgs when desired
		private string _txt;    // The text of the message, to be displayed in the LocalConsole

		// Msg properties
		public int Id => _id;
		public string Txt => _txt;

		public int Length
		{
			get { if (_txt != null) { return _txt.Length; } else { return 0; } }
		}

		// Msg methods
		public Msg(int nId, string nTxt)
		{
			_id = nId;
			_txt = nTxt;
		}
	}
}
