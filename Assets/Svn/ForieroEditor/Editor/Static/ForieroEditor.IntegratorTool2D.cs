using UnityEngine;
using System.IO;
using ForieroEditor.Extensions;
using System.Collections.Generic;
using UnityEditor;
using System.Xml.Linq;
using System.Linq;

using UnityEditor.SceneManagement;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ForieroEditor
{
    public static partial class IntegratorTool2DEditor
    {
        public static partial class RecreateScene
        {
            public enum ImageType
            {
                png,
                svg
            }

            static string assetXMLFilePath = "";
            static ImageType imageType = ImageType.png;
            static Material vectorMaterial = null;
            static Material vectorGradientMaterial = null;
            static float scaleFactor = 1f;
            static float pixelOffset = 0f;
            static float z = 0f;
            public static float zIncrements = 0.001f;
            static Vector2 illustratorDesigResolution;
            static Vector2 unityDesignResolution;


            public static void GenerateHierarchy(string importUnityXml, string name, ImageType imageType = ImageType.png)
            {
                if (!importUnityXml.IsItAssetPath())
                {
                    Debug.LogError("IT2D | Source images were exported outside unity project!!! : " + importUnityXml);
                    return;
                }

                TextAsset f = AssetDatabase.LoadAssetAtPath(importUnityXml, typeof(TextAsset)) as TextAsset;
                RecreateScene.assetXMLFilePath = importUnityXml.FixAssetsPath().Replace("imports/import_unity.xml", "");
                Debug.Log("Recreating : " + RecreateScene.assetXMLFilePath);
                RecreateScene.imageType = imageType;

#if VECTOR_GRAPHICS
                if (!vectorMaterial) vectorMaterial = new Material(Shader.Find("Unlit/Vector"));
                if (!vectorGradientMaterial) vectorGradientMaterial = new Material(Shader.Find("Unlit/VectorGradient"));
#endif
                if (!f)
                {
                    Debug.LogError("INTEGRATORTOOL2D: Can not fin import_unity.xml file!!! : " + importUnityXml);
                    return;
                }

                Camera camera = null;

                GameObject cameraObject = new GameObject("Integrator Camera");
                cameraObject.transform.position = new Vector3(0, 0, -10);
                camera = cameraObject.AddComponent<Camera>();
                camera.orthographic = true;

                try
                {
                    XDocument doc = XDocument.Load(f.text.GetMemoryStream());

                    var ai2unity = doc.Element("AI2UNITY");

                    XAttribute orthoSizeAttribute = ai2unity.Attribute("orthoSize");

                    if (orthoSizeAttribute != null)
                    {
                        camera.orthographicSize = int.Parse(orthoSizeAttribute.Value);
                    }
                    else
                    {
                        Debug.LogError("Could not find orthographics size attribute. Item will be reconstructed with orthographics size 5 wich may or may not lead to wrong visual result.");
                    }

                    string objectName = "";

                    if (string.IsNullOrEmpty(name))
                    {
                        objectName = Path.GetFileNameWithoutExtension(ai2unity.Attribute("fileName").Value);
                    }
                    else
                    {
                        objectName = Path.GetFileNameWithoutExtension(name);
                    }

                    GameObject goRoot = new GameObject(objectName);

                    scaleFactor = float.Parse(ai2unity.Attribute("scaleFactor").Value, CultureInfo.InvariantCulture);

                    illustratorDesigResolution = new Vector2(
                        float.Parse(ai2unity.Attribute("illustratorDesignResolutionWidth").Value, CultureInfo.InvariantCulture),
                        float.Parse(ai2unity.Attribute("illustratorDesignResolutionHeight").Value, CultureInfo.InvariantCulture)
                    );

                    unityDesignResolution = new Vector2(
                        float.Parse(ai2unity.Attribute("unityDesignResolutionWidth").Value, CultureInfo.InvariantCulture),
                        float.Parse(ai2unity.Attribute("unityDesignResolutionHeight").Value, CultureInfo.InvariantCulture)
                    );

                    float aspect = illustratorDesigResolution.x / illustratorDesigResolution.y;
                    Vector3 rightPoint = new Vector3(camera.orthographicSize * aspect, camera.transform.position.y, 0);
                    Vector3 rightPixelPoint = camera.WorldToScreenPoint(rightPoint);
                    Vector3 leftPoint = new Vector3(-camera.orthographicSize * aspect, camera.transform.position.y, 0);
                    Vector3 leftPixelPoint = camera.WorldToScreenPoint(leftPoint);

                    pixelOffset = camera.WorldToScreenPoint(new Vector3(-camera.orthographicSize * aspect, camera.transform.position.y, 0)).x;
                    scaleFactor *= Vector3.Distance(leftPixelPoint, rightPixelPoint) / unityDesignResolution.x;

                    var aiItems = ai2unity.Elements();

                    z = 0;

                    foreach (var aiItem in aiItems)
                    {
                        z = int.Parse(aiItem.Attribute("layerId").Value);
                        GenerateAIItem(aiItem, goRoot.transform, null, camera);
                    }

                    Extensions.ForieroEditorExtensions.CenterPivot(goRoot.transform, true, true);

                }
                catch (System.Exception e)
                {
                    Debug.LogError("Invalid XML: " + importUnityXml + " " + e.Message);
                }
                finally
                {
                    if (camera)
                    {
                        Object.DestroyImmediate(camera.gameObject);
                    }
                }

                Debug.Log("If your reconstructed object does not look as it shoud then there could be two interconnected reasons." +
                "\n" +
                "You dont have camera in the scene that is orthograhics." +
                "\n" +
                "Your orthographics camera size is different to what the sprites were imported with in 2D Integrator Tool. ( Try sizes 1 or 5 )"
                );

                EditorSceneManager.MarkAllScenesDirty();
            }

            /*
#1 What is in Illustrator or Photoshop upper in hierarchy it is closed to camera
            */

            static void GenerateAIItem(XElement aiItem, Transform parent, XElement aiItemParent, Camera camera)
            {
                string name = aiItem.Attribute("itemName").Value;
                string tag = aiItem.Attribute("tag").Value;

                if (tag.Contains("include") || HasChildInclude(aiItem))
                {
                    GameObject goItem = new GameObject(name);
                    goItem.transform.SetParent(parent);
                    goItem.transform.position = new Vector3(goItem.transform.position.x, goItem.transform.position.y, z);

                    if (Regex.IsMatch(goItem.name, @"^.*_[Ss]\d+$"))
                    {
                        var index = goItem.name.LastIndexOf("_S", System.StringComparison.InvariantCulture);
                        if (index < 0) index = goItem.name.LastIndexOf("_s", System.StringComparison.InvariantCulture);

                        var scaleText = goItem.name.Substring(index + 2);

                        if (float.TryParse(scaleText, out var scale)) goItem.transform.localScale = Vector3.one * 100f / scale;
                    }

                    z += zIncrements;

                    if (tag.Contains("include"))
                    {
                        SpriteRenderer sr = goItem.AddComponent<SpriteRenderer>();
                        sr.sprite = GetAIPathSprite(aiItem);

                        if (imageType == ImageType.svg)
                        {
                            if (sr.sprite.texture == null) sr.material = vectorMaterial;
                            else sr.material = vectorGradientMaterial;
                        }

                        sr.sortingOrder = -1 * int.Parse(aiItem.Attribute("layerId").Value);
                        SetAIItemPosition(aiItem, goItem.transform, camera);
                    }

                    var aiItems = aiItem.Elements();
                    foreach (var item in aiItems)
                    {
                        GenerateAIItem(item, goItem.transform, aiItem, camera);
                    }
                }
            }

            static void SetAIItemPosition(XElement aiItem, Transform transform, Camera camera)
            {
                float x = float.Parse(aiItem.Attribute("x").Value, CultureInfo.InvariantCulture) * scaleFactor;
                float y = float.Parse(aiItem.Attribute("y").Value, CultureInfo.InvariantCulture) * scaleFactor;

                transform.position = camera.ScreenToWorldPoint(new Vector3(pixelOffset + x, y, Vector3.Distance(camera.transform.position, transform.position)));
            }

            public static string GetResourceFilePath(XElement aiItem, string baseDir)
            {
                string assetPath = GetResourceDirectoryPath(aiItem, baseDir);

                return Path.Combine(assetPath, aiItem.Attribute("itemName").Value + "." + imageType.ToString()).FixOSPath();
            }

            public static string GetResourceDirectoryPath(XElement aiItem, string baseDir)
            {
                string itemPath = aiItem.Attribute("itemPath").Value;

                while (itemPath.Length > 0 && itemPath.Substring(0, 1) == "/") itemPath = itemPath.Substring(1);

                Debug.Log("Combining : " + baseDir + " " + itemPath);

                string result = Path.Combine(baseDir, itemPath).FixOSPath();

                Debug.Log("Combined : " + result);

                return result;
            }

            static Sprite GetAIPathSprite(XElement aiItem)
            {
                string p = GetResourceFilePath(aiItem, Path.GetDirectoryName(assetXMLFilePath)).FixAssetsPath();
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(p);
                if (sprite == null) Debug.LogError("Sprite does not exists (Check Texture Importer is set to Sprite?) : " + p);
                return sprite;
            }

            public static bool HasChildInclude(XElement aiItem)
            {
                var elements = (from e in aiItem.Descendants("AIItem")
                                where e.Attribute("tag").Value.Contains("include")
                                select e);
                return elements.Count() > 0;
            }

            class FileExists
            {
                public string path = "";
                public bool exists = false;
            }
                        
            public static void Clean(string exportXmlFullPath, string[] excludedFiles, bool alert = true, bool debug = false)
            {
                if (!File.Exists(exportXmlFullPath.FixOSPath()))
                {
                    Debug.LogError("FILE NOT FOUND : " + exportXmlFullPath);
                    return;
                }

                string exportPath = exportXmlFullPath.FixOSPath().Replace("/imports/import_unity.xml".FixOSPath(), "");

                if (!Directory.Exists(exportPath))
                {
                    Debug.LogError("DIRECTORY NOT FOUND : " + exportPath);
                    return;
                }

                XDocument doc = XDocument.Load(ForieroEditor.Extensions.ForieroEditorExtensions.GetMemoryStream(File.ReadAllText(exportXmlFullPath.FixOSPath())));

                var ai2unity = doc.Element("AI2UNITY");

                string[] files = Directory.GetFiles(exportPath, "*.*", SearchOption.AllDirectories)
                    .Where(s =>
                    s.EndsWith(".png", System.StringComparison.Ordinal)
                    || s.EndsWith(".jpg", System.StringComparison.Ordinal)
                    || s.EndsWith(".svg", System.StringComparison.Ordinal)
                    || s.EndsWith(".ai", System.StringComparison.Ordinal)
                    ).ToArray();

                var filesThatExist = files.Select(g => new FileExists()
                {
                    path = g,
                    exists = false
                }).ToArray();

                foreach (FileExists file in filesThatExist)
                {
                    foreach (string exludedPath in excludedFiles)
                    {
                        if ((file.path.FixOSPath().Contains(exludedPath.FixOSPath())))
                        {
                            file.exists = true;
                        }
                    }
                }

                var aiItems = ai2unity.Elements();

                foreach (var aiItem in aiItems)
                {
                    CleanAIXMLItem(filesThatExist, aiItem, null, exportPath);
                }

                bool alertBackup = alert;

                foreach (FileExists file in filesThatExist)
                {
                    bool delete = false;

                    if (file.exists)
                    {
                        //Debug.Log ("EXISTS : " + file.path);
                    }
                    else
                    {
                        //Debug.Log ("FILE DELETED : " + file.path);

                        if (alert)
                        {
                            switch (EditorUtilities.DisplayDialogComplex("Delete", "Delete file? : " + file.path, "Yes", "No", "Yes - don't alert!"))
                            {
                                case 0:
                                    delete = true;
                                    break;
                                case 1:

                                    break;
                                case 2:
                                    alert = false;
                                    delete = true;
                                    break;
                            }
                        }
                        else
                        {
                            delete = true;
                        }

                        if (delete)
                        {                            
                            if (file.path.IsInProjectAssetsFolder())
                            {
                                string meta = file.path + ".meta";
                                if (File.Exists(meta)) File.Delete(meta);
                                else Debug.LogError("Trying to delete meta file that does not exits : " + meta);
                            }
                            File.Delete(file.path);
                            
                            if (debug) Debug.Log("File deleted : " + file.path);                            
                        }
                    }
                }

                exportPath.DeleteEmptyDirectories(alertBackup, debug);

                if (exportPath.IsInProjectAssetsFolder()) EditorUtilities.AssetDatabase.Refresh();
            }

            static void CleanAIXMLItem(FileExists[] filesThatExist, XElement aiItem, XElement aiItemParent, string exportPath)
            {

                string tag = aiItem.Attribute("tag").Value;

                if (tag.Contains("include") || HasChildInclude(aiItem))
                {
                    if (tag.Contains("include"))
                    {
                        string itemName = aiItem.Attribute("itemName").Value;
                        string itemPath = aiItem.Attribute("itemPath").Value.FixOSPath();

                        if (itemPath.Length > 0 && itemPath.Substring(0, 1) == "/".FixOSPath())
                        {
                            itemPath = itemPath.Substring(1);
                        }

                        string f = Path.Combine(exportPath.FixOSPath(), itemPath).FixOSPath();
                        f = Path.Combine(f, itemName).FixOSPath();

                        foreach (FileExists file in filesThatExist)
                        {
                            if (file.path.Contains(f + ".png") || file.path.Contains(f + ".jpg") || file.path.Contains(f + ".svg") || file.path.Contains(f + ".ai"))
                            {
                                file.exists = true;
                            }
                        }
                    }

                    var aiItems = aiItem.Elements();

                    foreach (var item in aiItems)
                    {
                        CleanAIXMLItem(filesThatExist, item, aiItem, exportPath);
                    }
                }
            }            
        }
    }
}
