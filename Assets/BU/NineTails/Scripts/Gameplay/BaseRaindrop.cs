using Notero.Raindrop;
using Notero.Raindrop.UI;
using Notero.Unity.MidiNoteInfo;
using Notero.Unity.UI.VirtualPiano;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BU.NineTails.MidiGameplay.Scripts.Gameplay
{
    public class BaseRaindrop : MonoBehaviour
    {
        [SerializeField]
        protected RaindropNoteSpawner m_RaindropNoteSpawner;

        [SerializeField]
        protected BarlineSpawner m_BarlineSpawner;

        /*[SerializeField]
        protected RaindropBarOverlay m_RaindropBarOverlay;*/

        public RaindropNoteSpawner RaindropNoteSpawner => m_RaindropNoteSpawner;
        public int OctaveInputAmount { get; private set; }
        public int MinimumKeyId { get; private set; }
        public float RaindropScrollSpeed { get; private set; }
        public List<RaindropBarline> RaindropBarlineList => m_BarlineSpawner.GetBarlineList();
        public List<float> LanePosList => m_RaindropNoteSpawner.LanePositionList;

        protected Dictionary<int, RaindropNote> m_RaindropNoteStorage = new();

        protected readonly string[] m_NotesName = new string[] { "C", "D", "E", "F", "G", "A", "B" };
        protected readonly string m_SharpUnicode = "\u266F";
        protected readonly string m_FlatUnicode = "\u266D";

        protected bool m_IsHold;
        protected string m_FlatSign;
        protected string m_SharpSign;
        protected bool m_IsNoteNameShow;

        public virtual void Setup(int octaveInputAmount, int minimumKeyGiven, float speed)
        {
            OctaveInputAmount = octaveInputAmount;
            MinimumKeyId = minimumKeyGiven;
            RaindropScrollSpeed = speed;
        }

        public virtual void Init(float spawnPointPosX)
        {
            spawnPointPosX = 0; //Have to set 0 because Horizontal Raindrop not working. and setting not save at all.
            m_RaindropNoteSpawner.Init(OctaveInputAmount, MinimumKeyId, RaindropScrollSpeed, spawnPointPosX);
            //m_RaindropBarOverlay.Init(m_RaindropNoteSpawner.PianoFitWidth, OctaveInputAmount, m_RaindropNoteSpawner.LanePositionList);
            m_RaindropNoteStorage.Clear();

            m_FlatSign = System.Net.WebUtility.HtmlDecode(m_FlatUnicode);
            m_SharpSign = System.Net.WebUtility.HtmlDecode(m_SharpUnicode);
        }

        public virtual void RemoveActionCue(int raindropNoteId)
        {
            m_RaindropNoteStorage[raindropNoteId].Remove();
            m_RaindropNoteStorage.Remove(raindropNoteId);
        }

        /// <summary>
        /// Check if raindrop is still show on game panel
        /// </summary>
        /// <param name="raindropNoteId"></param>
        /// <returns></returns>
        public bool IsCueShowing(int raindropNoteId)
        {
            return m_RaindropNoteStorage.ContainsKey(raindropNoteId);
        }

        public void ResetCues()
        {
            foreach (RaindropNote raindropNoteId in m_RaindropNoteStorage.Values)
            {
                raindropNoteId.Remove();
            }
            m_RaindropNoteStorage.Clear();
        }

        public void SetDefault(int raindropNoteId)
        {
            m_RaindropNoteStorage[raindropNoteId].SetDefault();
        }

        public void SetCorrect(int raindropNoteId)
        {
            m_RaindropNoteStorage[raindropNoteId].SetCorrect();
        }

        public void SetMiss(int raindropNoteId)
        {
            m_RaindropNoteStorage[raindropNoteId].SetMiss();
        }

        //TODO: remove if hold on mode is confirm visual
        public void SetHide(int raindropNoteId) => m_RaindropNoteStorage[raindropNoteId].SetHide();

        /// <summary>
        /// Set to pause or resume raindrop note
        /// </summary>
        /// <param name="value"></param>
        public void SetHold(bool value) => m_IsHold = value;

        public virtual void SetRaindropScrollSpeed(float speed)
        {
            RaindropScrollSpeed = speed;
            m_RaindropNoteSpawner.SetRaindropSpeed(speed);
        }

        protected bool HasRaindropNote() => m_RaindropNoteStorage?.Count > 0;

        //protected bool HasRaindropBarline() => m_BarlineSpawner.GetBarlineList()?.Count > 0;

        public virtual void BarlineSetup(float bpm, float timeSignatureTop, float timeSiganatureBottom, float songDuration)
        {
            if (m_BarlineSpawner == null) return;

            var barPeriod = m_BarlineSpawner.GetBarLinePeriod(bpm, timeSignatureTop, timeSiganatureBottom);
            var barCount = m_BarlineSpawner.GetBarLineCount(songDuration, barPeriod);
            m_BarlineSpawner.SpawnBarlines(barCount);
            m_BarlineSpawner.SetBarLineEndTime(RaindropScrollSpeed, barPeriod);
        }

        /*protected void OnShowNoteNameSet(bool isActive)
        {
            m_IsNoteNameShow = isActive;

            foreach (var note in m_RaindropNoteStorage.Values)
            {
                GetKeySignature(note, note.MidiNoteInfo);
            }
        }*/

        //public void SetBarOverlayActive(bool isActive) => m_RaindropBarOverlay.SetActive(isActive);

        /*private void CheckAccidentalText(string noteName, RaindropNote note, MidiNoteInfo info)
        {
            var accidentalNumber = (sbyte)info.AccidentalNumber;

            if (VirtualPianoHelper.GetNoteName(info.MidiId).Contains("#"))
            {
                if (accidentalNumber < 0)
                {
                    var noteIndex = Array.IndexOf(m_NotesName, noteName);

                    note.SetNoteNameText(m_NotesName[noteIndex + 1]);
                    note.SetActiveAccidentalText(true);
                    note.SetAccidentalText(m_FlatSign);
                }
                else
                {
                    note.SetActiveAccidentalText(true);
                    note.SetAccidentalText(m_SharpSign);
                }
            }
            else
            {
                note.SetActiveAccidentalText(false);
            }
        }*/

        /*public void GetKeySignature(RaindropNote note, MidiNoteInfo info)
        {
            var noteName = VirtualPianoHelper.GetNoteName(info.MidiId).Substring(0, 1);
            var isActive = m_IsNoteNameShow;

            note.SetActiveKeySignatureText(isActive);

            if (!isActive) return;

            note.SetNoteNameText(noteName);
            CheckAccidentalText(noteName, note, info);
        }*/

        public virtual void CreateActionCue(MidiNoteInfo info)
        {
            RaindropScrollSpeed = 150;
            RaindropNote note = m_RaindropNoteSpawner.Create(info);
            note.Init(RaindropScrollSpeed, (float)info.NoteOnTime / 1000);
            //GetKeySignature(note, info);
            note.SetDefault();
            m_RaindropNoteStorage.Add(info.RaindropNoteId, note);
        }

        public virtual void UpdateLogic(float currentTime)
        {
            if (m_IsHold) return;

            if (HasRaindropNote())
            {
                foreach (RaindropNote note in m_RaindropNoteStorage.Values)
                {
                    note.UpdatePosition(currentTime);
                }
            }

            /*if (HasRaindropBarline())
            {
                foreach (RaindropBarline barline in m_BarlineSpawner.GetBarlineList())
                {
                    if (barline.isActiveAndEnabled)
                    {
                        barline.UpdatePosition(currentTime);
                    }
                }
            }
        }*/
        }

        public enum KeySignatureEnum
        {
            CFlatMaj = -7,
            GFlatMaj_EFlatMin = -6,
            DFlatMaj_BFlatMin = -5,
            AFlatMaj_FMin = -4,
            EFlatMaj_CMin = -3,
            BFlatMaj_GMin = -2,
            FMaj_DMin = -1,
            CMaj_AMin = 0,
            GMaj_EMin = 1,
            DMaj_BMin = 2,
            AMaj_FSharpMin = 3,
            EMaj_CSharpMin = 4,
            BMaj_GSharpMin = 5,
            FSharpMaj_DSharpMin = 6,
            CSharpMaj = 7
        }

        public enum ScaleEnum
        {
            Major = 0,
            Minor = 1
        }

        public enum RaindropState //state of each press key
        {
            Default,
            Correct,
            Miss
        }

    }
}
