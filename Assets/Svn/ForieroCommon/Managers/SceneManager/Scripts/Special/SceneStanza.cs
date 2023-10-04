using DG.Tweening;
using UnityEngine;

public class SceneStanza : MonoBehaviour
{

    public int destroySceneIndex = 2;
    public float fadeOutTime = 2f;
    public AudioSource audioSource;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += (UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode) =>
        {
            if (scene.buildIndex == destroySceneIndex)
            {
                audioSource.DOFade(0f, fadeOutTime).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    Destroy(this.gameObject);
                });
            }
        };
    }
}
