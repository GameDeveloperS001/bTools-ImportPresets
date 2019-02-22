using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace bTools.ImportPresets
{
    public static class ImportPresetsResources
    {
        private static string k_settingsAssetPath;
        internal static string SettingsAssetPath
        {
            get
            {
                if (string.IsNullOrEmpty(k_settingsAssetPath))
                {
                    k_settingsAssetPath = Directory.GetDirectories(Application.dataPath, "bTools", SearchOption.AllDirectories)[0];
                    k_settingsAssetPath = k_settingsAssetPath.Replace(Application.dataPath, "Assets");
                    k_settingsAssetPath = k_settingsAssetPath.Replace('\\', '/');
                    k_settingsAssetPath = k_settingsAssetPath + @"/ImportPresets/Settings/bToolsImportPresets.asset";

                }

                return k_settingsAssetPath;
            }
        }

        private static SavedImportSettings m_importSettingsData;
        public static SavedImportSettings ImportSettingsData
        {
            get
            {
                if (m_importSettingsData == null)
                {
                    m_importSettingsData = AssetDatabase.LoadAssetAtPath<SavedImportSettings>(SettingsAssetPath);

                    if (m_importSettingsData == null)
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(SettingsAssetPath))) Directory.CreateDirectory(Path.GetDirectoryName(SettingsAssetPath));
                        m_importSettingsData = ScriptableObject.CreateInstance<SavedImportSettings>();
                        AssetDatabase.CreateAsset(m_importSettingsData, SettingsAssetPath);
                        AssetDatabase.SaveAssets();
                    }
                }

                return m_importSettingsData;
            }
        }

        private static Texture2D m_PlusIcon;
        internal static Texture2D PlusIcon
        {
            get
            {
                if (m_PlusIcon == null) m_PlusIcon = EditorGUIUtility.FindTexture("Toolbar Plus");
                return m_PlusIcon;
            }
        }

        private static Texture2D m_MinusIcon;
        internal static Texture2D MinusIcon
        {
            get
            {
                if (m_MinusIcon == null) m_MinusIcon = EditorGUIUtility.FindTexture("Toolbar Minus");
                return m_MinusIcon;
            }
        }

        internal static readonly Color HeaderSeparatorColor = new Color32(237, 166, 3, 255);
    }
}