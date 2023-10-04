using UnityEngine;
using ForieroEngine;
using System.Collections.Generic;
using ForieroEngine.Settings;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SettingsManager]
public partial class PrefabSettings : Settings<PrefabSettings>, ISettingsProvider
{
#if UNITY_EDITOR
    [MenuItem("Foriero/Settings/Prefab", false, -1000)] public static void PrefabSettingsMenu() => Select();   
#endif
   
    public List<PM.PrefabCategory> categories = new List<PM.PrefabCategory>();

    PM.PrefabItem prefabItem = null;

    public PM.PrefabItem FindPrefabItem(string prefabId, string categoryId = "")
    {
        prefabItem = null;

        foreach (PM.PrefabCategory category in categories)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                prefabItem = category.FindPrefabItem(prefabId);

                if (prefabItem != null) return prefabItem;
            }
            else
            {
                if (category.id == categoryId)
                {
                    return category.FindPrefabItem(prefabId);
                }
            }
        }

        if (Foriero.debug) Debug.LogWarning("Prefab not found : " + prefabId);

        return null;
    }

    public PM.PrefabItem RandomPrefabItem(string categoryId = "")
    {
        prefabItem = null;

        foreach (PM.PrefabCategory category in categories)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                prefabItem = category.RandomPrefabItem();

                if (prefabItem != null) return prefabItem;
            }
            else
            {
                if (category.id == categoryId)
                {
                    return category.RandomPrefabItem();
                }
            }
        }

        if (Foriero.debug) Debug.LogWarning("No prefabs found");

        return null;
    }
}
