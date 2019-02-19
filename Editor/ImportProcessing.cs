using UnityEngine;
using UnityEditor;
using bTools.CodeExtensions;
using System.IO;
using UnityEditor.Presets;
using System.Collections.Generic;

namespace bTools.ImportPresets
{
    public class ImportProcessing : AssetPostprocessor
    {
        private char[] filterSep = new char[] { ';' };

        internal static string k_settingsAssetPath
        {
            get
            {
                string path = Directory.GetDirectories(Application.dataPath, "bTools", SearchOption.AllDirectories)[0];
                path = path.Replace(Application.dataPath, "Assets");
                path = path.Replace('\\', '/');
                path = path + @"/ImportPresets/Settings/bToolsImportPresets.asset";

                return path;
            }
        }

        private static SavedImportSettings m_importSettingsData;
        public static SavedImportSettings ImportSettingsData
        {
            get
            {
                if (m_importSettingsData == null)
                {
                    if (!Directory.Exists(Path.GetDirectoryName(k_settingsAssetPath))) Directory.CreateDirectory(Path.GetDirectoryName(k_settingsAssetPath));
                    var settings = AssetDatabase.LoadAssetAtPath<SavedImportSettings>(k_settingsAssetPath);
                    if (m_importSettingsData == null)
                    {
                        m_importSettingsData = ScriptableObject.CreateInstance<SavedImportSettings>();
                        AssetDatabase.CreateAsset(m_importSettingsData, k_settingsAssetPath);
                        AssetDatabase.SaveAssets();
                    }
                }

                return m_importSettingsData;
            }
        }


        private void OnPreprocessModel()
        {
            ApplyImportPresets(ImportSettingsData.meshImportSettingsList);
        }

        private void OnPreprocessAudio()
        {
            ApplyImportPresets(ImportSettingsData.audioImportSettingsList);
        }

        private void OnPreprocessTexture()
        {
            ApplyImportPresets(ImportSettingsData.textureImportSettingsList);
        }

        private void ApplyImportPresets(List<ImportConfigBase> presets)
        {
            // Make sure Editor is not compiling
            if (EditorApplication.isCompiling) return;

            //Make sure there are at least one settings file
            if (presets.Count <= 0) return;

            //Check config conditions
            for (int i = 0; i < presets.Count; i++)
            {
                bool filenameTest = presets[i].FilenameFilterTest(assetPath);
                bool pathTest = presets[i].PathFilterTest(assetPath);
                if (filenameTest && pathTest)
                {
                    //Check that the asset doesn't already exist (Apply triggers this script)
                    if (AssetDatabase.LoadAssetAtPath<Object>(assetPath) == null)
                    {
                        presets[i].targetPreset.ApplyTo(assetImporter);
                        break;
                    }
                }
            }
        }
    }
}