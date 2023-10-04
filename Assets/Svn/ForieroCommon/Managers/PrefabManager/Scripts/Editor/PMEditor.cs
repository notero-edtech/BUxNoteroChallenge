using ForieroEditor;
using UnityEditor;

public static class PMEditor
{
    [MenuItem("Foriero/Assets/Banks/Create Prefabs Bank", false, 50)] public static void CreatePMBank() => ScriptableObjects.CreateScriptableObject<PMBank>();    
}
