/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections;
using ForieroEngine;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;
using ForieroEngine.Threading.Unity;
using Michsky.UI.ModernUIPack;
using UnityEngine;

public class MSMVPSongsUI : MonoBehaviour
{
    public WindowManager windowManager;
    private NSSessionBank bank => NSSessionSettings.instance.banks[0];
    public GameObject songItemPrefab;
    public RectTransform contentRT;
    public void Awake()
    {
        foreach (var l in bank.sessions)
        {
            var go = Instantiate(songItemPrefab, contentRT);
            var s = go.GetComponent<MSMVPSongItemUI>();
            s.nameText.text = l.name;
            s.session = l;
            s.songs = this;
        }
    }

    public void Play(Session session)
    {
        Scene.FadeOut(0.3f, Color.black, () =>
        {
            var now = DateTime.Now;
            windowManager.gameObject.SetActive(false);
            NSPlayback.Session.Init(session);
            MainThreadDispatcher.Instance.StartCoroutine(FadeIn());
            IEnumerator FadeIn()
            {
                yield return new WaitForSeconds(Mathf.Clamp(1f - (float)((DateTime.Now - now).TotalSeconds), 0f, 10f));
                Scene.FadeIn(0.5f, Color.black, null);
            }
        }, Scene.SceneEnum.Loading);
        
    }
}
