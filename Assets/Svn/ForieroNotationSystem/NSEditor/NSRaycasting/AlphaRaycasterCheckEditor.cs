/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;
using TMPro;
using ForieroEngine.Music.NotationSystem.Classes;
using ForieroEngine.Music.NotationSystem.Raycasting;

[CustomEditor(typeof(AlphaRaycasterCheck)), CanEditMultipleObjects]
public class AlphaRaycasterCheckEditor : Editor
{
    private SerializedProperty alphaThreshold;
    private SerializedProperty includeMaterialAlpha;
    private SerializedProperty outerEdgeDetection;

    AlphaRaycasterCheck o;

    private void OnEnable()
    {
        alphaThreshold = serializedObject.FindProperty("alphaThreshold");
        includeMaterialAlpha = serializedObject.FindProperty("includeMaterialAlpha");
        outerEdgeDetection = serializedObject.FindProperty("outerEdgeDetection");
    }

    public override void OnInspectorGUI()
    {
        o = target as AlphaRaycasterCheck;

        serializedObject.Update();
        EditorGUILayout.PropertyField(alphaThreshold);
        EditorGUILayout.PropertyField(includeMaterialAlpha);
        EditorGUILayout.PropertyField(outerEdgeDetection);
        serializedObject.ApplyModifiedProperties();

        if (o)
        {
            var image = o.GetComponent<Image>();
            if (image)
            {
                var path = AssetDatabase.GetAssetPath(image.mainTexture);
                if (path != string.Empty && !image.sprite.packed)
                {
                    var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (!textureImporter)
                    {
                        EditorGUILayout.HelpBox("Assign a custom source image to the Image component to configure alpha checking.\nBuilt-in Unity images are not supported.", MessageType.Warning);
                        return;
                    }

                    if (!textureImporter.isReadable)
                    {
                        EditorGUILayout.HelpBox("The texture is not readable. Alpha check won't have effect.", MessageType.Warning);
                        if (GUILayout.Button("FIX"))
                        {
                            textureImporter.isReadable = true;
                            AssetDatabase.ImportAsset(path);
                        }
                        return;
                    }
                }
                else if (!image.sprite.packed)
                {
                    EditorGUILayout.HelpBox("Assign a source image to the Image component to configure alpha checking.", MessageType.Warning);
                    return;
                }

                var blockingChilds = o.GetComponentsInChildren<CanvasRenderer>(false)
                    .Where(child => child.gameObject != o && (!child.GetComponent<CanvasGroup>() || child.GetComponent<CanvasGroup>().blocksRaycasts)).ToList();
                if (blockingChilds.Count > 0)
                {
                    EditorGUILayout.HelpBox("Some of the child objects may be blocking the raycast.", MessageType.Warning);
                    if (GUILayout.Button("FIX"))
                    {
                        foreach (var blockingChild in blockingChilds)
                        {
                            var canvasGroup = blockingChild.GetComponent<CanvasGroup>() ? blockingChild.GetComponent<CanvasGroup>() : blockingChild.gameObject.AddComponent<CanvasGroup>();
                            canvasGroup.blocksRaycasts = false;
                        }
                    }
                }
            }
            else if (o.GetComponent<Text>())
            {
                var text = o.GetComponent<Text>();
                var path = AssetDatabase.GetAssetPath(text.mainTexture);
                if (path != string.Empty)
                {
                    var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                    if ((!textureImporter || !textureImporter.isReadable) && !text.font.dynamic)
                    {
                        EditorGUILayout.HelpBox("The font texture is not readable. Alpha check won't have effect.\nConsult the documentation on how to prepare fonts to use with Alpha Raycaster.", MessageType.Warning);
                        return;
                    }
                }

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("NATIVE RECT"))
                {
                    text.rectTransform.SetSize(new Vector2(text.preferredWidth, text.preferredHeight));
                }

                if (GUILayout.Button("CLOSEST RECT"))
                {
                    text.SetBestFitRect();
                }

                GUILayout.EndHorizontal();
            }
            else if (o.GetComponent<TextMeshProUGUI>())
            {
                var text = o.GetComponent<TextMeshProUGUI>();

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("NATIVE RECT"))
                {
                    text.rectTransform.SetSize(new Vector2(text.preferredWidth, text.preferredHeight));
                }

                if (GUILayout.Button("CLOSEST RECT"))
                {
                    text.SetBestFitRect();
                }

                GUILayout.EndHorizontal();
                return;
            }
            else if (o.GetComponent<UIVector>())
            {
                return;
            }
            else
            {
                EditorGUILayout.HelpBox("Can't find Image or Text or or TexmMeshPRO or UIVector components. Alpha check is only possible for UI objects with an Image or Text components attached.", MessageType.Error);
                return;
            }
        }


    }
}
