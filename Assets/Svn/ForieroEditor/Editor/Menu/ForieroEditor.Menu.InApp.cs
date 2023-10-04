using System;
using System.Collections;
using System.IO;
using ForieroEditor.Coroutines;
using ForieroEngine.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace ForieroEditor.Menu
{
    public static partial class MenuItems
    {
        private static readonly Uri uriUnityInApp = new Uri("https://public-cdn.cloud.unity3d.com/UnityEngine.Cloud.Purchasing.unitypackage");

        // Works fine //
        [MenuItem("Foriero/GitHub/Manual IAP Download (UniRx)")]
        static public void DoDownloadUniRx()
        {
           EditorCoroutineStart.StartCoroutine(Download(uriUnityInApp));
        }

        private static readonly Uri uriSpineRuntime = new Uri("https://esotericsoftware.com/files/runtimes/unity/spine-unity.unitypackage");
        private static readonly Uri uriSpineRuntimeBeta = new Uri("https://esotericsoftware.com/files/runtimes/unity/spine-unity-beta.unitypackage");

        [MenuItem("Window/Spine/Update Spine Runtime")]
        static public void DoUpdateSpineRuntime()
        {
            StartBackgroundTask(Download(uriSpineRuntime));
        }

        [MenuItem("Window/Spine/Update Spine Runtime (Beta)")]
        static public void DoUpdateSpineRuntimeBeta()
        {
            StartBackgroundTask(Download(uriSpineRuntimeBeta));
        }

        static void StartBackgroundTask(IEnumerator update, Action end = null)
        {
            EditorApplication.CallbackFunction closureCallback = null;

            closureCallback = () =>
            {
                try
                {
                    if (update.MoveNext() == false)
                    {
                        end?.Invoke();
                        EditorApplication.update -= delegate { closureCallback(); };
                    }
                }
                catch (Exception ex)
                {
                    end?.Invoke();
                    Debug.LogException(ex);
                    EditorApplication.update -= delegate { closureCallback(); };
                }
            };

            EditorApplication.update += closureCallback;
        }

        static IEnumerator Download(Uri uri)
        {
            yield return Download(uri.ToString());
        }

        static IEnumerator Download(string url)
        {
            var www = UnityWebRequest.Get(url.ToString());

            yield return www.SendWebRequest();

            if (www.HasError())
            {
                Debug.LogError(www.error);
            }
            else
            {
                SaveAndImport(www.downloadHandler.data);
            }
        }

        static void SaveAndImport(byte[] bytes)
        {
            var location = FileUtil.GetUniqueTempPathInProject();
            // Extension is required for correct Windows import.
            location = Path.ChangeExtension(location, ".unitypackage");

            File.WriteAllBytes(location, bytes);

            AssetDatabase.ImportPackage(location, false);
        }
    }
}