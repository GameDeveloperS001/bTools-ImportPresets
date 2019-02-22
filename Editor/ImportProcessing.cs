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

        private void OnPreprocessModel()
        {
            //ApplyImportPresets(ImportSettingsData.meshImportSettingsList);

        }

        private void OnPreprocessAudio()
        {
            //ApplyImportPresets(ImportSettingsData.audioImportSettingsList);
        }

        private void OnPreprocessTexture()
        {
            //ApplyImportPresets(ImportSettingsData.textureImportSettingsList);
        }

        private void ApplyImportPresets(List<ImportConfig> presets)
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