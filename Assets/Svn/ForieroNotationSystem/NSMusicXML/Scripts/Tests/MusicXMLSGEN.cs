/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Diagnostics;
using System.IO;
using UnityEngine;
using ForieroEngine.Music.MusicXML.Xsd;

public class MusicXMLSGEN : MonoBehaviour
{
    public TextAsset file;

    string s = "";

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        if (GUILayout.Button("Deserialize"))
        {
            scorepartwise sp = null;

            Stopwatch watch = new Stopwatch();
            watch.Start();

            using (MemoryStream s = new MemoryStream(file.bytes))
            {
                sp = sp.Load(s);
            }

            watch.Stop();

            s = sp.movementtitle + " " + watch.ElapsedMilliseconds.ToString();
        }
        GUILayout.Label(s);
    }
}
