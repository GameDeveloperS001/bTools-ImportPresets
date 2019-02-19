using System.Collections.Generic;
using UnityEngine;

namespace bTools.ImportPresets
{
    [System.Serializable]
    public class SavedImportSettings : ScriptableObject
    {
        [SerializeField]
        [HideInInspector]
        public List<ImportConfigBase> meshImportSettingsList = new List<ImportConfigBase>();
        [SerializeField]
        [HideInInspector]
        public List<ImportConfigBase> audioImportSettingsList = new List<ImportConfigBase>();
        [SerializeField]
        [HideInInspector]
        public List<ImportConfigBase> textureImportSettingsList = new List<ImportConfigBase>();
    }
}