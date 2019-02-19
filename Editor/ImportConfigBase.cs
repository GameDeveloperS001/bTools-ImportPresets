using System.Collections.Generic;
using System.IO;
using System.Text;
using bTools.CodeExtensions;
using UnityEditor;
using UnityEngine;
using UnityEditor.Presets;

namespace bTools.ImportPresets
{
    public abstract class ImportConfigBase
    {
        //GUI settings
        protected bool folded = false;
        private char[] filterSep = new char[] { ';' };
        private string matchingPaths;

        //Utilities
        [HideInInspector] public bool tagForDeletion = false;
        [HideInInspector] public int positionOffset = 0;
        [SerializeField] public string saveName = "New Preset";
        [SerializeField] public string pathNameFilter = string.Empty;
        [SerializeField] public string fileNameFilter = string.Empty;
        [SerializeField] public bool isEnabled = true;
        [SerializeField] public Preset targetPreset;

        Editor cachedPresetEditor;

        public abstract void DrawInnerGUI();

        protected void DrawFilterGUI()
        {
            EditorGUILayout.LabelField("Preset Settings", EditorStyles.boldLabel);
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), ImportSettingsWindow.HeaderSeparatorColor);
            GUILayout.Space(3);

            EditorGUI.BeginChangeCheck();
            saveName = EditorGUILayout.TextField(new GUIContent("Name", "Only used for organisation"), saveName);
            EditorGUI.BeginChangeCheck();
            pathNameFilter = EditorGUILayout.DelayedTextField(new GUIContent("Path Contains Filter", "Applied only if the path contains this string. Leave empty to apply to all paths. Separate multiple filters with ;"), pathNameFilter);
            fileNameFilter = EditorGUILayout.DelayedTextField(new GUIContent("Filename Contains Filter", "Applied only if the filename contains this string. Leave empty to apply to all filenames. Separate multiple filters with ;"), fileNameFilter);
            if (EditorGUI.EndChangeCheck())
            {
                pathNameFilter = pathNameFilter.Replace('/', '\\');
                fileNameFilter = fileNameFilter.Replace('/', '\\');
                matchingPaths = GetMatchingPaths();
            }

            //EditorGUILayout.HelpBox(matchingPaths, MessageType.Info);
            EditorGUILayout.LabelField(matchingPaths, EditorStyles.helpBox);

            targetPreset = EditorGUILayout.ObjectField("Preset", targetPreset, typeof(Preset), false) as Preset;
            if (targetPreset != null)
            {
                Editor.CreateCachedEditor(targetPreset, null, ref cachedPresetEditor);
                if (cachedPresetEditor != null) cachedPresetEditor.OnInspectorGUI();
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(ImportProcessing.ImportSettingsData);
            }
        }


        protected string GetMatchingPaths()
        {
            if (pathNameFilter.Length == 0 && fileNameFilter.Length == 0)
            {
                return "Empty filters means this will be applied to everything";
            }

            StringBuilder matchingPaths = new StringBuilder();
            foreach (var item in Directory.GetDirectories(Application.dataPath, "*", SearchOption.AllDirectories))
            {
                if (PathFilterTest(item))
                {
                    matchingPaths.Append("Assets" + item.Replace(Application.dataPath, string.Empty) + "\r\n");
                }
            }

            if (matchingPaths.Length == 0)
            {
                matchingPaths.Append("No match");
            }

            return matchingPaths.ToString();
        }

        internal bool FilenameFilterTest(string path)
        {
            bool filenameTest = false;

            // Split string into multiple filters
            string[] filenameFilterSplit = fileNameFilter.Split(filterSep, System.StringSplitOptions.RemoveEmptyEntries);
            string fileName = Path.GetFileName(path);

            //Check if filter is empty
            if (fileNameFilter == string.Empty)
            {
                filenameTest = true;
            }

            // Check if filename contains what we want
            for (int i = 0; i < filenameFilterSplit.Length; i++)
            {
                if (path.Contains(filenameFilterSplit[i]))
                {
                    filenameTest = true;
                }
            }

            return filenameTest;
        }

        internal bool PathFilterTest(string path)
        {
            bool pathTest = false;

            // Split string into multiple filters
            string[] pathFilterSplit = pathNameFilter.Split(filterSep, System.StringSplitOptions.RemoveEmptyEntries);

            //Check if filter is empty
            if (pathNameFilter == string.Empty)
            {
                pathTest = true;
            }

            // Check if path contains what we want
            for (int i = 0; i < pathFilterSplit.Length; i++)
            {
                if (path.Contains(pathFilterSplit[i]))
                {
                    pathTest = true;
                }
            }

            return pathTest;
        }
    }
}
