using Notero.Unity.MidiNoteInfo;
using Notero.Unity.UI.VirtualPiano;
using Notero.Raindrop;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace BU.NineTails.MidiGameplay.Scripts.Gameplay
{
    public class RaindropNoteSpawner : MonoBehaviour
    {
        [SerializeField] protected RaindropNote m_WhiteLeftSeed;
        [SerializeField] protected RaindropNote m_WhiteRightSeed;
        [SerializeField] protected RaindropNote m_BlackLeftSeed;
        [SerializeField] protected RaindropNote m_BlackRightSeed;
        [SerializeField] protected RectTransform m_WhiteRaindropContainer;
        [SerializeField] protected RectTransform m_BlackRaindropContainer;
        
        //Best UpdateSpawnNotes
        [SerializeField] protected RaindropNote noteC;
        [SerializeField] protected RaindropNote noteD;
        [SerializeField] protected RaindropNote noteE;
        [SerializeField] protected RaindropNote noteF;
        [SerializeField] protected RaindropNote noteG;
        [SerializeField] protected RaindropNote noteA;
        [SerializeField] protected RaindropNote noteB;

        public List<float> LanePositionList { get; protected set; }
        public float PianoFitWidth { get; protected set; }
        public virtual Vector3 Position => ((RectTransform)transform).position;

        protected int m_MinimumKeyGiven;
        protected float m_RaindropSpeed;
        protected float m_WhiteKeySize;
        protected float m_BlackKeySize;

        public virtual void Init(int octaveInputAmount, int minimumKeyGiven, float speed, float spawnPosition)
        {
            m_RaindropSpeed = speed;
            float containerWidth = ((RectTransform)transform).rect.width;
            m_WhiteKeySize = ((RectTransform)m_WhiteLeftSeed.transform).rect.width;
            m_BlackKeySize = ((RectTransform)m_BlackLeftSeed.transform).rect.width;
            m_MinimumKeyGiven = minimumKeyGiven;
            LanePositionList = VirtualPianoHelper.GetLanePosition(containerWidth, m_WhiteKeySize, m_BlackKeySize, octaveInputAmount);
            PianoFitWidth = LanePositionList.Last() - LanePositionList.First() + m_WhiteKeySize;
            SetSpawnerPosition(spawnPosition);
        }

        public virtual RaindropNote Create(MidiNoteInfo info)
        {
            int notePosIndex = info.MidiId - m_MinimumKeyGiven;
            float xPos = this.PianoFitWidth;
            float yPos = LanePositionList[notePosIndex];
            RaindropNote raindropNote = PoolNewRaindropNote(info, new Vector2(xPos, yPos));
            return raindropNote;
        }

        public virtual void SetRaindropSpeed(float speed) => m_RaindropSpeed = speed;

        protected virtual void SetSpawnerPosition(float yPos)
        {
            float xPos = 0; // have to set 0 because horizontal raindrop
            RectTransform rect = (RectTransform)transform;
            rect.anchoredPosition = new Vector2(xPos, yPos);
        }

        // Best UpdateSpawnNotes 
        protected RaindropNote GetRaindropNoteType(MidiNoteInfo info)
        {
            if (info.MidiId == 24 || info.MidiId == 36 || info.MidiId == 48 || info.MidiId == 60 || info.MidiId == 72)
            {
                return noteC;
            }
            if (info.MidiId == 26 || info.MidiId == 38 || info.MidiId == 50 || info.MidiId == 62 || info.MidiId == 74)
            {
                return noteD;
            }
            if (info.MidiId == 28 || info.MidiId == 40 || info.MidiId == 52 || info.MidiId == 64 || info.MidiId == 76)
            {
                return noteE;
            }
            if (info.MidiId == 29 || info.MidiId == 41 || info.MidiId == 53 || info.MidiId == 65 || info.MidiId == 77)
            {
                return noteF;
            }
            if (info.MidiId == 31 || info.MidiId == 43 || info.MidiId == 55 || info.MidiId == 67 || info.MidiId == 79)
            {
                return noteG;
            }
            if (info.MidiId == 33 || info.MidiId == 45 || info.MidiId == 57 || info.MidiId == 69 || info.MidiId == 79)
            {
                return noteA;
            }
            else
            {
                return noteB;
            }
            
            /*if (info.TrackIndex == (int)Handside.Left)
            {
                return VirtualPianoHelper.IsBlackKey(info.MidiId) ? m_BlackLeftSeed : m_WhiteLeftSeed;
            }
            else
            {
                return VirtualPianoHelper.IsBlackKey(info.MidiId) ? m_BlackRightSeed : m_WhiteRightSeed;
            }*/
        }

        protected RaindropNote PoolNewRaindropNote(MidiNoteInfo info, Vector2 pos)
        {
            var container = VirtualPianoHelper.IsBlackKey(info.MidiId) ? m_BlackRaindropContainer : m_WhiteRaindropContainer;
            RaindropNote raindropGo = GetRaindropNoteType(info).Rent(container);
            raindropGo.SetMidiInfo(info);
            raindropGo.name = VirtualPianoHelper.GetNoteName(info.MidiId);
            RectTransform rect = (RectTransform)raindropGo.transform;
            rect.localScale = Vector3.one;
            rect.anchoredPosition = pos;
            return raindropGo;
        }
        protected void ReturnRaindropNoteToPool(RaindropNote note)
        {
            Destroy(note.gameObject);
        }

        /*void Update()
        {
            foreach (var note in GetComponentsInChildren<RaindropNote>())
            {
                RectTransform rect = (RectTransform)note.transform;
                rect.anchoredPosition += new Vector2(-m_RaindropSpeed * Time.deltaTime, 0);

                if (rect.anchoredPosition.x < -1280)
                {
                    ReturnRaindropNoteToPool(note);
                }
            }
        }*/
    }
}
