using UnityEngine;
using static SM;

[CreateAssetMenu(menuName = "Scriptable Objects/Sound Manager/SFX Bank")]
public class SMFXBank : ScriptableObject
{
    public FXItem[] items = new FXItem[0];

    public void Find(out FXItem fxItemReturn, string clipId){
        fxItemReturn = null;
        if(items == null) return;
        
        foreach (FXItem fxItem in items)
        {
            if (string.IsNullOrEmpty(fxItem.id))
            {
                if (fxItem.clip != null)
                {
                    if (fxItem.clip.name == clipId)
                    {
                        fxItemReturn = fxItem; return;
                    }
                }
            }
            else
            {
                if (fxItem.clip != null)
                {
                    if (fxItem.id == clipId)
                    {
                        fxItemReturn = fxItem; 
                        return;
                    }
                }
            }
        }
    }
}
