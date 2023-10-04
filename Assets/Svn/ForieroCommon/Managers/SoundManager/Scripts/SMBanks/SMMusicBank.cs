using UnityEngine;
using static SM;

[CreateAssetMenu(menuName = "Scriptable Objects/Sound Manager/Music Bank")]
public class SMMusicBank : ScriptableObject
{
    public SM.MusicItem[] items = new SM.MusicItem[0];

    public void Find(out MusicItem musicItemReturn, string musicId){
        musicItemReturn = null;

        foreach (MusicItem musicItem in items)
        {
            if (string.IsNullOrEmpty(musicItem.id))
            {
                if (musicItem.clip != null)
                {
                    if (musicItem.clip.name == musicId)
                    {
                        musicItemReturn = musicItem; return;
                    }
                }
            }
            else
            {
                if (musicItem.clip != null)
                {
                    if (musicItem.id == musicId)
                    {
                        musicItemReturn = musicItem; return;
                    }
                }
            }
        }
    }
}
