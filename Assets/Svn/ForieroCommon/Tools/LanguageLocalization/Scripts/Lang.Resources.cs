using UnityEngine;
using System;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.IO;
using System.Linq;
using ForieroEngine.Threading.Unity;

#if UNITY_EDITOR
using UnityEditor;
using ForieroEditor.Extensions;
using ForieroEditorInternal.Extensions;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.AddressableAssets;
#endif

public static partial class Lang
{
    public class LangResourceItem
	{
		public AudioClip clip;
		public TextAsset phonemes;
	}
        
    public static void GetLangResourceItemAsync(Action<LangResourceItem> onLangResourceItem, string dictionary, string id, Lang.LanguageCode languageCode = LanguageCode.Unassigned)
    {
        MainThreadDispatcher.Instance.StartCoroutine(GetLangResourceItemAsyncCoroutine(onLangResourceItem, dictionary, id, languageCode));
    }
       
    public static IEnumerator GetLangResourceItemAsyncCoroutine(Action<LangResourceItem> onLangResourceItem, string dictionary, string id, Lang.LanguageCode languageCode = LanguageCode.Unassigned)
    {
        AudioClip clip = null;
        TextAsset phonemes = null;

        var clipAddress = GetAudioAddressableAddress(dictionary, id, languageCode);
        var clipLocationAsync = Addressables.LoadResourceLocationsAsync(clipAddress);
        yield return clipLocationAsync;

        if (clipLocationAsync.Status == AsyncOperationStatus.Succeeded && clipLocationAsync.Result.Count > 0)
        {
            var clipAsync = Addressables.LoadAssetAsync<AudioClip>(clipAddress);
            yield return clipAsync;

            if (clipAsync.Status == AsyncOperationStatus.Succeeded) clip = clipAsync.Result;
        }

        var phonemesAddress = GetPhonemesAddressableAddress(dictionary, id, languageCode);
        var phonemesLocationAsync = Addressables.LoadResourceLocationsAsync(phonemesAddress);
        yield return phonemesLocationAsync;

        if (phonemesLocationAsync.Status == AsyncOperationStatus.Succeeded && phonemesLocationAsync.Result.Count > 0)
        {
            var phonemesAsync = Addressables.LoadAssetAsync<TextAsset>(phonemesAddress);
            yield return phonemesAsync;

            if (phonemesAsync.Status == AsyncOperationStatus.Succeeded) phonemes = phonemesAsync.Result;
        }

        onLangResourceItem?.Invoke(new LangResourceItem { clip = clip, phonemes = phonemes });        
    }

    public static string GetFMODEventPath(string dictionary, string id, Lang.LanguageCode languageCode = LanguageCode.Unassigned)
    => $"languages/{dictionary}/{(languageCode == Lang.LanguageCode.Unassigned ? Lang.selectedLanguage : languageCode)}/{id}";
    
    
    static string GetAddressableBaseAddress(string dictionary, string id, Lang.LanguageCode languageCode = LanguageCode.Unassigned)
        => $"{(languageCode == Lang.LanguageCode.Unassigned ? Lang.selectedLanguage : languageCode)}.{LangSettings.instance.guid}.{dictionary}.{id}";

    static string GetAudioAddressableAddress(string dictionary, string id, Lang.LanguageCode languageCode = LanguageCode.Unassigned)
        => $"{GetAddressableBaseAddress(dictionary, id, languageCode)}.ogg";

    static string GetPhonemesAddressableAddress(string dictionary, string id, Lang.LanguageCode languageCode = LanguageCode.Unassigned)
        => $"{GetAddressableBaseAddress(dictionary, id, languageCode)}.txt";

#if UNITY_EDITOR
    public static void UpdateAddressables()
    {
        Clean();
        Debug.Log("Lang | Updating Language Addressables...");
        foreach (var d in LangSettings.instance.dictionaries)                    
            foreach (var l in LangSettings.instance.supportedLanguages)            
                if (l.included) UpdateAddressableDictionary(LangSettings.instance.guid, d, l.langCode);

    }

    public static void Clean()
    {
        Debug.Log("Lang | Cleaning Language Addressables...");
        foreach (var d in LangSettings.instance.dictionaries)
            foreach (var l in LangSettings.instance.supportedLanguages)
            {
                var addrGroupName = $"{l.langCode}.{LangSettings.instance.guid}.{d.aliasName}";
                if (AddressablesHelper.GroupExists(addrGroupName)) {
                    var group = AddressablesHelper.GetGroup(addrGroupName);
                    var entries = group.entries.ToList();
                    for (int i = entries.Count - 1; i >= 0; i--) {
                        group.RemoveAssetEntry(entries[i]);
                    }
                }
                //else
                //    Debug.LogWarning($"Lang | Addressable Group does not exists : {addrGroupName}");
            }
    }

    static void UpdateAddressableDictionary(string guid, LangSettings.LangDictionary langDictionary, Lang.LanguageCode languageCode)
    {
        var dictionary = langDictionary.aliasName;

        Debug.Log("Lang | Updating Language Reference : " + languageCode.ToString() + " " + dictionary);
                
        var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets/Resources Localization/" + guid + "/LanguageAudios/" + dictionary + "/" + languageCode);

        if (!Directory.Exists(sourcePath)) return;

        string[] audioFiles = Directory.GetFiles(sourcePath, "*" + ForieroEngine.Extensions.ForieroEngineExtensions.PlatformRuntimeAudioExtension(), SearchOption.TopDirectoryOnly);
        
        var addrGroupName = $"{languageCode}.{guid}.{dictionary}";
        
        var addrGroup = AddressablesHelper.GroupExists(addrGroupName)
            ? AddressablesHelper.GetGroup(addrGroupName)
            : AddressablesHelper.CreateGroup(addrGroupName);

        var addrGroupSchema = addrGroup.GetSchema<BundledAssetGroupSchema>();

        switch (langDictionary.stored)
        {
            case LangSettings.LangDictionary.Storage.InBuild:
                addrGroupSchema.BuildPath.SetVariableByName(AddressableAssetSettingsDefaultObject.Settings, "LocalBuildPath");
                addrGroupSchema.LoadPath.SetVariableByName(AddressableAssetSettingsDefaultObject.Settings, "LocalLoadPath");
                break;
            case LangSettings.LangDictionary.Storage.OnServer:
                addrGroupSchema.BuildPath.SetVariableByName(AddressableAssetSettingsDefaultObject.Settings, "RemoteBuildPath");
                addrGroupSchema.LoadPath.SetVariableByName(AddressableAssetSettingsDefaultObject.Settings, "RemoteLoadPath");                
                break;
        }
        
        foreach (var f in audioFiles)
        {
            var clipAssetPath = f.GetAssetPathFromFullPath();

            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(clipAssetPath);
            if (!clip.IsInAddressableAssetGroup(addrGroupName)) {
                var address = $"{addrGroupName}.{Path.GetFileName(clipAssetPath)}";
                var entry = clip.CreateAssetEntry(addrGroupName, address);              
                entry.SetLabel(addrGroupName, true, true);
            }
        }
 
        string[] phonemeFiles = Directory.GetFiles(sourcePath, "*.txt", SearchOption.TopDirectoryOnly);        
        foreach (var f in phonemeFiles)
        {
            var phonemesAssetPath = f.GetAssetPathFromFullPath();

            var phoneme = AssetDatabase.LoadAssetAtPath<TextAsset>(phonemesAssetPath);
            if (!phoneme.IsInAddressableAssetGroup(addrGroupName))
            {
                var address = $"{addrGroupName}.{Path.GetFileName(phonemesAssetPath)}";
                var entry = phoneme.CreateAssetEntry(addrGroupName, address);
                entry.SetLabel(addrGroupName, true, true);
            }
        }
    }
#endif
}
