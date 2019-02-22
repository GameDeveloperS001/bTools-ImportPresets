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
        #endregion

        [MenuItem("bTools/ImportPresets")]
        static void Init()
        {
            var window = GetWindow<ImportSettingsWindow>(string.Empty, true);

            window.titleContent = new GUIContent("Import", "Asset Import Presets Configuration");
            window.minSize = new Vector2(500, 400);
            Undo.undoRedoPerformed += window.OnUndoRedo;
        }

        private void OnUndoRedo()
        {
            Repaint();
            // On undo/redo, reset selection to last item in the list
            if (presetOrderableList != null) presetOrderableList.index = presetOrderableList.count - 1;
        }

        private void OnGUI()
        {
            Rect windowRect = new Rect(position) { x = 0, y = 0 };
            Rect insetRect = windowRect.WithPadding(3);

            #region LeftPane - Preset list
            Rect leftPane = new Rect(insetRect);
            leftPane.width = Mathf.Ceil(insetRect.width * (1.0f / 4.0f));
            leftPane.width = Mathf.Min(Mathf.Max(leftPane.width, 175), 300);

            using (new GUILayout.AreaScope(leftPane, string.Empty, EditorStyles.helpBox))
            {
                DoReorderableList(ImportPresetsResources.ImportSettingsData.importConfigs);
            }

            #endregion

            #region Right Pane - Settings Data
            Rect rightPane = new Rect(leftPane) { x = leftPane.xMax + 4, xMax = windowRect.xMax - 2 };

            if (0 > presetOrderableList.index) presetOrderableList.index = 0;

            if (presetOrderableList.index >= ImportPresetsResources.ImportSettingsData.importConfigs.Count)
                presetOrderableList.index = 0;
            if (ImportPresetsResources.ImportSettingsData.importConfigs.Count > 0)
                ImportPresetsResources.ImportSettingsData.importConfigs[presetOrderableList.index].DrawGUI(rightPane);
            else
            {
                Rect noPresetInfoRect = new Rect(rightPane) { y = rightPane.height / 2, height = 16, x = rightPane.x + 20 };
                GUI.Label(noPresetInfoRect, "No Import Presets - Start by adding some !");
                GUI.Box(rightPane, string.Empty, EditorStyles.helpBox);
            }
            #endregion
        }

        private void DoReorderableList(List<ImportConfig> list)
        {
            if (presetOrderableList == null)
            {
                presetOrderableList = new ReorderableList(list, typeof(ImportConfig), true, false, false, false);
                presetOrderableList.headerHeight = 0;
                presetOrderableList.showDefaultBackground = false;
                presetOrderableList.drawNoneElementCallback = (Rect rect) =>
                {
                    GUI.Label(rect, "No Presets");
                };
                presetOrderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var elem = (ImportConfig)list[index];
                    GUI.Label(rect, elem.saveName);
                    rect.x = rect.xMax - 15;
                    elem.isEnabled = GUI.Toggle(rect, elem.isEnabled, string.Empty);
                };
            }

            using (new GUILayout.HorizontalScope())
            {
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

                GUILayout.Space(4);

                if (GUILayout.Button(ImportPresetsResources.PlusIcon, GUIStyle.none, GUILayout.Width(16)))
                {
                    Undo.RecordObject(ImportPresetsResources.ImportSettingsData, "Added Import Preset");
                    list.Add(new ImportConfig());
                    presetOrderableList.index = list.Count - 1;
                    orderableListScroll.y = float.MaxValue;
                    presetOrderableList.GrabKeyboardFocus();
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("All On", EditorStyles.miniButtonLeft))
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        ((ImportConfig)list[i]).isEnabled = false;
                    }
                }

                if (GUILayout.Button("Flip", EditorStyles.miniButtonMid))
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        var item = ((ImportConfig)list[i]);
                        item.isEnabled = !item.isEnabled;
                    }
                }

                if (GUILayout.Button("All Off", EditorStyles.miniButtonRight))
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        ((ImportConfig)list[i]).isEnabled = true;
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
