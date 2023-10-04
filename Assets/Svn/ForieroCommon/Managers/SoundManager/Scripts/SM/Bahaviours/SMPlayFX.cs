using UnityEngine;
//using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;

public class SMPlayFX : MonoBehaviour
{

    [System.Serializable]
    public class FXItem
    {
        public string id;
        //[BoxGroup("SM settings")]
        public string group_id;
        //[BoxGroup("SM settings")]
        public string clip_id;
        //[BoxGroup("SM settings")]
        public float volume = 1;
    }

    //[ValidateInput("itemsValidation", "repeated ids on the list")]
    public List<FXItem> sfx_items;

    public void PlaySFX(string id)
    {
        for (int i = 0; i < sfx_items.Count; i++)
        {
            if (sfx_items[i].id != id)
                continue;

            SM.PlayFX(sfx_items[i].clip_id, sfx_items[i].volume, sfx_items[i].group_id);
            break;
        }
    }

    bool itemsValidation(List<FXItem> _sfx_items)
    {
        List<string> ids = new List<string>();
        for (int i = 0; i < _sfx_items.Count; i++)
        {
            if (ids.Contains(_sfx_items[i].id))
                return false;
            else
                ids.Add(_sfx_items[i].id);
        }
        return true;
    }
}
