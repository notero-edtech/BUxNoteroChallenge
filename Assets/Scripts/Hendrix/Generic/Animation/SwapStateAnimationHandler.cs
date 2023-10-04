using System;
using UnityEngine;

namespace Hendrix.Generic.Animation
{
    public class SwapStateAnimationHandler : InOutAnimationHandler
    {
        public event Action OnAnimationAToBSwapFinished;
        public event Action OnAnimationBToASwapFinished;
        public event Action OnAnimationSwapFinished;

        public bool IsSwapPlaying;

        private readonly int m_AToBAnimationHash = Animator.StringToHash("a_to_b");
        private readonly int m_BToAAnimationHash = Animator.StringToHash("b_to_a");

        /// <summary>
        /// Trigger a to b animation
        /// </summary>
        public void PlayAToB()
        {
            IsSwapPlaying = true;
            m_Animator.SetTrigger(m_AToBAnimationHash);
        }

        /// <summary>
        /// Trigger b to a animation
        /// </summary>
        public void PlayBToA()
        {
            IsSwapPlaying = true;
            m_Animator.SetTrigger(m_BToAAnimationHash);
        }

        /// <summary>
        /// Trigger a_idle or b_idle animation
        /// </summary>
        public void SetTrigger(int hash)
        {
            IsSwapPlaying = true;
            m_Animator.SetTrigger(hash);
        }

        /// <summary>
        /// This method be called by a_to_b animation event
        /// </summary>
        public void AnimationAToBSwapFinished()
        {
            IsSwapPlaying = false;
            OnAnimationAToBSwapFinished?.Invoke();
        }

        /// <summary>
        /// This method be called by b_to_a animation event
        /// </summary>
        public void AnimationBToASwapFinished()
        {
            IsSwapPlaying = false;
            OnAnimationBToASwapFinished?.Invoke();
        }

        /// <summary>
        /// This method be called animation event
        /// </summary>
        public void AnimationSwapFinished()
        {
            IsSwapPlaying = false;
            OnAnimationSwapFinished?.Invoke();
        }
    }
}