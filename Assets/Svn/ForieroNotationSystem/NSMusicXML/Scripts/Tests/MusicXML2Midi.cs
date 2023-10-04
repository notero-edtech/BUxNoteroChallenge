/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;

public class MusicXML2Midi : MonoBehaviour
{
    public TextAsset xml;

    //XslCompiledTransform xslCT = new XslCompiledTransform();

    void OnGUI()
    {
        if (GUILayout.Button("Transform"))
        {
            //XmlReader r = new XmlReader();
            //xslCompiledTransform.Load();
            //StringBuilder stringBuilder = new StringBuilder();
            //StringWriter stringWriter = new StringWriter(stringBuilder);
            //xslCompiledTransform.Transform(xmlFileNameWithPath, null, stringWriter);
            //stringWriter.Close();
        }
    }
}
