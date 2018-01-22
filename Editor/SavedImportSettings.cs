using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace bTools.ImportPresets
{

    [System.Serializable]
    public class SavedImportSettings : ScriptableObject
    {
        [SerializeField]
        [HideInInspector]
        public List<MeshImportConfig> meshImportSettingsList = new List<MeshImportConfig>();
        [SerializeField]
        [HideInInspector]
        public List<AudioImportConfig> audioImportSettingsList = new List<AudioImportConfig>();
        [SerializeField]
        [HideInInspector]
        public List<TextureImportConfig> textureImportSettingsList = new List<TextureImportConfig>();
    }
}