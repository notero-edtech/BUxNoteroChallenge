using Notero.Raindrop;
using System.Collections.Generic;
using UnityEngine;

namespace BU.NineTails.MidiGameplay.Scripts.Gameplay
{
    public class BarlineSpawner : MonoBehaviour
    {
        [SerializeField]
        private RaindropBarline m_RaindropBarLine;

        [SerializeField]
        private GameObject m_RaindropBarLineParent;

        [SerializeField]
        private List<RaindropBarline> m_BarLineList = new List<RaindropBarline>();
        public List<RaindropBarline> GetBarlineList() => m_BarLineList;

        public void SpawnBarlines(int barCount)
        {
            for (int i = 0; i < barCount; i++)
            {
                var barline = Instantiate(m_RaindropBarLine, transform);
                m_BarLineList.Add(barline);
                barline.transform.SetParent(m_RaindropBarLineParent.transform);
                barline.gameObject.SetActive(false);
            }
        }

        public float GetBarLinePeriod(float BPM, float timeSignatureTopNumber, float timeSignatureBottomNumber)
        {
            var quarterNoteTimeInSecond = 60 / BPM;

            //if get time signature message 7th byte to calculate
            //if 7th byte = 0x08 >> 8 32th notes per beat, then a note-on/note-off pair with a distance of 100 ticks is displayed as a quarter note.
            //if 7th byte = 0x32 >> 32 32th notes per beat, then a note-on/note-off pair with a distance of 100 ticks is displayed as a whole note.
            //replace quarterNoteTimeSignatureBase with quarterNoteTimeSignature7thByte = 8; 
            //oneBeatTimeInSecond = quarterNoteTimeInSecond * (7th byte of time signature message/quarterNoteTimeSignature7th);
            var quarterNoteTimeSignatureBase = 4;
            var oneBeatTimeInSecond = quarterNoteTimeInSecond * (quarterNoteTimeSignatureBase / timeSignatureBottomNumber);

            return oneBeatTimeInSecond * timeSignatureTopNumber;

        }

        /// <summary>
        /// Calculate number of bar to instanciate. SongDuration in second.
        /// </summary>
        /// <param name="SongDuration"></param>
        public int GetBarLineCount(float SongDuration, float barlinePeriod)
        {
            return (int)(SongDuration / barlinePeriod);
        }

        public void SetBarLineEndTime(float speed, float barlinePeriod)
        {
            float barTime = 0;

            foreach (RaindropBarline barline in m_BarLineList)
            {
                barline.SetupBarLine(speed, barTime);
                barTime += barlinePeriod;
            }
        }
    }
}
