using System;
using UnityEngine;

namespace Hendrix.Generic.Animation
{
    public class InOutAnimationHandler : MonoBehaviour
    {
        [SerializeField]
        protected Animator m_Animator;

        [SerializeField]
        private ChildInOutAnimationHandler[] m_ChildAnimationHandlers;

        public event Action OnAnimationInFinished;
        public event Action OnAnimationOutFinished;

        public bool IsPlaying { get; private set; }

        protected readonly int m_InAnimationHash = Animator.StringToHash("in");
        protected readonly int m_OutAnimationHash = Animator.StringToHash("out");

        /// <summary>
        /// Trigger in animation
        /// </summary>
        public void PlayIn()
        {
            IsPlaying = true;
            m_Animator.SetTrigger(m_InAnimationHash);

            foreach(var handler in m_ChildAnimationHandlers)
            {
                handler.PlayIn();
            }
        }

        /// <summary>
        /// Trigger out animation
        /// </summary>
        public void PlayOut()
        {
            IsPlaying = true;
            m_Animator.SetTrigger(m_OutAnimationHash);

            foreach (var handler in m_ChildAnimationHandlers)
            {
                handler.PlayOut();
            }

        }

        /// <summary>
        /// This method be called by in animation event
        /// </summary>
        public void AnimationInFinished()
        {
            IsPlaying = false;
            OnAnimationInFinished?.Invoke();
        }

        /// <summary>
        /// This method be called by out animation event
        /// </summary>
        public void AnimationOutFinished()
        {
            IsPlaying = false;
            OnAnimationOutFinished?.Invoke();
        }

        public AnimatorStateInfo GetCurrentStateInfo() => m_Animator.GetCurrentAnimatorStateInfo(0);
    }
}