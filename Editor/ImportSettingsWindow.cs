using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace bTools.ImportPresets
{
    public class ImportSettingsWindow : EditorWindow
    {
        private ReorderableList presetOrderableList;
        private Vector2 orderableListScroll;

        [MenuItem("bTools/Import Presets")]
        private static void Init()
        {
            var window = GetWindow<ImportSettingsWindow>(string.Empty, true);

            window.titleContent = new GUIContent("Import", "Asset Import Presets Configuration");
            window.minSize = new Vector2(500, 400);

            Undo.undoRedoPerformed += () => window.Repaint();
        }

        private void OnGUI()
        {
            Rect windowRect = new Rect(position)
            { x = 0, y = 0 };
            Rect insetRect = new Rect(windowRect)
            { x = windowRect.x + 3, xMax = windowRect.xMax - 3, y = windowRect.y + 3, yMax = windowRect.yMax - 3 };

            #region LeftPane - Preset list
            Rect leftPane = new Rect(insetRect);
            leftPane.width = Mathf.Ceil(insetRect.width * (1.0f / 4.0f));
            leftPane.width = Mathf.Min(Mathf.Max(leftPane.width, 175), 300);

            using (new GUILayout.AreaScope(leftPane, string.Empty, EditorStyles.helpBox))
            {
                DrawPresetsList(ImportPresetsResources.DataAsset.importConfigs);
            }
            #endregion

            #region Right Pane - Settings Data
            Rect rightPane = new Rect(leftPane)
            { x = leftPane.xMax + 4, xMax = windowRect.xMax - 2 };

            if (0 > presetOrderableList.index) presetOrderableList.index = 0;

            if (presetOrderableList.index >= ImportPresetsResources.DataAsset.importConfigs.Count)
                presetOrderableList.index = 0;
            if (ImportPresetsResources.DataAsset.importConfigs.Count > 0)
                ImportPresetsResources.DataAsset.importConfigs[presetOrderableList.index].DrawGUI(rightPane, presetOrderableList.index);
            else
            {
                Rect noPresetInfoRect = new Rect(rightPane) { y = rightPane.height / 2, height = 16, x = rightPane.x + 20 };
                GUI.Label(noPresetInfoRect, "No Import Presets - Start by adding some !");
                GUI.Box(rightPane, string.Empty, EditorStyles.helpBox);
            }
            #endregion
        }

        private void DrawPresetsList(List<ImportConfig> importConfigList)
        {
            if (presetOrderableList == null)
            { // Init preset Reorderable list.
                presetOrderableList = new ReorderableList(importConfigList, typeof(ImportConfig), true, false, false, false);
                presetOrderableList.headerHeight = 0;
                presetOrderableList.elementHeight = 26;
                presetOrderableList.showDefaultBackground = false;
                presetOrderableList.drawNoneElementCallback = (Rect rect) =>
                {
                    GUI.Label(rect, "No Presets");
                };
                presetOrderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    Rect topRect = new Rect(rect);
                    var elem = importConfigList[index];
                    GUI.Label(topRect, elem.saveName);
                    topRect.x = topRect.xMax - 15;
                    elem.isEnabled = GUI.Toggle(topRect, elem.isEnabled, string.Empty);

                    if (importConfigList[index].targetPreset != null)
                    {
                        Rect bottomRect = new Rect(rect) { y = rect.y + 10, height = 16 };
                        GUIStyle style = new GUIStyle(ImportPresetsResources.MiniLabelStyle);
                        if (!isActive) style.normal.textColor = Color.gray;
                        GUI.Label(bottomRect, importConfigList[index].targetPreset.GetTargetTypeName().Replace("Importer", string.Empty), style);
                    }
                };
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(4);

                if (GUILayout.Button(ImportPresetsResources.MinusIcon, GUIStyle.none, GUILayout.Width(16)))
                {
                    if (presetOrderableList.index >= 0 && (importConfigList.Count - 1) >= presetOrderableList.index)
                    {
                        Undo.RecordObject(ImportPresetsResources.DataAsset, "Removed Import Preset");
                        importConfigList.RemoveAt(presetOrderableList.index);
                        presetOrderableList.index = Mathf.Max(0, presetOrderableList.index - 1);
                        presetOrderableList.GrabKeyboardFocus();
                    }
                }

                GUILayout.Space(4);

                if (GUILayout.Button(ImportPresetsResources.PlusIcon, GUIStyle.none, GUILayout.Width(16)))
                {
                    Undo.RecordObject(ImportPresetsResources.DataAsset, "Added Import Preset");
                    importConfigList.Add(new ImportConfig());
                    presetOrderableList.index = importConfigList.Count - 1;
                    orderableListScroll.y = float.MaxValue;
                    presetOrderableList.GrabKeyboardFocus();
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("All On", EditorStyles.miniButtonLeft))
                {
                    for (int i = 0; i < importConfigList.Count; i++)
                    {
                        ((ImportConfig)importConfigList[i]).isEnabled = false;
                    }
                }

                if (GUILayout.Button("Flip", EditorStyles.miniButtonMid))
                {
                    for (int i = 0; i < importConfigList.Count; i++)
                    {
                        var item = ((ImportConfig)importConfigList[i]);
                        item.isEnabled = !item.isEnabled;
                    }
                }

                if (GUILayout.Button("All Off", EditorStyles.miniButtonRight))
                {
                    for (int i = 0; i < importConfigList.Count; i++)
                    {
                        ((ImportConfig)importConfigList[i]).isEnabled = true;
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