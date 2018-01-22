using bTools.CodeExtensions;
using UnityEditor;
using UnityEngine;

namespace bTools.ImportPresets
{
	public abstract class ImportConfigBase
	{
		//GUI settings
		protected bool folded = false;

		//Utilities
		[HideInInspector]
		public bool tagForDeletion = false;
		[HideInInspector]
		public int positionOffset = 0;
		[SerializeField]
		public string saveName = "New Preset";
		[SerializeField]
		public string pathNameFilter = string.Empty;
		[SerializeField]
		public string fileNameFilter = string.Empty;
		[SerializeField]
		public bool isEnabled = true;

		public abstract void DrawInnerGUI();

		protected void DrawFilterGUI()
		{
			EditorGUILayout.LabelField( "Preset Settings", EditorStyles.boldLabel );
			EditorGUI.DrawRect( EditorGUILayout.GetControlRect( false, 2 ), Colors.DarkGrayX11 );
			GUILayout.Space( 3 );

			saveName = EditorGUILayout.TextField( new GUIContent( "Name", "Only used for organisation" ), saveName );
			pathNameFilter = EditorGUILayout.TextField( new GUIContent( "Path Contains Filter", "Applied only if the path contains this string. Leave empty to apply to all paths. Separate multiple filters with ;" ), pathNameFilter );
			fileNameFilter = EditorGUILayout.TextField( new GUIContent( "Filename Contains Filter", "Applied only if the filename contains this string. Leave empty to apply to all filenames. Separate multiple filters with ;" ), fileNameFilter );

			if ( pathNameFilter.Length == 0 && fileNameFilter.Length == 0 )
			{
				EditorGUILayout.HelpBox( "Empty filters means this will be applied to all imported meshes", MessageType.Info );
			}
		}
	}
}