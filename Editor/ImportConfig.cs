using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Presets;
using UnityEditorInternal;

namespace bTools.ImportPresets
{
    [System.Serializable]
    public class ImportConfig
    {
        #region Data
        [SerializeField] public string saveName = "New Preset";
        [SerializeField] public bool isEnabled = true;
        [SerializeField] public Preset targetPreset = null;
        [SerializeField] public List<string> pathFilters = new List<string>();
        [SerializeField] public List<string> filenameFilters = new List<string>();
        #endregion

        #region GUI
        private SerializedObject m_serializedObject;
        private SerializedObject _SerializedObject
        {
            get
            {
                if (m_serializedObject == null) m_serializedObject = new SerializedObject(ImportPresetsResources.DataAsset);
                return m_serializedObject;
            }
        }

        private bool showPathFilter;
        private Vector2 pathFilterListScroll;
        private ReorderableList pathFilterList;

        private bool showFilenameFilter;
        private Vector2 pathFilenameListScroll;
        private ReorderableList filenameFilterList;

        private bool showPreset;
        private Vector2 presetScroll;
        private Editor cachedPresetEditor;
        #endregion

        internal void DrawGUI(Rect areaRect, int index)
        {
            _SerializedObject.Update();
            SerializedProperty arrayProperty = _SerializedObject.FindProperty("importConfigs").GetArrayElementAtIndex(index);

            // Draw title section.
            Rect titleArea = new Rect(areaRect) { height = 22 };
            using (new GUILayout.AreaScope(titleArea, string.Empty, EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Import Preset: ", EditorStyles.boldLabel, GUILayout.Width(100));
                    EditorGUILayout.PropertyField(arrayProperty.FindPropertyRelative("saveName"), GUIContent.none);
                    if (GUILayout.Button("Preview", EditorStyles.miniButton))
                    {
                        Rect popupRect = new Rect(areaRect) { x = areaRect.width - 32, y = 0, height = 4 };
                        PopupWindow.Show(popupRect, new FilterPreviewPopup(areaRect.height, GetMatchingPaths()));
                    }
                }
            }

            // Draw filter sections.
            Rect sectionRect = new Rect(titleArea) { y = titleArea.yMax + 4 };
            sectionRect = DrawFilterListSection(sectionRect, ref pathFilterList, pathFilters, ref showPathFilter, ref pathFilterListScroll, "Path Filters", arrayProperty.FindPropertyRelative("pathFilters"));
            sectionRect.y = sectionRect.yMax + 4;
            sectionRect = DrawFilterListSection(sectionRect, ref filenameFilterList, filenameFilters, ref showFilenameFilter, ref pathFilenameListScroll, "Filename Filters", arrayProperty.FindPropertyRelative("filenameFilters"));
            sectionRect.y = sectionRect.yMax + 4;

            // Draw preset section.
            if (showPreset) sectionRect.yMax = areaRect.yMax;
            else sectionRect.height = 22;

            using (new GUILayout.AreaScope(sectionRect, string.Empty, EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    showPreset = EditorGUILayout.Foldout(showPreset, "Preset: ", true);
                    EditorGUILayout.PropertyField(arrayProperty.FindPropertyRelative("targetPreset"), GUIContent.none);
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
                        EditorGUILayout.LabelField("No preset - Add one !");
                    }
                }
            }

            _SerializedObject.ApplyModifiedProperties();
        }

        private Rect DrawFilterListSection(Rect filterRect, ref ReorderableList filterList, List<string> filterData, ref bool showFilter, ref Vector2 scrollBar, string headerTitle, SerializedProperty listSerializedProp)
        {
            if (filterList == null)
            {
                filterList = new ReorderableList(filterData, typeof(string), true, false, false, false);
                filterList.showDefaultBackground = false;
                filterList.headerHeight = 0;
                filterList.footerHeight = 0;
                filterList.drawNoneElementCallback = (Rect rect) =>
                {
                    GUI.Label(rect, "No Filter - Everything will be accepted");
                };
                filterList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    Rect textRect = new Rect(rect) { y = rect.y + 1, yMax = rect.yMax - 4 };

                    listSerializedProp.serializedObject.Update();
                    SerializedProperty filterProp = listSerializedProp.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(textRect, filterProp, GUIContent.none);
                    filterProp.stringValue = filterProp.stringValue.Replace('/', '\\');
                    listSerializedProp.serializedObject.ApplyModifiedProperties();
                };
            }

            float pathFilterAreaHeight = 22;
            if (showFilter)
            {
                pathFilterAreaHeight = (filterData.Count == 0 ? 50 : 38) + filterList.elementHeight * filterData.Count;
                pathFilterAreaHeight = Mathf.Min(pathFilterAreaHeight, 175);
            }

            Rect pathFilterArea = new Rect(filterRect) { height = pathFilterAreaHeight };

            using (new GUILayout.AreaScope(pathFilterArea, string.Empty, EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    showFilter = EditorGUILayout.Foldout(showFilter, headerTitle, true);

                    if (showFilter)
                    {
                        if (GUILayout.Button(ImportPresetsResources.MinusIcon, EditorStyles.label, GUILayout.Width(16)))
                        {
                            if (filterList.index >= 0 && (filterData.Count - 1) >= filterList.index)
                            {
                                Undo.RecordObject(ImportPresetsResources.DataAsset, "Removed Import Filter");
                                filterData.RemoveAt(filterList.index);
                                filterList.index = Mathf.Max(0, filterList.index - 1);
                                filterList.GrabKeyboardFocus();
                            }
                        }

                        if (GUILayout.Button(ImportPresetsResources.PlusIcon, EditorStyles.label, GUILayout.Width(16)))
                        {
                            Undo.RecordObject(ImportPresetsResources.DataAsset, "Added Import Filter");
                            filterData.Add(string.Empty);
                            filterList.index = filterData.Count - 1;
                            filterList.GrabKeyboardFocus();
                            scrollBar.y = float.MaxValue;
                        }
                    }
                }

                if (showFilter)
                {
                    GUILayout.Space(-2);
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

        private (string, int, int) GetMatchingPaths()
        {
            if (pathFilters.Count == 0)
            {
                string info = "Empty path filters means this will be applied to every folder";
                return (info, 1, info.Length);
            }

            StringBuilder matchingPaths = new StringBuilder();
            int matchCount = 0;
            int longestString = 0;
            foreach (var item in Directory.GetDirectories(Application.dataPath, "*", SearchOption.AllDirectories))
            {
                if (PathFilterTest(item))
                {
                    matchCount++;
                    string success = "Assets" + item.Replace(Application.dataPath, string.Empty) + "\r\n";
                    if (success.Length > longestString) longestString = success.Length;
                    matchingPaths.Append(success);
                }
            }

            if (matchingPaths.Length == 0)
            {
                matchCount++;
                longestString = 10;
                matchingPaths.Append("No match");
            }

            return (matchingPaths.ToString(), matchCount, longestString);
        }

        internal bool FilenameFilterTest(string path)
        {
            string fileName = Path.GetFileName(path);

            // Check if filter is empty.
            if (filenameFilters == null || filenameFilters.Count == 0)
            {
                return true;
            }

            // Check if filename contains what we want.
            bool success = false;
            for (int i = 0; i < filenameFilters.Count; i++)
            {
                if (string.IsNullOrEmpty(filenameFilters[i])) continue; // Ignore empty filters.
                if (fileName.Contains(filenameFilters[i]))
                {
                    success = true;
                    break;
                }
            }

            return success;
        }

        internal bool PathFilterTest(string path)
        {
            path = path.Replace('/', '\\');

            // Check if filter is empty.
            if (pathFilters == null || pathFilters.Count == 0)
            {
                return true;
            }

            // Check if path contains what we want.
            bool success = false;
            for (int i = 0; i < pathFilters.Count; i++)
            {
                if (string.IsNullOrEmpty(pathFilters[i])) continue; // Ignore empty filters.
                if (path.Contains(pathFilters[i]))
                {
                    success = true;
                    break;
                }
            }

            return success;
        }
    }
}