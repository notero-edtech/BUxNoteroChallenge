/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEditor;
using UnityEngine;
using PathologicalGames;

// Only compile if not using Unity iPhone
[CustomEditor(typeof(NSPoolBank))]
public class NSPoolBankInspector : Editor
{
    public bool expandPrefabs = true;

    public override void OnInspectorGUI()
	{
        var script = target as NSPoolBank;

        EditorGUI.indentLevel = 0;
        PGEditorUtils.LookLikeControls();
       
        this.expandPrefabs = PGEditorUtils.SerializedObjFoldOutList<PrefabPool>
                            (
                                "Per-Prefab Pool Options", 
                                script.prefabPool,
                                this.expandPrefabs,
                                ref script._editorListItemStates,
                                true
                            );

        // Flag Unity to save the changes to to the prefab to disk
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

}


