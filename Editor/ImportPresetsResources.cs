using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace bTools.ImportPresets
{
    public static class ImportPresetsResources
    {
        private static string m_DataAssetPath;
        internal static string DataAssetPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_DataAssetPath))
                {
                    m_DataAssetPath = Directory.GetDirectories(Application.dataPath, "bTools", SearchOption.AllDirectories).FirstOrDefault();
                    if (string.IsNullOrEmpty(m_DataAssetPath))
                    {
                        Debug.LogWarning("Could not find bTools folder - creating one");
                        m_DataAssetPath = Directory.CreateDirectory(Application.dataPath + "bTools").FullName;
                    }
                    m_DataAssetPath = m_DataAssetPath.Replace(Application.dataPath, "Assets");
                    m_DataAssetPath = m_DataAssetPath.Replace('\\', '/');
                    m_DataAssetPath = m_DataAssetPath + @"/ImportPresets/Data/bToolsImportPresets.asset";
                }

                return m_DataAssetPath;
            }
        }

        private static SavedImportSettings m_DataAsset;
        public static SavedImportSettings DataAsset
        {
            get
            {
                if (m_DataAsset == null)
                {
                    m_DataAsset = AssetDatabase.LoadAssetAtPath<SavedImportSettings>(DataAssetPath);

                    if (m_DataAsset == null)
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(DataAssetPath))) Directory.CreateDirectory(Path.GetDirectoryName(DataAssetPath));
                        m_DataAsset = ScriptableObject.CreateInstance<SavedImportSettings>();
                        AssetDatabase.CreateAsset(m_DataAsset, DataAssetPath);
                        AssetDatabase.SaveAssets();
                    }
                }

                return m_DataAsset;
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
        internal static readonly GUIStyle MiniLabelStyle = new GUIStyle(EditorStyles.miniLabel) { fontSize = 9 };
    }
}