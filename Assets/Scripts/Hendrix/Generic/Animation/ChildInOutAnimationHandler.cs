using System;
using UnityEngine;

namespace Hendrix.Generic.Animation
{
    public class ChildInOutAnimationHandler : MonoBehaviour
    {
        [SerializeField]
        private Animator m_Animator;

        private readonly int m_InAnimationHash = Animator.StringToHash("in");
        private readonly int m_OutAnimationHash = Animator.StringToHash("out");

        /// <summary>
        /// Trigger in animation
        /// </summary>
        public void PlayIn()
        {
            m_Animator.SetTrigger(m_InAnimationHash);
        }

        /// <summary>
        /// Trigger out animation
        /// </summary>
        public void PlayOut()
        {
            m_Animator.SetTrigger(m_OutAnimationHash);
        }
    }
}