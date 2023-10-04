using System.Collections;
using System.IO;
using ForieroEditor.Coroutines;
using ForieroEditor.Extensions;
using ForieroEngine.Extensions;
using UnityEngine;
using UnityEngine.Networking;

namespace ForieroEditor
{
    public static class NuGet
	{
        static string baseUrl = "https://www.nuget.org/api/v2/package/";

        public static void Get (string packageName, string packageVersion, string platform, string assetsPath)
		{

           

            string path = ForieroEditorExtensions.FixOSPath (Path.Combine (Directory.GetCurrentDirectory (), assetsPath));
            			
			string directory = Path.GetDirectoryName (path);

			if (!Directory.Exists (directory)) {
				Directory.CreateDirectory (directory);
			}

            EditorCoroutineStart.StartCoroutine(DownloadNuget(packageName, packageVersion, platform, assetsPath));

            //ObservableWWW.GetAndGetBytes(url).Subscribe((bytes) =>
            //{
            //    downloadCounter--;
            //    var fileName = Path.Combine(downloadDirectory, Path.GetFileName(url));
            //    File.WriteAllBytes(fileName, bytes);
            //    Debug.Log("Saved : " + fileName);

            //    if (ZipFile.CheckZip(fileName))
            //    {
            //        var zip = ZipFile.Read(fileName);

            //        void Process(string from, string to)
            //        {
            //            if (!string.IsNullOrEmpty(from))
            //            {
            //                if (zip.EntryFileNames.Contains(from))
            //                {
            //                    string toFileName = Path.Combine(unityPluginsDirectory, to);

            //                    if (!File.Exists(toFileName)) Debug.LogError("WEIRD PLUGIN SHOUDL EXISTS : " + toFileName);

            //                    MemoryStream memoryStream = new MemoryStream();
            //                    zip[from].Extract(memoryStream);
            //                    File.WriteAllBytes(toFileName, memoryStream.ToArray());
            //                    Debug.Log("Saving plugin : " + toFileName);
            //                }
            //                else
            //                {
            //                    Debug.LogError("NOT FOUND : " + from);
            //                }
            //            }
            //        }

            //        foreach (var pluginItem in plugin.plugins)
            //        {
            //            Process(pluginItem.pluginPath, pluginItem.unityPath);
            //        }
            //    }
            //    if (downloadCounter == 0) AssetDatabase.Refresh();
            //},
            //(ex) =>
            //{
            //    downloadCounter--;
            //    if (downloadCounter == 0) AssetDatabase.Refresh();
            //    Debug.LogError(url);
            //});
        }

        static IEnumerator DownloadNuget(string packageName, string packageVersion, string platform, string assetsPath)
        {
            string url = baseUrl + packageName + "/" + packageVersion;
            Debug.Log(url);
            using (UnityWebRequest www = new UnityWebRequest(url))
            {
                yield return www.SendWebRequest();

                if (www.HasError())
                {
                    Debug.Log(www.error);
                }
                else
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "Temp/" + packageName + ".zip");
                    Debug.Log(path);
                    File.WriteAllBytes(path, www.downloadHandler.data);
                }
            }
        }        
	}
}
