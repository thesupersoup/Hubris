using UnityEngine;
using UnityEditor;

namespace Hubris
{
	/// <summary>
	/// Editor UI and related code for creating Commands
	/// </summary>
	public class CommandEditorUI : EditorWindow
	{
		[SerializeField]
		private Command _cmdArr;
		private string _uiName;				// Name to be used in UI and other human-readable contexts
		private string _cmdName;			// Name to be used when invoking the Command in console or config. Should always be lowercase.
		private CmdType _type;				// What type of Command is it? See CmdType enum
		private string _data;				// Data to be used as values for Commands
		private bool _cont;					// Continuous key (true), or only on key down (false)?

		[MenuItem( "Window/Commands" )]
		public static void ShowWindow()
		{
			GetWindow( typeof( CommandEditorUI ) );
		}

		void OnGUI()
		{
			/* Start Add Command Area */
			GUILayout.BeginArea( new Rect( 16, 8, Screen.width - 32, 512 ) );
			GUILayout.Label( "Add a Command:", EditorStyles.boldLabel );

			_uiName = EditorGUILayout.TextField( new GUIContent( "Default UI Name", "Name to be used in UI and other human-readable contexts" ), _uiName );
			_cmdName = EditorGUILayout.TextField( new GUIContent( "Default Cmd Name", "Name to be used when invoking the Command in console or config. Should always be lowercase." ), _cmdName );
			_type = (CmdType)EditorGUILayout.EnumFlagsField( new GUIContent( "Command Type", "What type of Command is it? See CmdType enum" ), _type );
			_data = EditorGUILayout.TextField( new GUIContent( "Command Metadata", "Data to be used as values for Commands" ), _data );
			_cont = EditorGUILayout.Toggle( new GUIContent( "Is Continuous?", "Continuous key (true), or only on key down (false)?" ), _cont );

			GUILayout.Space( 8 );

			if ( GUILayout.Button( new GUIContent( "Add Command", "Add Command to the static Command array" ) ) )
			{
				AddCommand( _uiName, _cmdName, _type, _data, _cont );
			}

			GUILayout.EndArea();
			/* End Add Command Area */
		}

		private void AddCommand( string ui, string name, CmdType type, string data, bool cont )
		{
			Debug.Log( "Command Added! (\"" + ui + "\" | \"" + name + "\" | " + type + " | \"" + data + "\" | " + cont + ")" );
		}
	}
}
