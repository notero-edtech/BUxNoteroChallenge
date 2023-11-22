using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BU.NineTails.Scripts.UI
{
    public class HealthSliderController : MonoBehaviour
    {
        [SerializeField] private Slider Healthbar_Silder;
        [SerializeField] private float maxHealth = 100;
        [SerializeField] private float currentHealth;
        [SerializeField] private float currentVelocity = 0;
        [SerializeField] private bool alived;
        [SerializeField] private float hp;
        private bool isStart;

        private bool isDead;

        public void SetMaxHealth(float health)
        {
            Healthbar_Silder.maxValue = health;
            Healthbar_Silder.value = health;
        }

        public void Start()
        {
            isDead = false;
            if  (isDead == false)
            {
                isStart = true;
                alived = true;
                currentHealth = maxHealth;
                hp = maxHealth;
                SetMaxHealth(maxHealth);
            }
        }

        public void Perfect_Healthbar()
        {
            hp += 20f;
        }

        public void Good_Healthbar()
        {
            hp += 15f;
        }

        public void Opps_Healthbar()
        {
            hp -= 10f;
        }
        private void Update()
        {
            if (isDead == false)
            {
                if (isStart)
                {
                    currentHealth = Mathf.SmoothDamp(currentHealth, hp, ref currentVelocity, 0.5f);
                    Healthbar_Silder.value = currentHealth;
                }

                if (currentHealth > 0)
                {
                    alived = true;
                }
                else
                {
                    alived = false;
                }

                if (hp > maxHealth)
                {
                    hp = maxHealth;
                }
                else if (alived == false)
                {
                    hp = 0;
                    isDead = true;
                }
            }
        }
    }
}