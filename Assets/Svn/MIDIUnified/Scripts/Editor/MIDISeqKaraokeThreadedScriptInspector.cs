using UnityEditor;

[CustomEditor(typeof(MidiSeqKaraokeThreadedScript))]
public class MIDISeqKaraokeScriptInspector : Editor
{
    MidiSeqKaraokeThreadedScript o;
             
    public void OnEnable()
    {
        o = target as MidiSeqKaraokeThreadedScript;       
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        DrawDefaultInspector();
        
        EditorGUI.BeginChangeCheck();
          
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}