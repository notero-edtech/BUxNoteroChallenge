using System.Collections.Generic;
using UnityEngine;

namespace Notero.Unity.AudioModule.Core
{
    public class AudioSpeakerCollection : MonoBehaviour
    {
        public int Count => m_WaitingQueue.Count;

        private int m_InitialAmount = 5;
        private Queue<AudioSpeaker> m_WaitingQueue = new Queue<AudioSpeaker>();

        public void Init()
        {
            for(int i = 0 ; i < m_InitialAmount ; i++)
            {
                CreateAudioSpeaker();
            }
        }

        public AudioSpeaker GetAudioSpeaker()
        {
            if(m_WaitingQueue.Count == 0)
            {
                CreateAudioSpeaker();
            }

            return m_WaitingQueue.Dequeue();
        }

        public void ReturnAudioSpeaker(AudioSpeaker audioSpeaker)
        {
            if(audioSpeaker == null)
                return;

            audioSpeaker.transform.SetAsLastSibling();
            m_WaitingQueue.Enqueue(audioSpeaker);
        }

        private void CreateAudioSpeaker()
        {
            var audioSpeaker = new GameObject("AudioSpeaker", typeof(AudioSpeaker)).GetComponent<AudioSpeaker>();
            audioSpeaker.transform.SetParent(transform);
            audioSpeaker.Init();
            m_WaitingQueue.Enqueue(audioSpeaker);
        }
    }
}
