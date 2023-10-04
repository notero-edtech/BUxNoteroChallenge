using System.Collections;
using ForieroEditor.Coroutines;
using ForieroEngine.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class EditorCoroutineDemo
{
    [MenuItem("Foriero/Others/Coroutines/DemoUI")]
    public static void DemoEditorCoroutinesUI()
    {
        // adds a menu item to test the coroutine system. 
        if (!Application.isPlaying)
        {
            // lets fire off the demo coroutine with a UI so we can see what its doing. We could also run it without a UI by using EditorCoroutineStart.StartCoroutine(...)
            EditorCoroutineStart.StartCoroutineWithUI(DemoCoroutiune(), "Coroutine Demo", true);
        }
    }

    [MenuItem("Foriero/Others/Coroutines/Demo")]
    public static void DemoEditorCoroutines()
    {
        // adds a menu item to test the coroutine system. 
        if (!Application.isPlaying)
        {
            // lets fire off the demo coroutine with a UI so we can see what its doing. We could also run it without a UI by using EditorCoroutineStart.StartCoroutine(...)
            EditorCoroutineStart.StartCoroutine(DemoCoroutiune());
        }
    }

    static IEnumerator DemoCoroutiune()
    {
        // You can code editor coroutines exactly like you would a normal unity coroutine
        Debug.Log("Step: 0");
        yield return null;

        // all the normal return types that work with regular Unity coroutines should work here! for example lets wait for a second
        Debug.Log("Step: 1");
        yield return new WaitForSeconds(1);

        // We can also yeild any type that extends Unitys CustomYieldInstruction class. here we are going to use EditorStatusUpdate. this allows us to yield and update the
        // editor coroutine UI at the same time!
        yield return new EditorStatusUpdate("coroutine is running", 0.2f);

        // We can also yield to nested coroutines
        Debug.Log("Step: 2");

        yield return EditorCoroutineStart.StartCoroutine(DemoTwo());
        EditorCoroutineStart.UpdateUIProgressBar(0.35f); // we can use the UpdateUI helper methods to update the UI whenever, without yielding a EditorStatusUpdate
        yield return DemoTwo(); // it shouldnt matter how we start the nested coroutine, the editor runner can hadle it

        // we can even yield a WWW object if we want to grab data from the internets!
        Debug.Log("Step: 3");

        // for example, lets as random.org to generate us a list of random numbers and shove it into the console
        var www = UnityWebRequest.Get("https://www.random.org/integers/?num=100&min=1&max=1000&col=1&base=10&col=5&format=plain&rnd=new");
        yield return www.SendWebRequest();
        if(www.HasError()) Debug.Log(www.error);
        Debug.Log(www.downloadHandler.text);

        EditorCoroutineStart.UpdateUI("Half way!", 0.5f);
        yield return new WaitForSeconds(1);

        // Finally lets do a long runnig task and split its updates over many frames to keep the editor responsive
        Debug.Log("Step: 4");
        var test = 1000;
        yield return new WaitUntil(() =>
        {
            test--;
            EditorCoroutineStart.UpdateUI("Crunching Numbers: " + test, 0.5f + (((1000 - test) / 1000f) * 0.5f));
            return (test <= 0);
        });
        Debug.Log("Done!!");
    }

    static IEnumerator DemoTwo()
    {
        Debug.Log("TESTTWO: Starting second test coroutine");
        yield return new WaitForSeconds(1.2f);
        Debug.Log("TESTTWO: finished second test coroutine");
    }
}