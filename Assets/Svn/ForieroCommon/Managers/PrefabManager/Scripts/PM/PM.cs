using UnityEngine;

public static partial class PM
{
    static PrefabItem prefabItem;

    public static GameObject Instantiate(string prefabId, string categoryId = "")
    {
        prefabItem = null;

        prefabItem = PrefabSettings.instance.FindPrefabItem(prefabId, categoryId);

        if (prefabItem == null)
        {
            return null;
        }
        else
        {
            return GameObject.Instantiate<GameObject>(prefabItem.prefab);
        }
    }
}
