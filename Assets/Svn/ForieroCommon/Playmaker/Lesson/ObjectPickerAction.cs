using System;
using System.Collections.Generic;
using UnityEngine;
using HedgehogTeam.EasyTouch;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Question Actions")]
    [Tooltip("Selects a GameObject and fires a CORRECT or INCORRECT event depending on if the GameObject is in the " +
        "'correctGameObjects' Array")]
    public class ObjectPickerAction : FsmStateAction
    {
        [ArrayEditor(VariableType.GameObject)]
        public FsmArray correctGameObjects;
        [Tooltip("If TRUE any gameObject in the correct list can only be correct once")]
        public FsmBool correctUnique;
        [ArrayEditor(VariableType.GameObject),
            Tooltip("if the picked gameObject is in this list then it's not correct anymore")]
        public FsmArray correctGameObjectsToPick;
        [Tooltip("Variable the store the picked GameObject")]
        public FsmGameObject pickedGameObject;

        public FsmEvent event_correct;
        public FsmEvent event_incorrect;
        public FsmEvent event_null;

        EasyTouch easyTouch;

        public override void OnEnter()
        {
            easyTouch = GameObject.FindObjectOfType<EasyTouch>();
            EasyTouch.On_TouchStart += OnTouch;
        }

        void OnTouch(Gesture gesture)
        {
            //Debug.Log("Touch");
            GameObject pickedObject = gesture.pickedObject;
            if (!pickedObject)
                return;
            pickedGameObject.Value = pickedObject;
            Debug.Log(pickedObject.name);

            List<GameObject> correctGOs = new List<GameObject>();
            for (int i = 0; i < correctGameObjects.Length; i++)
            {
                correctGOs.Add(correctGameObjects.Values[i] as GameObject);
            }

            if (correctGOs.Contains(pickedObject))
            {
                if (correctUnique.Value)
                {
                    List<GameObject> correctGOsToPick = new List<GameObject>();
                    for (int i = 0; i < correctGameObjectsToPick.Length; i++)
                    {
                        correctGOsToPick.Add(correctGameObjectsToPick.Values[i] as GameObject);
                    }

                    if (correctGOsToPick.Contains(pickedObject))
                    {
                        // Remove the GameObject from the list of correct objects to pick
                        correctGOsToPick.Remove(pickedObject);
                        correctGameObjectsToPick.Resize(correctGOsToPick.Count);
                        for (int i = 0; i < correctGOsToPick.Count; i++)
                        {
                            correctGameObjectsToPick.Set(i, correctGOsToPick[i]);
                        }
                        //Debug.Log("picker, legth: " + correctGameObjectsToPick.Length);
                        if (event_correct != null) Fsm.Event(event_correct);
                    }
                    else
                    {
                        if (event_null != null) Fsm.Event(event_null);
                    }
                }
                else
                {
                    if (event_correct != null) Fsm.Event(event_correct);
                }
            }
            else
            {
                if (event_incorrect != null) Fsm.Event(event_incorrect);
            }

            Finish();
        }

        public override void OnExit()
        {
            EasyTouch.On_TouchStart -= OnTouch;
        }
    }
}
