using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Changerbg : MonoBehaviour
{
    public Animator animator;

    private int levelToLoad;

    // Update is called once per frame
    void Start() {
        if (Input.GetMouseButtonDown(0))
        {
            FadeToNextlevel();
        } 
    }
    public void FadeToNextlevel()
    {
        FadeTolevel(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void FadeTolevel (int levelIndex)
    {
        levelToLoad = levelIndex;
        animator.SetTrigger("FadeOut");
    }
    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
