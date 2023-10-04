using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ForieroSRDebugger : MonoBehaviour
{

    static ForieroSRDebugger forieroSRDebugger = null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Init()
    {
        System.Diagnostics.Stopwatch stopWatch = ForieroDebug.CodePerformance ? System.Diagnostics.Stopwatch.StartNew() : null;
        if (!forieroSRDebugger)
        {
            var go = new GameObject("ForieroSRDebugger");
            go.AddComponent<ForieroSRDebugger>();
        }
        if(ForieroDebug.CodePerformance) Debug.Log("METHOD STOPWATCH (ForieroSRDebugger - AfterSceneLoad): " + stopWatch?.Elapsed.ToString());
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void OpenSRDebugger()
    {
        SRDebug.Instance.ShowDebugPanel();
    }

    int touchCount = 0;
    bool addTouch = false;

    void Update()
    {
        #if ENABLE_INPUT_SYSTEM
        if (Keyboard.current[Key.LeftCtrl].isPressed && Keyboard.current[Key.R].wasPressedThisFrame)
        #else
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        #endif
        {
            OpenSRDebugger();
        }

        if (Input.touchCount == 2)
        {
            addTouch = true;
        }
        else if (Input.touchCount == 3)
        {
            if (addTouch)
            {

                addTouch = false;
                touchCount++;

                if (touchCount == 3)
                {
                    OpenSRDebugger();
                    touchCount = 0;
                }

            }
        }
        else
        {
            touchCount = 0;
        }
    }
}
