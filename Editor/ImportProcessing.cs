using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace bTools.ImportPresets
{
    public class ImportProcessing : AssetPostprocessor
    {
        private void OnPreprocessAsset()
        {
            // Make sure Editor is not compiling.
            if (EditorApplication.isCompiling) return;

            // Make sure there is at least one settings file.
            List<ImportConfig> presetsList = ImportPresetsResources.DataAsset.importConfigs;
            if (0 >= presetsList.Count) return;

            // Check config conditions.
            for (int i = 0; i < presetsList.Count; i++)
            {
                if (!presetsList[i].isEnabled) continue; // Skip if preset is disabled.
                if (presetsList[i].targetPreset == null) continue; // Skip if preset is empty.
                if (!presetsList[i].targetPreset.CanBeAppliedTo(assetImporter)) continue; // Skip if preset cannot be applied to this.

                bool filenameTest = presetsList[i].FilenameFilterTest(assetPath);
                if (!filenameTest) continue; // Check filename first, less expensive.

                bool pathTest = presetsList[i].PathFilterTest(assetPath);
                if (!pathTest) continue;

                // Check that the asset isn't already imported (Apply triggers this script).
                if (AssetDatabase.LoadAssetAtPath<Object>(assetPath) == null)
                {
                    presetsList[i].targetPreset.ApplyTo(assetImporter);
                    break;
                }
            }
        }
    }
}