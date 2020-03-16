using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
                    string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();

                    for (int i = 0; i < allAssetPaths.Length; i++)
                    {
                        Object obj = AssetDatabase.LoadAssetAtPath<Object>(allAssetPaths[i]);

                        if (obj == null)
                            continue;

                        if (obj.GetType() == typeof(SavedImportSettings))
                        {
                            m_DataAsset = (SavedImportSettings) obj;
                            break;
                        }
                    }

                    if (m_DataAsset == null)
                    {
                        string path = DataAssetPath.Replace(Application.dataPath, @"Assets/");
                        if (!Directory.Exists(Path.GetDirectoryName(path))) Directory.CreateDirectory(Path.GetDirectoryName(path));
                        m_DataAsset = ScriptableObject.CreateInstance<SavedImportSettings>();
                        AssetDatabase.CreateAsset(m_DataAsset, path);
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

        private static GUIStyle m_MiniLabelStyle;
        internal static GUIStyle MiniLabelStyle
        {
            get
            {
                if (m_MiniLabelStyle == null) m_MiniLabelStyle = new GUIStyle(EditorStyles.miniLabel) { fontSize = 9 };
                return m_MiniLabelStyle;
            }
        }

        internal static readonly Color HeaderSeparatorColor = new Color32(237, 166, 3, 255);
        internal const string VerboseApplyKey = "bTools.ImportPresets.VerboseApply";
    }
}