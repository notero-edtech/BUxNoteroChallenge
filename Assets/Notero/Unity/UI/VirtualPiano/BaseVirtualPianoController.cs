using System;
using System.Collections.Generic;
using UnityEngine;

namespace Notero.Unity.UI.VirtualPiano
{
    public class BaseVirtualPianoController : MonoBehaviour
    {
        [SerializeField]
        private VirtualPianoSpawner m_VirtualPianoSpawner;

        [SerializeField]
        private RectTransform m_NoteIndicator;

        private List<PianoKey> m_KeyStorage;

        private string m_currentPianoType;

        public Rect WhiteKeyRect => m_VirtualPianoSpawner.WhiteKeyRect;

        public Rect BlackKeyRect => m_VirtualPianoSpawner.BlackKeyRect;

        public float IndicatorPosY => m_NoteIndicator.anchoredPosition.y;

        public static event Action<bool> ShowLabel;

        bool isShowLabel;

        public void SetActiveNoteIdicator(bool isNoteIndicatorActive)
        {
            m_NoteIndicator.gameObject.SetActive(isNoteIndicatorActive);
        }

        public void Create(string pianoType)
        {
            m_currentPianoType = pianoType;
            m_KeyStorage = m_VirtualPianoSpawner.Create(pianoType);
            SetPianoNoteLabel(isShowLabel);
            SetActiveNoteIdicator(true);
        }

        public void SetPianoNoteLabel(bool show)
        {
            isShowLabel = show;
            var showLabel = m_currentPianoType == "gameplay" && isShowLabel;
            ShowLabel?.Invoke(showLabel);
        }

        public void DeleteAllPianoKeys()
        {
            if(m_KeyStorage == null || m_KeyStorage.Count == 0)
            {
                Debug.Log($"Cannot find {nameof(m_KeyStorage)}");
                return;
            }

            foreach(PianoKey key in m_KeyStorage)
            {
                if(Application.isPlaying)
                {
                    Destroy(key.gameObject);
                }
                else
                {
                    DestroyImmediate(key.gameObject);
                    continue;
                }
            }

            m_KeyStorage.Clear();
            SetActiveNoteIdicator(false);
        }

        public void SetDefault(int midiId, bool isPressing)
        {
            SetSprite(midiId, "0", Handside.Left, isPressing);
        }

        public void SetQuiz(int midiId, bool isPressing)
        {
            SetSprite(midiId, "3", Handside.Left, isPressing);
        }

        public void SetRaindropIn(int midiId, Handside handSide, bool isPressing)
        {
            SetSprite(midiId, "1", handSide, isPressing);
        }

        public void SetMissKey(int midiId, bool isPressing)
        {
            SetSprite(midiId, "2", Handside.Left, isPressing);
        }

        public void ResetPiano()
        {
            foreach(PianoKey key in m_KeyStorage)
            {
                key.SetSprite("0", Handside.Left, false);
            }

            m_VirtualPianoSpawner.ResetKeys();
        }

        private void SetSprite(int midiId, string state, Handside handside, bool isPressing)
        {
            if(m_VirtualPianoSpawner.IsMidiIdInRange(midiId))
            {
                int index = GetPianoKeyIndex(midiId);
                m_KeyStorage[index].SetSprite(state, handside, isPressing);
            }
            else
            {
                Debug.LogError($"The midiID {midiId} is out of range. it must be between {m_VirtualPianoSpawner.MinimumKey} to {m_VirtualPianoSpawner.MaximumKey}");
            }
        }

        public bool IsMidiIdInRange(int midiId) => m_VirtualPianoSpawner.IsMidiIdInRange(midiId);

        public int GetPianoKeyIndex(int midiId) => midiId - m_VirtualPianoSpawner.MinimumKey;
    }
}