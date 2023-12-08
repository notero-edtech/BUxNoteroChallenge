using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BU.NineTails.Gameplay
{
    public class CharactersAnimationController : MonoBehaviour
    {
        [SerializeField] private int[] characterNotes;
        private Animator animator;
        private float animationTimer = 0.5f;
        private CharacterState currentState;

        private enum CharacterState
        {
            Idle,
            Opps,
            Good,
            Perfect
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
            SetAnimationState(CharacterState.Idle);
        }

        private void SetAnimationState(CharacterState newState)
        {
            currentState = newState;
            if (currentState == CharacterState.Idle)
            {
                animator.SetBool("opps", false);
                animator.SetBool("good", false);
                animator.SetBool("perfect", false);
            }
            else
            {
                animator.SetBool("opps", currentState == CharacterState.Opps);
                animator.SetBool("good", currentState == CharacterState.Good);
                animator.SetBool("perfect", currentState == CharacterState.Perfect);
            }
        }

        private bool CheckNote(int note)
        {
            foreach (int i in characterNotes)
            {
                if (i == note)
                {
                    return true;
                }
            }
            return false;
        }

        public void CheckOppsAnimation(int note)
        {
            if (CheckNote(note))
            {
                SetAnimationState(CharacterState.Opps);
                StartCoroutine(ResetToIdleAfterDelay(animationTimer));
            }
        }

        public void CheckGoodAnimation(int note)
        {
            if (CheckNote(note))
            {
                SetAnimationState(CharacterState.Good);
                StartCoroutine(ResetToIdleAfterDelay(animationTimer));
            }
        }

        public void CheckPerfectAnimation(int note)
        {
            if (CheckNote(note))
            {
                SetAnimationState(CharacterState.Perfect);
                StartCoroutine(ResetToIdleAfterDelay(animationTimer));
            }
        }

        private IEnumerator ResetToIdleAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            SetAnimationState(CharacterState.Idle);
        }
    }
}
