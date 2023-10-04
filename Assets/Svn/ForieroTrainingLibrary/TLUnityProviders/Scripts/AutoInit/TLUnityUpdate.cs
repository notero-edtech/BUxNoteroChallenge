/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.Training;
using UnityEngine;

public class TLUnityUpdate : MonoBehaviour
{
    static TLUnityUpdate instance = null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Init()
    {
        System.Diagnostics.Stopwatch stopWatch = Debug.isDebugBuild ? System.Diagnostics.Stopwatch.StartNew() : null;
        if (instance == null)
        {
            instance = new GameObject("TLUnityUpdate").AddComponent<TLUnityUpdate>();
            GameObject.DontDestroyOnLoad(instance);
        }
        if(Debug.isDebugBuild) Debug.Log("METHOD STOPWATCH (TLUnityUpdate - AfterSceneLoad): " + stopWatch?.Elapsed.ToString());
    }

    void Update()
    {
        if (TL.Inputs.OnUpdate != null)
        {
            TL.Inputs.OnUpdate();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (TL.Inputs.OnSpacebarDown != null)
            {
                TL.Inputs.OnSpacebarDown();
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (TL.Inputs.OnSpacebarUp != null)
            {
                TL.Inputs.OnSpacebarUp();
            }
        }
    }
}
