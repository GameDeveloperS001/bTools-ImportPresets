using System.Collections.Generic;
using System.IO;
using System.Text;
using bTools.CodeExtensions;
using UnityEditor;
using UnityEngine;
using UnityEditor.Presets;
using UnityEditorInternal;
using System;

namespace bTools.ImportPresets
{
    [System.Serializable]
    public abstract class ImportConfigBase
    {
        //GUI settings
        protected bool folded = false;
        private char[] filterSep = new char[] { ';' };
        private string matchingPaths;
        Editor cachedPresetEditor;

        // Data
        [HideInInspector] public bool tagForDeletion = false;
        [HideInInspector] public int positionOffset = 0;
        [SerializeField] public string saveName = "New Preset";
        [SerializeField] public string pathNameFilter = string.Empty;
        [SerializeField] public string fileNameFilter = string.Empty;



        // New Data
        [SerializeField] public bool isEnabled = true;
        [SerializeField] public Preset targetPreset;
        [SerializeField] public List<string> pathFilters = new List<string>();
        [SerializeField] public List<string> filenameFilters = new List<string>();

        // New GUI
        private bool showPathFilter;
        private Vector2 pathFilterListScroll;
        private ReorderableList m_pathFilterList;
        public ReorderableList PathFilterList
        {
            get
            {
                if (m_pathFilterList == null) SetupReorderableList(ref m_pathFilterList, pathFilters);
                return m_pathFilterList;
            }
        }

        private bool showFilenameFilter;
        private Vector2 pathFilenameListScroll;
        private ReorderableList m_filenameFilterList;
        public ReorderableList FilenameFilterList
        {
            get
            {
                if (m_filenameFilterList == null) SetupReorderableList(ref m_filenameFilterList, filenameFilters);
                return m_filenameFilterList;
            }
        }

        public virtual void DrawInnerGUI(Rect areaRect)
        {
            Rect titleArea = new Rect(areaRect) { height = 24 };

            using (new GUILayout.AreaScope(titleArea, string.Empty, EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Preset Settings: ", EditorStyles.boldLabel, GUILayout.Width(110));
                    saveName = EditorGUILayout.TextField(saveName);
                }
            }

            Rect filtersRect = new Rect(titleArea) { y = titleArea.yMax + 4 };
            filtersRect = DrawFilterSection(filtersRect);
            filtersRect.y = filtersRect.yMax + 4;


        }

        private Rect DrawFilterSection(Rect filterArea)
        {
            float pathFilterAreaHeight = 24;

            if (showPathFilter)
            {
                pathFilterAreaHeight = (filenameFilters.Count == 0 ? 58 : 42) + PathFilterList.elementHeight * filenameFilters.Count;
                pathFilterAreaHeight = Mathf.Min(pathFilterAreaHeight, 175);
            }

            Rect pathFilterArea = new Rect(filterArea) { height = pathFilterAreaHeight };

            using (new GUILayout.AreaScope(pathFilterArea, string.Empty, EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    showPathFilter = EditorGUILayout.Foldout(showPathFilter, "Path Filter", true);

                    if (showPathFilter)
                    {
                        if (GUILayout.Button(ImportPresetsResources.MinusIcon, EditorStyles.label, GUILayout.Width(16)))
                        {
                            if (PathFilterList.index >= 0 && (filenameFilters.Count - 1) >= PathFilterList.index)
                            {
                                Undo.RecordObject(ImportPresetsResources.ImportSettingsData, "Removed Path Filter");
                                filenameFilters.RemoveAt(PathFilterList.index);
                                PathFilterList.index = Mathf.Max(0, PathFilterList.index - 1);
                                PathFilterList.GrabKeyboardFocus();
                            }
                        }

                        if (GUILayout.Button(ImportPresetsResources.PlusIcon, EditorStyles.label, GUILayout.Width(16)))
                        {
                            Undo.RecordObject(ImportPresetsResources.ImportSettingsData, "Added Path Filter");
                            filenameFilters.Add(string.Empty);
                            PathFilterList.index = filenameFilters.Count - 1;
                            PathFilterList.GrabKeyboardFocus();
                            pathFilterListScroll.y = float.MaxValue;
                        }
                    }
                }

                if (showPathFilter)
                {
                    GUILayout.Space(2);
                    EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), ImportPresetsResources.HeaderSeparatorColor);
                    GUILayout.Space(2);

                    using (var scroll = new EditorGUILayout.ScrollViewScope(pathFilterListScroll, GUILayout.MaxHeight(PathFilterList.GetHeight() + 4)))
                    {
                        pathFilterListScroll = scroll.scrollPosition;
                        PathFilterList.DoLayoutList();
                    }
                }
            }

            return pathFilterArea;
        }

        private void SetupReorderableList(ref ReorderableList UIList, List<string> filterList)
        {
            if (UIList == null)
            {
                UIList = new ReorderableList(filterList, typeof(string), true, false, false, false);
                UIList.showDefaultBackground = false;
                UIList.headerHeight = 0;
                UIList.footerHeight = 0;
                UIList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    Rect textRect = new Rect(rect) { y = rect.y + 1, yMax = rect.yMax - 4 };
                    filterList[index] = GUI.TextField(textRect, filterList[index]);
                };
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
