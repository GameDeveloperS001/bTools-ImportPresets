using System;
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using bTools.CodeExtensions;

namespace bTools.ImportPresets
{
    public class ImportSettingsWindow : EditorWindow
    {
        // GUI
        public static readonly Color HeaderSeparatorColor = new Color32(237, 166, 3, 255);
        private int selectedTab = 0;
        private string[] tabsText = new string[] { "Model", "Texture", "Audio" };
        private ReorderableList presetOrderableList;
        private Vector2 orderableListScroll;
        private Vector2 settingsScroll;

        [MenuItem("bTools/ImportPresets")]
        static void Init()
        {
            var window = GetWindow<ImportSettingsWindow>(string.Empty, true);

            window.titleContent = new GUIContent("Import", "Asset Import Presets Configuration");
            window.minSize = new Vector2(800, 600);
            Undo.undoRedoPerformed += () => window.Repaint();
        }

        private void OnGUI()
        {
            minSize = new Vector2(800, 600);
            Rect windowRect = new Rect(position)
            {
                x = 0,
                y = 0
            };
            Rect insetRect = windowRect.WithPadding(3);
            //EditorGUI.DrawRect(windowRect, Colors.ChineseBlack);

            EditorGUI.BeginChangeCheck();

            #region TopLeftPane - Reordeing & Rules
            Rect topLeftPane = new Rect(insetRect);
            topLeftPane.width = Mathf.Ceil(insetRect.width * 0.333333f);
            //EditorGUI.DrawRect( topLeftPane, Colors.DarkCharcoal );

            GUILayout.BeginArea(topLeftPane, EditorStyles.helpBox);

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUI.BeginChangeCheck();
            selectedTab = GUILayout.Toolbar(selectedTab, tabsText, EditorStyles.toolbarButton);
            if (EditorGUI.EndChangeCheck())
            {
                presetOrderableList = null;
            }
            GUILayout.EndHorizontal();

            switch (selectedTab)
            {
                default:
                case 0:
                    DoReorderableList(ImportProcessing.ImportSettingsData.meshImportSettingsList, typeof(MeshImportConfig));
                    break;
                case 1:
                    DoReorderableList(ImportProcessing.ImportSettingsData.textureImportSettingsList, typeof(TextureImportConfig));
                    break;
                case 2:
                    DoReorderableList(ImportProcessing.ImportSettingsData.audioImportSettingsList, typeof(AudioImportConfig));
                    break;
            }

            GUILayout.EndArea();
            #endregion

            #region Right Pane - Settings Data
            Rect rightPane = new Rect(insetRect);
            //rightPane.width = Mathf.Ceil((insetRect.width * 0.666666f)) - 3;
            rightPane.x = topLeftPane.xMax;
            rightPane.xMax = windowRect.xMax;
            //EditorGUI.DrawRect(rightPane, Colors.DarkCharcoal);

            Rect rightPaneGUI = rightPane;
            rightPaneGUI.x += 4;
            rightPaneGUI.width -= 8;
            GUILayout.BeginArea(rightPaneGUI, EditorStyles.helpBox);

            if (presetOrderableList.index < 0)
            {
                presetOrderableList.index = 0;
            }

            using (var scroll = new EditorGUILayout.ScrollViewScope(settingsScroll))
            {
                settingsScroll = scroll.scrollPosition;

                switch (selectedTab)
                {
                    default:
                    case 0:
                        if (presetOrderableList.index >= ImportProcessing.ImportSettingsData.meshImportSettingsList.Count)
                        {
                            presetOrderableList.index = 0;
                        }
                        if (ImportProcessing.ImportSettingsData.meshImportSettingsList.Count > 0)
                        {
                            ImportProcessing.ImportSettingsData.meshImportSettingsList[presetOrderableList.index].DrawInnerGUI();
                        }
                        break;
                    case 1:
                        if (presetOrderableList.index >= ImportProcessing.ImportSettingsData.textureImportSettingsList.Count)
                        {
                            presetOrderableList.index = 0;
                        }
                        if (ImportProcessing.ImportSettingsData.textureImportSettingsList.Count > 0)
                        {
                            ImportProcessing.ImportSettingsData.textureImportSettingsList[presetOrderableList.index].DrawInnerGUI();
                        }
                        break;
                    case 2:
                        if (presetOrderableList.index >= ImportProcessing.ImportSettingsData.audioImportSettingsList.Count)
                        {
                            presetOrderableList.index = 0;
                        }
                        if (ImportProcessing.ImportSettingsData.audioImportSettingsList.Count > 0)
                        {
                            ImportProcessing.ImportSettingsData.audioImportSettingsList[presetOrderableList.index].DrawInnerGUI();
                        }
                        break;
                }
            }

            GUILayout.EndArea();
            #endregion

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(ImportProcessing.ImportSettingsData);
            }
        }

        private void DoReorderableList(IList list, Type listType)
        {
            if (presetOrderableList == null)
            {
                presetOrderableList = new ReorderableList(list, listType, true, false, false, false);
                presetOrderableList.headerHeight = 0;
                presetOrderableList.showDefaultBackground = false;
                presetOrderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var elem = (ImportConfigBase)list[index];
                    GUI.Label(rect, elem.saveName);
                    rect.x = rect.xMax - 15;
                    elem.isEnabled = GUI.Toggle(rect, elem.isEnabled, string.Empty);
                };
            }
            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            GUILayout.Space(4);
            if (GUILayout.Button(EditorGUIUtility.FindTexture("d_Toolbar Plus"), GUIStyle.none, GUILayout.Width(16)))
            {
                list.Add(Activator.CreateInstance(listType));
                presetOrderableList.index = list.Count - 1;
                presetOrderableList.GrabKeyboardFocus();
                settingsScroll.y = float.MaxValue;
            }
            GUILayout.Space(4);
            if (GUILayout.Button(EditorGUIUtility.FindTexture("d_Toolbar Minus"), GUIStyle.none, GUILayout.Width(16)))
            {
                if (presetOrderableList.index >= 0 && presetOrderableList.index <= list.Count - 1)
                {
                    Undo.RecordObject(ImportProcessing.ImportSettingsData, "Removed Import Preset");
                    list.RemoveAt(presetOrderableList.index);
                    presetOrderableList.index = Mathf.Max(0, presetOrderableList.index - 1);
                    presetOrderableList.GrabKeyboardFocus();
                }
            }
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Disbl.All", EditorStyles.miniButtonLeft))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    ((ImportConfigBase)list[i]).isEnabled = false;
                }
            }
            if (GUILayout.Button("Enabl.All", EditorStyles.miniButtonRight))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    ((ImportConfigBase)list[i]).isEnabled = true;
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), HeaderSeparatorColor);

            using (var scroll = new EditorGUILayout.ScrollViewScope(orderableListScroll))
            {
                orderableListScroll = scroll.scrollPosition;
                presetOrderableList.DoLayoutList();
            }
        }
    }
}