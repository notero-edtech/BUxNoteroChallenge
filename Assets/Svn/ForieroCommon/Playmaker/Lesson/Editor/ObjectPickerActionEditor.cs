using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMakerEditor;
using UnityEngine;

[CustomActionEditor(typeof(ObjectPickerAction))]
public class ObjectPickerActionEditor : CustomActionEditor
{
    ObjectPickerAction action;

    public override void OnEnable()
    {
        action = target as ObjectPickerAction;
    }

    public override bool OnGUI()
    {
        bool isDirty = false;

        EditField("correctGameObjects");
        EditField("correctUnique");
        if (action.correctUnique.Value)
            EditField("correctGameObjectsToPick");
        EditField("pickedGameObject");
        EditField("event_correct");
        EditField("event_incorrect");
        if (action.correctUnique.Value)
            EditField("event_null");
        return isDirty || GUI.changed;
    }
}
