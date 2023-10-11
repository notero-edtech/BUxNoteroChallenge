using Notero.Unity.UI.VirtualPiano;
using Notero.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Notero.MidiAdapter;
using UnityEngine;
using UnityEngine.UI;

namespace Notero.Unity.UI.Quiz
{
    public class PianoKeyQuizController : MonoBehaviour
    {
        [SerializeField]
        private PianoKeyQuizSpawner m_PianoKeyQuizSpawner;

        private BaseVirtualPianoController m_VirtualPianoController;

        private bool m_IsPressing;
        private bool m_IsChoicePressing;
        private bool m_IsChoiceConfirm;

        private readonly Dictionary<int, string> m_KeyChoices = new() { { 0, "1" }, { 4, "2" }, { 7, "3" }, { 11, "4" } };

        private HashSet<int> m_KeyIndexLedOnList = new HashSet<int>();
        private Coroutine m_WaitforStudentAnswerCoroutine;
        private const int AnswerDelay = 1; // in seconds
        private const int m_MidiIdOffset = 12;

        // Timer
        private float m_TimerValue;
        private Image m_TimerImage;

        public event Action<int, string> OnReleasePiano;

        public event Action<int, string> OnPressingPiano;

        public event Action<int, string> OnSubmitPiano;

        void OnEnable()
        {
            MidiInputAdapter.Instance.NoteOnEvent += OnKeyPressed;
            MidiInputAdapter.Instance.NoteOffEvent += OnKeyReleased;
        }

        void OnDestroy()
        {
            PianoKeyDestroy();
        }

        public void PianoKeyQuizSpawner()
        {
            m_PianoKeyQuizSpawner.ClearKeyList();
            m_VirtualPianoController = GetComponent<BaseVirtualPianoController>();
            m_VirtualPianoController.Create("quiz");
            MidiInputAdapter.Instance.SetComputerKeyboardOctave(5);

            m_TimerValue = 0;
            m_IsChoiceConfirm = false;

            foreach(var pianoKeyIndex in m_KeyChoices.Keys.Select(index => m_PianoKeyQuizSpawner.MinimumKey + index))
            {
                MidiInputAdapter.Instance.SetLedOn(pianoKeyIndex, true);
            }
        }

        public void PianoKeyDestroy()
        {
            if(MidiInputAdapter.Instance != null)
            {
                MidiInputAdapter.Instance.NoteOnEvent -= OnKeyPressed;
                MidiInputAdapter.Instance.NoteOffEvent -= OnKeyReleased;
                MidiInputAdapter.Instance.SetAllLedOff();
            }

            if(m_VirtualPianoController != null) m_VirtualPianoController.DeleteAllPianoKeys();
            if(m_PianoKeyQuizSpawner != null) m_PianoKeyQuizSpawner.ClearKeyList();
        }

        private void Update()
        {
            if(m_TimerImage != null && m_IsChoicePressing)
            {
                m_TimerValue += Time.deltaTime;
                m_TimerImage.fillAmount = m_TimerValue;
            }
        }

        private void OnKeyPressed(int midiId, int octaveIndex, int noteNumber)
        {
            if(!m_VirtualPianoController.IsMidiIdInRange(midiId)) return;
            if(m_IsChoiceConfirm) return;

            m_IsPressing = true;
            SetKeyColors(noteNumber, midiId);

            if(!m_KeyChoices.ContainsKey(noteNumber)) return;

            ResetAllTimer();
            m_TimerImage = m_PianoKeyQuizSpawner.GetTimer()[int.Parse(m_KeyChoices[noteNumber]) - 1];

            CheckStudentAnswer(noteNumber, true);
        }

        private void OnKeyReleased(int midiId, int octaveIndex, int noteNumber)
        {
            if(!m_VirtualPianoController.IsMidiIdInRange(midiId)) return;
            if(m_IsChoiceConfirm) return;

            m_IsPressing = false;
            SetKeyColors(noteNumber, midiId);

            if(!m_KeyChoices.ContainsKey(noteNumber)) return;

            m_TimerImage = m_PianoKeyQuizSpawner.GetTimer()[int.Parse(m_KeyChoices[noteNumber]) - 1];
            ResetAllTimer();

            CheckStudentAnswer(noteNumber, false);
        }

        private void SetKeyColors(int noteNumber, int midiId)
        {
            if(!m_KeyChoices.ContainsKey(noteNumber))
            {
                m_VirtualPianoController.SetDefault(midiId, m_IsPressing);
            }
            else
            {
                m_VirtualPianoController.SetQuiz(midiId, m_IsPressing);
            }
        }

        private void CheckStudentAnswer(int pianoKeyIndex, bool isPressed)
        {
            if(!m_KeyChoices.TryGetValue(pianoKeyIndex, out var choice)) return;

            m_IsChoicePressing = isPressed;
            switch(isPressed)
            {
                case false:
                    OnReleasePiano?.Invoke(pianoKeyIndex, choice);
                    break;
                case true:
                    OnPressingPiano?.Invoke(pianoKeyIndex, choice);
                    break;
            }

            if(m_WaitforStudentAnswerCoroutine != null && !isPressed)
            {
                StopCoroutine(m_WaitforStudentAnswerCoroutine);
                return;
            }

            if(m_WaitforStudentAnswerCoroutine == null)
            {
                m_WaitforStudentAnswerCoroutine = StartCoroutine(ConfirmChoice(pianoKeyIndex, choice));
            }
            else
            {
                m_WaitforStudentAnswerCoroutine = CoroutineHelper.Instance.Restart(m_WaitforStudentAnswerCoroutine, ConfirmChoice(pianoKeyIndex, choice));
            }
        }

        private IEnumerator ConfirmChoice(int pianoKeyIndex, string choice)
        {
            yield return new WaitForSeconds(AnswerDelay);
            m_IsChoiceConfirm = true;
            OnSubmitPiano?.Invoke(pianoKeyIndex, choice);
            m_PianoKeyQuizSpawner.OnConfirmPianoKeyChoice(pianoKeyIndex, choice);
            m_WaitforStudentAnswerCoroutine = null;
        }

        private void ResetAllTimer()
        {
            foreach(Image timer in m_PianoKeyQuizSpawner.GetTimer())
            {
                m_TimerValue = 0;
                timer.fillAmount = 0;
            }
        }
    }
}