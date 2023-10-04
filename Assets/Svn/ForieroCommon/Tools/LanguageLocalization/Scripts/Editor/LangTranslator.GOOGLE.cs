using System;
using System.Collections;
using System.IO;
using ForieroEditor.CommandLine;
using ForieroEditor.Coroutines;
using ForieroEditor.Extensions;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public static partial class LangTranslator
{
    static string _googleCoudTranslatorSH = "";

    public static string googleCloudTranslatorSH
    {
        get
        {
            if (string.IsNullOrEmpty(_googleCoudTranslatorSH))
            {
                _googleCoudTranslatorSH = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Assets"),
                    "google_cloud_translator.sh", SearchOption.AllDirectories)[0];
            }

            return _googleCoudTranslatorSH;
        }
    }

    static string _jsonPath = null;

    public static string jsonPath
    {
        get
        {
            if (_jsonPath == null)
            {
                _jsonPath = EditorPrefs.GetString("LANG_GOOGLE_JSON_PATH", "");
            }

            return _jsonPath;
        }
        set
        {
            _jsonPath = value;
            EditorPrefs.SetString("LANG_GOOGLE_JSON_PATH", _jsonPath);
        }
    }

    static string _sdkPath = null;

    public static string sdkPath
    {
        get
        {
            if (_sdkPath == null)
            {
                _sdkPath = EditorPrefs.GetString("LANG_GOOGLE_SDK_PATH", "");
            }

            return _sdkPath;
        }
        set
        {
            _sdkPath = value;
            EditorPrefs.SetString("LANG_GOOGLE_SDK_PATH", _sdkPath);
        }
    }

    public static void TranslateAPIV2(string langFrom, string langTo, string text, Action<string> onTranslated,
        bool threaded = false)
    {
        string cmd = googleCloudTranslatorSH;
        string args = jsonPath.DoubleQuotes() + " " + sdkPath.DoubleQuotes() + " " +
                      langFrom.DoubleQuotes() + " " +
                      langTo.DoubleQuotes() + " " +
                      text.DoubleQuotes();

        CMD.GenerateProcess(cmd, args, threaded: threaded, onOutput: (s) =>
        {
            var jo = JObject.Parse(s);
            var translatedText = jo["data"]["translations"][0]["translatedText"].Value<string>();
            onTranslated?.Invoke(translatedText);
        });
    }

    public static void Translate(string langFrom, string langTo, string text, Action<string> onTranslated)
    {
        EditorCoroutineStart.StartCoroutine(Process(langFrom, langTo, text, onTranslated));
    }

    // We have use googles own api built into google Translator.
    public static IEnumerator Process(string targetLang, string sourceText, Action<string> result)
    {
        yield return Process("auto", targetLang, sourceText, result);
    }

    // Exactly the same as above but allow the user to change from Auto, for when google get's all Jerk Butt-y
    public static IEnumerator Process(string sourceLang, string targetLang, string sourceText,
        Action<string> result)
    {
        string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl="
                     + sourceLang + "&tl=" + targetLang + "&dt=t&q=" + UnityWebRequest.EscapeURL(sourceText);

        using (var www = UnityWebRequest.Get(url))
        {
            yield return www;

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                yield break;
            }

            if (string.IsNullOrEmpty(www.error))
            {
                var jo = JObject.Parse(www.downloadHandler.text);
                var translatedText = jo[0][0][0].Value<string>();

                result(translatedText);
            }
        }
    }
}