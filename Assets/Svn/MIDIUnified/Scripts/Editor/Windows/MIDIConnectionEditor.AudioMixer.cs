using UnityEditor;

public partial class MIDIConnectionEditor : EditorWindow
{
	//	public static List<AudioMixerController> GetAudioMixerControllers(){
	//		Assembly unityEditorAssembly = typeof(AudioMixerWindow).Assembly;
	//		Type AudioMixerWindowClass = unityEditorAssembly.GetType( "UnityEditor.AudioMixerWindow" );
	//		MethodInfo GetAllControllers = AudioMixerWindowClass.GetMethod( "GetAllControllers", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, new System.Type[] {  }, null );
	//		var returnValue = GetAllControllers.Invoke( [CALLING THIS AudioMixerWindow OBJECT], new System.Object[] { [INSERT THESE PARAMETERS HERE: ] } ) as System.Collections.Generic.List`1[UnityEditor.Audio.AudioMixerController];
	//	}
}