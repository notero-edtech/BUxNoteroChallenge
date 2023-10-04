/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Collections.Generic;
using ForieroEngine.Music.MusicXML.MXL;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using ForieroEngine.Extensions;
using TMPro;

public class NSTestSuiteXML : MonoBehaviour
{
    public GameObject PREFAB_categories_button;
    public GameObject PREFAB_files_button;
    
    public RectTransform xmlCategoriesContainer;
    public RectTransform xmlFilesContainer;
    
    private NSBehaviour NSB => NSBehaviour.instance;
    private NSDebug nsDebug => NSBehaviour.instance.nsDebug;
    private List<RectTransform> categoriesItems = new List<RectTransform>();
    private List<RectTransform> filesItems = new List<RectTransform>();

    bool refreshed = false;

    private void OnEnable()
    {
        if (!refreshed) { OnTestSuitRefreshClick(); }
    }

    private IEnumerator Start()
    {
        yield return new WaitWhile(() => NSB == null);
        yield return new WaitWhile(() => NSB.ns == null);
    }
   
    public void OnTestSuitRefreshClick()
    {
        DestroyCategoriesItems();
        DestroyFileItems();

        StartCoroutine(OnTestSuiteRefreshClickInternal());

        IEnumerator OnTestSuiteRefreshClickInternal()
        {
            var url = "https://backend.foriero.com/unity/notation_system/test_suit.php";
            using var www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            if (www.HasError()) { Debug.LogError(www.error); yield break; }
            var lines = www.downloadHandler.text.Split('\n');
            xmlCategoriesContainer.SetSize(new Vector2(xmlCategoriesContainer.GetSize().x, 50f * lines.Length));
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line.Trim())) continue;
                var go = Instantiate(PREFAB_categories_button, xmlCategoriesContainer, false) as GameObject;
                var button = go.GetComponent<NSTestSuiteButton>();
                button.ItemName = line;
                button.xml = this;
                button.category = line;
                categoriesItems.Add(go.transform as RectTransform);
            }
            refreshed = true;
        }
    }
   
    public void OnItemClick(NSTestSuiteButton item)
    {
        switch (item.buttonType)
        {
            case NSTestSuiteButton.ButtonType.Category: GetFiles(item); break;
            case NSTestSuiteButton.ButtonType.LoadXML: LoadXML(item); break;
            case NSTestSuiteButton.ButtonType.LoadCode: LoadCode(item); break;
            case NSTestSuiteButton.ButtonType.LoadMP3: LoadMP3(item); break;
            case NSTestSuiteButton.ButtonType.ViewPNG: ViewPNG(item); break;
            case NSTestSuiteButton.ButtonType.ViewXML: ViewXML(item); break;
        }
    }

#if NETFX_CORE
	private static IEnumerable<Type> GetBaseTypes(Type type)
	{
	    yield return type;
	 
	    var baseType = type.GetTypeInfo().BaseType;
	 
	    if (baseType != null)
	    {
	        foreach (var t in GetBaseTypes(baseType))
	        {
	            yield return t;
	        }
	    }
	}

	public static PropertyInfo GetProperty(Type type, string name)
	{
	    return
	        GetBaseTypes(type)
	            .Select(baseType => baseType.GetTypeInfo().GetDeclaredProperty(name))
	            .FirstOrDefault(property => property != null);
	}
	 
	public static MethodInfo GetMethod(Type type, string name)
	{
	    return
	        GetBaseTypes(type)
	            .Select(baseType => baseType.GetTypeInfo().GetDeclaredMethod(name))
	            .FirstOrDefault(method => method != null);
	}
	 
	public static FieldInfo GetField(Type type, string name)
	{
	    return
	        GetBaseTypes(type)
	            .Select(baseType => baseType.GetTypeInfo().GetDeclaredField(name))
	            .FirstOrDefault(field => field != null);
	}
#endif


    private void LoadCode(NSTestSuiteButton item)
    {
        // delete all objects //
        NSB.ns.DestroyChildren();

        var staticClassName = "NSTestSuiteCode" + item.category.Replace(" ", "");
        var staticFunctionName = "_" + item.file.Replace("-", "");

        var type = Type.GetType(staticClassName);
#if NETFX_CORE
		GetMethod(type, staticFunctionName).Invoke(null, new object[] { nsBehaviour.ns });
#else
        type.GetMethod(staticFunctionName).Invoke(null, new object[] { NSB.ns });
#endif
    }

    private void ViewPNG(NSTestSuiteButton item)
    {
        StartCoroutine(ViewPNGInternal(item));

        IEnumerator ViewPNGInternal(NSTestSuiteButton item)
        {
            var url = "https://backend.foriero.com/unity/notation_system/test_suit/" +
                            item.category.Replace(" ", "%20") + "/" + item.file.Replace(" ", "%20");

            using var www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();
            if (www.HasError()) { Debug.LogError(www.error); yield break; }
            
            NSTestSuite.Instance.windowManager.OpenWindow("PNG Viewer");
            NSTestSuitePNGViewer.Instance.SetTexture(DownloadHandlerTexture.GetContent(www));
        }
    }

    private void ViewXML(NSTestSuiteButton item)
    {
        StartCoroutine(ViewXMLInternal(item));

        IEnumerator ViewXMLInternal(NSTestSuiteButton item)
        {
            var url = "https://backend.foriero.com/unity/notation_system/test_suit/" +
                      item.category.Replace(" ", "%20") + "/" + item.file.Replace(" ", "%20")
                          .Replace("png", item.xmlType == NSTestSuiteButton.XmlType.xml ? "xml" : "mxl");

            using var www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            if (www.HasError()) { Debug.LogError(www.error); yield break; }
            
            NSTestSuite.Instance.windowManager.OpenWindow("XML Viewer");
            NSTestSuiteXMLViewer.Instance.SetText(www.downloadHandler.data.IsZip() ? System.Text.Encoding.UTF8.GetString(www.downloadHandler.data.Unzip()) : www.downloadHandler.text);
        }
    }

    private void LoadMP3(NSTestSuiteButton item)
    {
        StartCoroutine(LoadMP3Internal(item));
        IEnumerator LoadMP3Internal(NSTestSuiteButton item)
        {
            var watchDownload = System.Diagnostics.Stopwatch.StartNew();
            
            var url = "https://backend.foriero.com/unity/notation_system/test_suit/" + item.category.Replace(" ", "%20") + "/" + item.file.Replace(" ", "%20");

            using var www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
            
            yield return www.SendWebRequest();
            if (www.HasError()) { Debug.LogError(www.error); yield break; }

            Debug.Log("MP3 Downloaded in : " + watchDownload.ElapsedMilliseconds.ToString() + "ms");
            watchDownload.Stop();
            
            var clip = DownloadHandlerAudioClip.GetContent(www);
            if (clip)
            {
                var data = new float[clip.samples];
                if (clip.GetData(data, 0)) { NSB.ns.LoadMusic(data, clip.channels, clip.length); }
            } else { Debug.LogError("Clip is NULL!!!"); }
        }
    }

    private void LoadXML(NSTestSuiteButton item)
    {
        StartCoroutine(LoadXMLInternal(item));
        IEnumerator LoadXMLInternal(NSTestSuiteButton item)
        {
            var watchDownload = System.Diagnostics.Stopwatch.StartNew();

            var url = "https://backend.foriero.com/unity/notation_system/test_suit/" + item.category.Replace(" ", "%20") + "/" + item.file.Replace(" ", "%20");
            
            using var www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            if (www.HasError()) { Debug.LogError(www.error); yield break; }

            Debug.Log("MusicXML Downloaded in : " + watchDownload.ElapsedMilliseconds.ToString() + "ms");
            watchDownload.Stop();

            if (www.HasError()) { Debug.LogError(www.error); yield break; }
            
            NSB.ns.LoadMusicXML(www.downloadHandler.data.IsZip() ? www.downloadHandler.data.Unzip() : www.downloadHandler.data);
            
            NSTestSuite.Instance.windowManager.gameObject.SetActive(false);
        }
    }

    private Dictionary<string, string[]> _categoryCache = new();

    private void GetFiles(NSTestSuiteButton item)
    {
        DestroyFileItems();

        StartCoroutine(GetFilesInternal(item));

        IEnumerator GetFilesInternal(NSTestSuiteButton item)
        {
            var lines = Array.Empty<string>();
            
            if(_categoryCache.ContainsKey(item.category))
            {
                yield return null;
                lines = _categoryCache[item.category];
            } else {
                var url = "https://backend.foriero.com/unity/notation_system/test_suit.php";
                var form = new WWWForm();
                form.AddField("category", item.category);

                using var www = UnityWebRequest.Post(url, form);
                yield return www.SendWebRequest();
                if (www.HasError()) { Debug.LogError(www.error); yield break; }
                lines = www.downloadHandler.text.Split('\n');
                _categoryCache.Add(item.category, lines);
            }

            xmlFilesContainer.SetSize(new Vector2(xmlFilesContainer.GetSize().x, 50f * lines.Length));

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line.Trim())) continue;

                var go = Instantiate(PREFAB_files_button, xmlFilesContainer, false) as GameObject;
                var buttons = go.GetComponentsInChildren<NSTestSuiteButton>();
                foreach (var button in buttons)
                {
                    button.xml = this;
                    button.category = item.category;
                    button.xmlType = line.Contains(".xml") ? NSTestSuiteButton.XmlType.xml : NSTestSuiteButton.XmlType.mxl;
                    switch (button.buttonType)
                    {
                        case NSTestSuiteButton.ButtonType.FileName:
                            button.ItemName = line.Replace(".xml", "").Replace(".mxl", "");
                            button.file = line;
                            break;
                        case NSTestSuiteButton.ButtonType.LoadXML:
                            button.text.text = button.xmlType.ToString().ToUpper();
                            button.file = line;
                            break;
                        case NSTestSuiteButton.ButtonType.LoadCode:
                            button.file = line.Replace(".xml", "").Replace(".mxl", "");
                            break;
                        case NSTestSuiteButton.ButtonType.LoadMP3:
                            button.file = line.Replace(".xml", ".mp3").Replace(".mxl", ".mp3");
                            break;
                        case NSTestSuiteButton.ButtonType.ViewPNG:
                            button.file = line.Replace(".xml", ".png").Replace(".mxl", ".png");
                            break;
                        case NSTestSuiteButton.ButtonType.ViewXML:
                            button.file = line;
                            break;
                    }
                }

                filesItems.Add(go.transform as RectTransform);
            }
        }
    }

    private void DestroyCategoriesItems()
    {
        foreach (var rt in categoriesItems) { Destroy(rt.gameObject); }
        categoriesItems.Clear();
    }

    private void DestroyFileItems()
    {
        foreach (var rt in filesItems) { Destroy(rt.gameObject); }
        filesItems.Clear();
    }
}
