using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using bTools.CodeExtensions;
using System.Collections.Generic;

namespace bTools.ImportPresets
{
    public class ImportSettingsWindow : EditorWindow
    {
        #region GUI Variables
        private readonly string[] tabsText = new string[] { "Model", "Texture", "Audio" };
        private int selectedTab = 0;
        private ReorderableList presetOrderableList;
        private Vector2 orderableListScroll;
        private Vector2 settingsScroll;
        #endregion

        [MenuItem("bTools/ImportPresets")]
        static void Init()
        {
            var window = GetWindow<ImportSettingsWindow>(string.Empty, true);

            window.titleContent = new GUIContent("Import", "Asset Import Presets Configuration");
            window.minSize = new Vector2(500, 400);
            Undo.undoRedoPerformed += window.OnUndoRedo;
        }

        private void OnGUI()
        {
            Rect windowRect = new Rect(position) { x = 0, y = 0 };
            Rect insetRect = windowRect.WithPadding(3);

            #region LeftPane - Tabs and Preset list
            Rect leftPane = new Rect(insetRect);
            leftPane.width = Mathf.Ceil(insetRect.width * (1.0f / 4.0f));
            leftPane.width = Mathf.Min(Mathf.Max(leftPane.width, 175), 300);

            using (new GUILayout.AreaScope(leftPane, string.Empty, EditorStyles.helpBox))
            {
                using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                {
                    EditorGUI.BeginChangeCheck();
                    selectedTab = GUILayout.Toolbar(selectedTab, tabsText, EditorStyles.toolbarButton);
                    if (EditorGUI.EndChangeCheck()) presetOrderableList = null;
                }

                switch (selectedTab)
                {
                    default:
                    case 0:
                        DoReorderableList(ImportPresetsResources.ImportSettingsData.meshImportSettingsList, typeof(MeshImportConfig));
                        break;
                    case 1:
                        DoReorderableList(ImportPresetsResources.ImportSettingsData.textureImportSettingsList, typeof(TextureImportConfig));
                        break;
                    case 2:
                        DoReorderableList(ImportPresetsResources.ImportSettingsData.audioImportSettingsList, typeof(AudioImportConfig));
                        break;
                }
            }

            #endregion

            #region Right Pane - Settings Data
            Rect rightPane = new Rect(insetRect) { x = leftPane.xMax + 4, xMax = windowRect.xMax - 4 };

            using (new GUILayout.AreaScope(rightPane, string.Empty, EditorStyles.helpBox))
            {
                if (0 > presetOrderableList.index) presetOrderableList.index = 0;

                using (var scroll = new EditorGUILayout.ScrollViewScope(settingsScroll))
                {
                    settingsScroll = scroll.scrollPosition;

                    switch (selectedTab)
                    {
                        default:
                        case 0: // Mesh
                            if (presetOrderableList.index >= ImportPresetsResources.ImportSettingsData.meshImportSettingsList.Count)
                                presetOrderableList.index = 0;
                            if (ImportPresetsResources.ImportSettingsData.meshImportSettingsList.Count > 0)
                                ImportPresetsResources.ImportSettingsData.meshImportSettingsList[presetOrderableList.index].DrawInnerGUI();
                            break;
                        case 1: // Texture
                            if (presetOrderableList.index >= ImportPresetsResources.ImportSettingsData.textureImportSettingsList.Count)
                                presetOrderableList.index = 0;
                            if (ImportPresetsResources.ImportSettingsData.textureImportSettingsList.Count > 0)
                                ImportPresetsResources.ImportSettingsData.textureImportSettingsList[presetOrderableList.index].DrawInnerGUI();
                            break;
                        case 2: // Audio
                            if (presetOrderableList.index >= ImportPresetsResources.ImportSettingsData.audioImportSettingsList.Count)
                                presetOrderableList.index = 0;
                            if (ImportPresetsResources.ImportSettingsData.audioImportSettingsList.Count > 0)
                                ImportPresetsResources.ImportSettingsData.audioImportSettingsList[presetOrderableList.index].DrawInnerGUI();
                            break;
                    }
                }
            }

            #endregion
        }

        private void OnUndoRedo()
        {
            Repaint();
            // On undo/redo, reset selection to last item in the list
            if (presetOrderableList != null) presetOrderableList.index = presetOrderableList.count - 1;
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

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(4);

                if (GUILayout.Button(ImportPresetsResources.PlusIcon, GUIStyle.none, GUILayout.Width(16)))
                {
                    Undo.RecordObject(ImportPresetsResources.ImportSettingsData, "Added Import Preset");
                    list.Add(Activator.CreateInstance(listType));
                    presetOrderableList.index = list.Count - 1;
                    presetOrderableList.GrabKeyboardFocus();
                    settingsScroll.y = float.MaxValue;
                }

                GUILayout.Space(4);

                if (GUILayout.Button(ImportPresetsResources.MinusIcon, GUIStyle.none, GUILayout.Width(16)))
                {
                    if (presetOrderableList.index >= 0 && (list.Count - 1) >= presetOrderableList.index)
                    {
                        Undo.RecordObject(ImportPresetsResources.ImportSettingsData, "Removed Import Preset");
                        list.RemoveAt(presetOrderableList.index);
                        presetOrderableList.index = Mathf.Max(0, presetOrderableList.index - 1);
                        presetOrderableList.GrabKeyboardFocus();
                    }
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("All On", EditorStyles.miniButtonLeft))
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        ((ImportConfigBase)list[i]).isEnabled = false;
                    }
                }

                if (GUILayout.Button("Flip", EditorStyles.miniButtonMid))
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        var item = ((ImportConfigBase)list[i]);
                        item.isEnabled = !item.isEnabled;
                    }
                }

                if (GUILayout.Button("All Off", EditorStyles.miniButtonRight))
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        ((ImportConfigBase)list[i]).isEnabled = true;
                    }
                }
            }

            GUILayout.Space(4);

            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), ImportPresetsResources.HeaderSeparatorColor);

            using (var scroll = new EditorGUILayout.ScrollViewScope(orderableListScroll))
            {
                orderableListScroll = scroll.scrollPosition;
                presetOrderableList.DoLayoutList();
            }
        }
    }
}
