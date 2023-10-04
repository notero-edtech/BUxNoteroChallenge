/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("NS Add")]
    [Tooltip("")]
    public class NSPAddNote : NSPAbstractObjectAdd
    {
        public NoteEnum noteEnum;
        public AccidentalEnum accidentalEnum;
        public StemEnum stemEnum;
        public BeamEnum beamEnum;

        public override void Reset()
        {
            base.Reset();

            noteEnum = NoteEnum.Undefined;
            accidentalEnum = AccidentalEnum.Undefined;
            stemEnum = StemEnum.Undefined;
            beamEnum = BeamEnum.Undefined;
        }

        public override void OnEnter()
        {
            if (NS.instance == null)
            {
                Finish();
                Debug.LogError("NS.instance missing!");
                return;
            }

            NSNote note = AddObject<NSNote>();

            note.options.noteEnum = noteEnum;
            note.options.accidentalEnum = accidentalEnum;
            note.options.stemEnum = stemEnum;
            note.options.beamEnum = beamEnum;

            this.CreateNSPObjectWrapper(note);
            this.Commit(note);

            Finish();
        }
    }
}
