using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace bTools.ImportPresets
{
    public class ImportPresetsPreferences
    {
        private static readonly GUIContent verboseApplyContent = new GUIContent("Verbose Apply", "Log a message in the console when an Import Preset is successfully applied");

        [SettingsProvider]
        protected static SettingsProvider GetSettingsProvider()
        {
            var provider = new SettingsProvider("Preferences/bTools/Import Presets", SettingsScope.User)
            {
                label = "Import Presets",
                guiHandler = (searchContext) =>
                {
                    bool verboseApply = EditorPrefs.GetBool(ImportPresetsResources.VerboseApplyKey, false);
                    EditorGUI.BeginChangeCheck();
                    verboseApply = EditorGUILayout.Toggle(verboseApplyContent, verboseApply);
                    if (EditorGUI.EndChangeCheck()) EditorPrefs.SetBool(ImportPresetsResources.VerboseApplyKey, verboseApply);
                },

                keywords = new HashSet<string>(new[] { "bTools", "Import", "Presets" })
            };

            return provider;
        }
    }
}