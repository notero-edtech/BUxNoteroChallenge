using System.IO;
using System.Linq;
using ForieroEditor;
using UnityEditor;

public static class SMEditor
{
    [MenuItem("Foriero/Assets/Banks/Create SoundFX", false, 50)] public static void CreateSoundFXBank() => ScriptableObjects.CreateScriptableObject<SMFXBank>();       
    [MenuItem("Foriero/Assets/Banks/Create Music", false, 50)] public static void CreateMusicBank() => ScriptableObjects.CreateScriptableObject<SMMusicBank>();

#if IT2D
    [MenuItem("Assets/Foriero/Assets/Banks/Create SoundFX")]
    private static void IngetratorTool2DCreateSoundFXBank()
    {
        foreach(var o in Selection.objects){
            if(o is IntegratorTool2DSpineAudioClipsScriptableObject audioClips){
                GenerateSMSFXBank(audioClips);
            }
        }

        AssetDatabase.Refresh();
    }
        
    private static void GenerateSMSFXBank(IntegratorTool2DSpineAudioClipsScriptableObject o)
    {
        var audioClipsAssetPath = AssetDatabase.GetAssetPath(o);
        var sfxBankAssetPath = audioClipsAssetPath.Replace("_AudioClips", "_SMFXBank");
        var sfxBank = Instances.EditorInstance<SMFXBank>(Path.GetFileNameWithoutExtension(sfxBankAssetPath), Path.GetDirectoryName(sfxBankAssetPath));

        sfxBank.items = o.clips.Select(c => new SM.FXItem() { volume = 1, clip = c.audioClip, id = c.spineEventName }).ToArray();
        EditorUtility.SetDirty(sfxBank);
    }

    [InitializeOnLoadMethod]
    static void IntegratorTool2DHooks(){
        IntegratorTool2D.OnAudioClipsGenerated += GenerateSMSFXBank;
    }
#endif
}
