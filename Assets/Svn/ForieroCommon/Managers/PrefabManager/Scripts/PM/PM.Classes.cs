using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class PM
{
    [System.Serializable]
    public class PrefabItem
    {
        public string id = "";
        public GameObject prefab = null;
    }

    [System.Serializable]
    public class PrefabCategory
    {
        public enum Tab
        {
            Self,
            Bank
        }

        public Tab tab = Tab.Self;

        public string id = "";
        public PMBank bank = null;
        public List<PrefabItem> items = new List<PrefabItem>();

        public PrefabItem FindPrefabItem(string prefabId)
        {
            foreach (PrefabItem prefabItem in items)
            {
                if (string.IsNullOrEmpty(prefabItem.id))
                {
                    if (prefabItem.prefab != null)
                    {
                        if (prefabItem.prefab.name == prefabId)
                        {
                            return prefabItem;
                        }
                    }
                }
                else
                {
                    if (prefabItem.prefab != null)
                    {
                        if (prefabItem.id == prefabId)
                        {
                            return prefabItem;
                        }
                    }
                }
            }

            if (bank)
            {
                foreach (PrefabItem prefabItem in bank.items)
                {
                    if (string.IsNullOrEmpty(prefabItem.id))
                    {
                        if (prefabItem.prefab != null)
                        {
                            if (prefabItem.prefab.name == prefabId)
                            {
                                return prefabItem;
                            }
                        }
                    }
                    else
                    {
                        if (prefabItem.prefab != null)
                        {
                            if (prefabItem.id == prefabId)
                            {
                                return prefabItem;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public PrefabItem RandomPrefabItem()
        {
            return items[Random.Range(0, items.Count)];
        }
    }
}
