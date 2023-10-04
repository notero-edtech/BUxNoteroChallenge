using UnityEditor;

[CustomEditor(typeof(FMODMidiSeq))]
public class FMODMidiSeqInspector : Editor
{
    private FMODMidiSeq o;
    
    private static readonly string[] _dontIncludeMe = new string[]{"m_Script"};
   
    public void OnEnable()
    {
        o = target as FMODMidiSeq;
        EditorApplication.update += this.Repaint;        
    }
 
    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();
        DrawPropertiesExcluding(serializedObject, _dontIncludeMe);
        if (EditorGUI.EndChangeCheck()) { serializedObject.ApplyModifiedProperties(); }
        
        MidiSeqKaraokeScriptInspector.DrawMidiSeqKaraokeInfo(o.midiSeq);
    }
}