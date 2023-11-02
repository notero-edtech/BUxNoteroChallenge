using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Slider Healthbar_Silder;
    public float maxHealth = 100;
    public float currentHealth;
    public float currentVelocity = 0;
    public bool life;
    public float hp;
    private bool isStart;

    public void SetMaxHealth(float health)
    {
        Healthbar_Silder.maxValue = health;
        Healthbar_Silder.value = health;
    }

    /*public void SetHealth(float health)
    {
        Healthbar_Silder.value = health;
    }*/

    public void Start()
    {
        isStart = true;
        life = true;
        currentHealth = maxHealth;
        hp = maxHealth;
        SetMaxHealth(maxHealth);

    }

    public void Perfect_Healthbar()
    {
        if (life == true)
        {
            hp += 40f;
        }
    }

    public void Great_Healthbar()
    {
        if (life == true)
        {
            hp += 20f;
        }
    }

    public void Missed_Healthbar()
    {
        if (life == true)
        {
            hp -= 20f;
        }
    }
    private void Update()
    {
        if (isStart)
        {
            currentHealth = Mathf.SmoothDamp(currentHealth, hp, ref currentVelocity, 0.5f);
            Healthbar_Silder.value = currentHealth;
        }

        if (currentHealth > 0)
        {
            life = true;
        }
        else
        {
            life = false;
        }

        if (hp > maxHealth)
        {
            hp = maxHealth;
        }
        else if (life == false)
        {
            hp = 0;
        }
    }
}