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
        //protected bool folded = false;
        private char[] filterSep = new char[] { ';' };
        private string matchingPaths;


        // Data
        [HideInInspector] public bool tagForDeletion = false;
        [HideInInspector] public int positionOffset = 0;
        [SerializeField] public string pathNameFilter = string.Empty;
        [SerializeField] public string fileNameFilter = string.Empty;



        // New Data
        [SerializeField] public string saveName = "New Preset";
        [SerializeField] public bool isEnabled = true;
        [SerializeField] public Preset targetPreset;
        [SerializeField] public List<string> pathFilters = new List<string>();
        [SerializeField] public List<string> filenameFilters = new List<string>();

        // New GUI
        private bool showPathFilter;
        private Vector2 pathFilterListScroll;
        private ReorderableList pathFilterList;

        private bool showFilenameFilter;
        private Vector2 pathFilenameListScroll;
        private ReorderableList filenameFilterList;

        private bool showPreset;
        private Vector2 presetScroll;
        Editor cachedPresetEditor;

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

            // Draw filter sections
            Rect filtersRect = new Rect(titleArea) { y = titleArea.yMax + 4 };
            filtersRect = DrawFilterSection(filtersRect, ref pathFilterList, pathFilters, ref showPathFilter, ref pathFilterListScroll, "Path Filters");
            filtersRect.y = filtersRect.yMax + 4;
            filtersRect = DrawFilterSection(filtersRect, ref filenameFilterList, filenameFilters, ref showFilenameFilter, ref pathFilenameListScroll, "Filename Filters");
            filtersRect.y = filtersRect.yMax + 4;

            // Draw preset
            if (showPreset) filtersRect.yMax = areaRect.yMax;
            else filtersRect.height = 24;

            using (new GUILayout.AreaScope(filtersRect, string.Empty, EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    showPreset = EditorGUILayout.Foldout(showPreset, "Preset: ", true);
                    targetPreset = (Preset)EditorGUILayout.ObjectField(targetPreset, typeof(Preset), false);
                }

                if (showPreset)
                {
                    GUILayout.Space(2);
                    EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), ImportPresetsResources.HeaderSeparatorColor);
                    GUILayout.Space(2);

                    if (targetPreset != null)
                    {
                        Editor.CreateCachedEditor(targetPreset, null, ref cachedPresetEditor);
                        using (var scroll = new EditorGUILayout.ScrollViewScope(presetScroll))
                        {
                            presetScroll = scroll.scrollPosition;
                            cachedPresetEditor?.OnInspectorGUI();
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("No preset :(");
                    }
                }
            }
        }

        private Rect DrawFilterSection(Rect filterArea, ref ReorderableList filterList, List<string> listData, ref bool showFilter, ref Vector2 scrollBar, string headerTitle)
        {
            if (filterList == null)
            {
                filterList = new ReorderableList(listData, typeof(string), true, false, false, false);
                filterList.showDefaultBackground = false;
                filterList.headerHeight = 0;
                filterList.footerHeight = 0;
                filterList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    Rect textRect = new Rect(rect) { y = rect.y + 1, yMax = rect.yMax - 4 };
                    listData[index] = GUI.TextField(textRect, listData[index]);
                };
            }

            float pathFilterAreaHeight = 24;
            if (showFilter)
            {
                pathFilterAreaHeight = (listData.Count == 0 ? 58 : 42) + filterList.elementHeight * listData.Count;
                pathFilterAreaHeight = Mathf.Min(pathFilterAreaHeight, 175);
            }

            Rect pathFilterArea = new Rect(filterArea) { height = pathFilterAreaHeight };

            using (new GUILayout.AreaScope(pathFilterArea, string.Empty, EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    showFilter = EditorGUILayout.Foldout(showFilter, headerTitle, true);

                    if (showFilter)
                    {
                        if (GUILayout.Button(ImportPresetsResources.MinusIcon, EditorStyles.label, GUILayout.Width(16)))
                        {
                            if (filterList.index >= 0 && (listData.Count - 1) >= filterList.index)
                            {
                                Undo.RecordObject(ImportPresetsResources.ImportSettingsData, "Removed Path Filter");
                                listData.RemoveAt(filterList.index);
                                filterList.index = Mathf.Max(0, filterList.index - 1);
                                filterList.GrabKeyboardFocus();
                            }
                        }

                        if (GUILayout.Button(ImportPresetsResources.PlusIcon, EditorStyles.label, GUILayout.Width(16)))
                        {
                            Undo.RecordObject(ImportPresetsResources.ImportSettingsData, "Added Path Filter");
                            listData.Add(string.Empty);
                            filterList.index = listData.Count - 1;
                            filterList.GrabKeyboardFocus();
                            scrollBar.y = float.MaxValue;
                        }
                    }
                }

                if (showFilter)
                {
                    GUILayout.Space(2);
                    EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), ImportPresetsResources.HeaderSeparatorColor);
                    GUILayout.Space(2);

                    using (var scroll = new EditorGUILayout.ScrollViewScope(scrollBar, GUILayout.MaxHeight(filterList.GetHeight() + 4)))
                    {
                        scrollBar = scroll.scrollPosition;
                        filterList.DoLayoutList();
                    }
                }
            }

            return pathFilterArea;
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