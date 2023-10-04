using UnityEditor;
using UnityEngine;

namespace ForieroEditor
{
    public static class ScriptableObjects
    {
        public static void CreateScriptableObject<T>() where T : ScriptableObject
        {
            T scriptable_object = ScriptableObject.CreateInstance<T>();

            string path = Instances.GetSelectedPathOrFallback();

            string assetPath = path + "/New " + typeof(T).Name + ".asset";
            
            string uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

            AssetDatabase.CreateAsset(scriptable_object, uniqueAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = scriptable_object;
        }
    }

    public static class UnityObjects
    {
        public static void SaveObject<T>(this T t, string fileName, string assetPath = "") where T : Object
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                string path = Instances.GetSelectedPathOrFallback();
                assetPath = path + "/" + fileName;
            }
                        
            string uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
                                    
            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(t), uniqueAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<T>(uniqueAssetPath);
        } 
    }
}
