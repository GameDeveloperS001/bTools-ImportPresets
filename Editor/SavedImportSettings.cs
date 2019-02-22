using System.Collections.Generic;
using UnityEngine;

namespace bTools.ImportPresets
{
    [System.Serializable]
    public class SavedImportSettings : ScriptableObject
    {
        [SerializeField]
        [HideInInspector]
        public List<ImportConfig> importConfigs = new List<ImportConfig>();
    }
}
