using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BU.NineTails.Gameplay
{
    public class CharactersAnimationController : MonoBehaviour
    {
        [SerializeField] private int[] characterNotes;
        private Vector3 jumpDistance = new Vector3(0, 0.4f, 0);
        private float jumpDuration = 0.5f;
        private Animator animator;
        private Vector3 originalPosition;
        private float animationTimer;
        private bool isJumping;
        private bool ownNote;

        private void Start()
        {
            animator = GetComponent<Animator>();
            animator.SetBool("opps", false);
            animator.SetBool("good", false);
            animator.SetBool("perfect", false);
            originalPosition = transform.position;
        }
        private void Update()
        {
            if (isJumping)
            {
                animationTimer += Time.deltaTime;
                float progress = animationTimer / jumpDuration;

                if (progress < 1.0f)
                {
                    transform.position = Vector3.Lerp(originalPosition, originalPosition + jumpDistance, progress);
                }
                else
                {
                    transform.position = originalPosition;
                    animator.SetBool("opps", false);
                    animator.SetBool("good", false);
                    animator.SetBool("perfect", false);
                    isJumping = false;
                }
            }
        }

        public void CheckOppsAnimation(int note)
        {
            foreach (int i in characterNotes)
            {
                if(i == note)
                {
                    ownNote = true;
                }
            }
            if (ownNote)
            {
                animationTimer = 0f;
                animator.SetBool("opps", true);
                isJumping = true;
                ownNote = false;
            }
        }

        public void CheckGoodAnimation(int note)
        {
            foreach (int i in characterNotes)
            {
                if (i == note)
                {
                    ownNote = true;
                }
            }
            if (ownNote)
            {
                animationTimer = 0f;
                animator.SetBool("good", true);
                isJumping = true;
                ownNote = false;
            }
        }

        public void CheckPerfectAnimation(int note)
        {
            foreach (int i in characterNotes)
            {
                if (i == note)
                {
                    ownNote = true;
                }
            }
            if (ownNote)
            {
                animationTimer = 0f;
                animator.SetBool("perfect", true);
                isJumping = true;
                ownNote = false;
            }
        }
    }
}
