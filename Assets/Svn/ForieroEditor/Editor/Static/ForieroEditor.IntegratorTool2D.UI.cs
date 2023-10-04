using System.Collections;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ForieroEditor.Coroutines;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ForieroEditor.Extensions;
using ForieroEditor.Utilities;

namespace ForieroEditor
{
    public static partial class IntegratorTool2DEditor
    {

        public static partial class RecreateScene
        {
            static Canvas canvas;
            static CanvasScaler canvasScaler;

            public static void GenerateHierarchyUI(string importUnityXml, string name, ImageType imageType = ImageType.png)
            {
                if (!importUnityXml.IsItAssetPath())
                {
                    Debug.LogError("IT2D | Source images were exported outside unity project!!! : " + importUnityXml);
                    return;
                }
                
                TextAsset f = AssetDatabase.LoadAssetAtPath(importUnityXml, typeof(TextAsset)) as TextAsset;

                RecreateScene.assetXMLFilePath = importUnityXml.FixAssetsPath().Replace("imports/import_unity.xml", "");
                RecreateScene.imageType = imageType;

                if (!f)
                {
                    Debug.LogError("FILE NOT FOUND : " + importUnityXml);
                    return;
                }

                try
                {
                    XDocument doc = XDocument.Load(ForieroEditor.Extensions.ForieroEditorExtensions.GetMemoryStream(f.text));

                    var ai2unity = doc.Element("AI2UNITY");

                    GameObject goRoot = new GameObject(Path.GetFileNameWithoutExtension(ai2unity.Attribute("fileName").Value));

                    canvas = goRoot.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvasScaler = goRoot.AddComponent<CanvasScaler>();
                    canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
                    
                    goRoot.AddComponent<GraphicRaycaster>();

                    scaleFactor = float.Parse(ai2unity.Attribute("scaleFactor").Value);

                    illustratorDesigResolution = new Vector2(
                        float.Parse(ai2unity.Attribute("illustratorDesignResolutionWidth").Value),
                        float.Parse(ai2unity.Attribute("illustratorDesignResolutionHeight").Value)
                    );

                    unityDesignResolution = new Vector2(
                        float.Parse(ai2unity.Attribute("unityDesignResolutionWidth").Value),
                        float.Parse(ai2unity.Attribute("unityDesignResolutionHeight").Value)
                    );
                    
                    var o = GameViewUtility.SetCustomSize((int)illustratorDesigResolution.x, (int)illustratorDesigResolution.y);
                    GameViewUtility.SelectSize(o);
                    
                    canvasScaler.referenceResolution = illustratorDesigResolution;
                    
                    Canvas.ForceUpdateCanvases();
                    
                    scaleFactor *= canvasScaler.referenceResolution.x / unityDesignResolution.x;
                    
                    var aiItems = ai2unity.Elements();
                    aiItems = aiItems.Reverse();
                    
                    foreach (var aiItem in aiItems) GenerateAIItemUI(aiItem, goRoot.transform, null);
                }
                catch (System.Exception e) { Debug.LogError("Invalid XML: " + importUnityXml + " " + e.Message); }
                finally { }
            }

            static void GenerateAIItemUI(XElement aiItem, Transform parent, XElement aiItemParent)
            {
                string name = aiItem.Attribute("itemName").Value;
                string tag = aiItem.Attribute("tag").Value;

                if (tag.Contains("include") || HasChildInclude(aiItem))
                {
                    GameObject goItem = new GameObject(name);
                    goItem.transform.SetParent(canvas.transform, true);

                    RectTransform rt = goItem.AddComponent<RectTransform>();
                    rt.anchoredPosition3D = Vector3.zero;
                    rt.SetSize(Vector2.zero);

                    if (tag.Contains("include"))
                    {
                        RectTransform imageRT = null;
                        Sprite imageSprite = null;
                        switch (imageType)
                        {
                            case ImageType.png:
                                Image image = goItem.AddComponent<Image>();
                                imageRT = image.rectTransform;
                                imageSprite = image.sprite = GetAIPathSprite(aiItem);
                                if (image.sprite.border != Vector4.zero) image.type = Image.Type.Sliced;
                                image.SetNativeSize();
                                break;
                            case ImageType.svg:
#if UNITY_2018_1_OR_NEWER && VECTOR_GRAPHICS
                                SVGImage svgImage = goItem.AddComponent<SVGImage>();
                                imageRT = svgImage.rectTransform;
                                imageSprite = svgImage.sprite = GetAIPathSprite(aiItem);
                                //if (svgImage.sprite.border != Vector4.zero) svgImage.type = Image.Type.Sliced;
                                //svgImage.SetNativeSize();
                                var min = imageSprite.bounds.min;
                                var max = imageSprite.bounds.max;
                                var rect = new Rect(min, max - min);
                                imageRT.anchorMax = imageRT.anchorMin;
                                imageRT.sizeDelta = new Vector2(rect.width, rect.height);
#else
                                Debug.LogError("Please import VectorGraphics package from PackageManager!!!");
#endif
                                break;
                        }

                        string p = GetResourceFilePath(aiItem, Path.GetDirectoryName(assetXMLFilePath)).FixAssetsPath();
                        float pixelsPerUnit = (AssetImporter.GetAtPath(p) as TextureImporter).spritePixelsPerUnit;
                        
                        float imageScaleFactor = scaleFactor / (canvasScaler.referencePixelsPerUnit / pixelsPerUnit);
                        imageRT.SetSize(imageRT.GetSize() * imageScaleFactor);

                        SetAIItemPositionUI(aiItem, rt);
                    }


                    goItem.transform.SetParent(parent);

                    var aiItems = aiItem.Elements();

                    aiItems = aiItems.Reverse();

                    foreach (var item in aiItems) { GenerateAIItemUI(item, goItem.transform, aiItem); }
                }
            }

            static void SetAIItemPositionUI(XElement aiItem, RectTransform rectTransform)
            {
                float x = float.Parse(aiItem.Attribute("x").Value) * scaleFactor;
                float y = float.Parse(aiItem.Attribute("y").Value) * scaleFactor;
                
                GameViewUtility.GetGameRenderSize(out var renderWidth, out var renderHeight);
                var renderFactor = (float)renderWidth / (float)canvasScaler.referenceResolution.x;
                
                rectTransform.anchoredPosition = new Vector2(
                    -renderWidth / renderFactor / 2f + x / renderFactor,
                    -renderHeight / renderFactor / 2f + y / renderFactor
                    );
            }
        }
    }
}

