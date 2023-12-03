using System;
using System.Collections;
using System.Collections.Generic;
using Notero.MidiGameplay.Core;
using Notero.RaindropGameplay.Core;
using UnityEngine;
using Notero.RaindropGameplay.UI;

namespace BU.Octopuz.Scripts
{
    public class Bright : MonoBehaviour
    {
        public GameObject Bridge1;
        public GameObject Bridge2;
        public GameObject Bridge3;
        public GameObject Bridge4;
        public GameObject Bridge5;
        public static int CountBridge = 0;
        public ITimeProvider TimeProvider { get; private set; }
        float timeRemaining = 0f;
        
        public float m_MaxTime;
        
        public void SetTimerBarMaxValue(float value)
        {
            m_MaxTime = value;
        }

        public void OnScoreUpdated(SelfResultInfo studentResultInfo)
        {
            SetSpawnBridge(studentResultInfo.AccuracyPercent);
        }

        public void OnGameplayTimeUpdate(float seconds)
        {
            var time = m_MaxTime - seconds;
            Debug.Log(time);
            Debug.Log(timeRemaining);

            if (time <= 10 && RunOneTime == 0)
            {
               // if (timeRemaining <= 0f) 
              //  {
                    
                    float newMaxTime = 10f;
                    Animetion(); 
                    RunOneTime = 1;
                    timeRemaining = 5f; 
             //   }
            }
            
           // if (time > 10 && timeRemaining > 0f)
           // {
            //    timeRemaining -= Time.deltaTime; 
         //  }
        }

        public void SetSpawnBridge(float Percent)
        {
            if (Percent >= 1 && CountBridge == 0) //ความคีบหน้าเมื่อถึง 19 สะพานอันที่ 1
            {
                CountBridge++;
                Bridge1.SetActive(true);

            }
            else if (Percent >= 20 && CountBridge == 1) //ความคีบหน้าเมื่อถึง 39 สะพานอันที่ 2
            {
                CountBridge++;
                Bridge2.SetActive(true);
            }
            else if (Percent >= 40 && CountBridge == 2) //ความคีบหน้าเมื่อถึง 59 สะพานอันที่ 3
            {
                CountBridge++;
                Bridge3.SetActive(true);
            }
            else if (Percent >= 60 && CountBridge == 3) //ความคีบหน้าเมื่อถึง 79 สะพานอันที่ 4
            {
                CountBridge++;
                Bridge4.SetActive(true);
            }
            else if (Percent >= 80 && CountBridge == 4) //ความคีบหน้าเมื่อถึง 100 สะพานอันที่ 5
            {
                CountBridge++;
                Bridge5.SetActive(true);
            }
        }
         public int RunOneTime = 0;
        public GameObject bad;
        public GameObject notbad;
        public GameObject good;
        public GameObject great;
        public GameObject perfect;
        public GameObject TrunoffP;
        public GameObject TrunoffB;
        public GameObject idel;
        public GameObject goodAnime;
        public GameObject badAnime;
        
        
        public void Animetion()
        {
            if (CountBridge == 0 || CountBridge == 1)
            {
                idel.SetActive(false);
                bad.SetActive(true);
                TrunoffP.SetActive(false);
                TrunoffB.SetActive(false);
            }
            if (CountBridge == 2)
            {
                idel.SetActive(false);
                notbad.SetActive(true);
                TrunoffP.SetActive(false);
                TrunoffB.SetActive(false);
            }
            if (CountBridge == 3)
            {
                idel.SetActive(false);
                good.SetActive(true);
                TrunoffP.SetActive(false);
                TrunoffB.SetActive(false);
            }
            if (CountBridge == 4)
            {
                idel.SetActive(false);
                great.SetActive(true);
                TrunoffP.SetActive(false);
                TrunoffB.SetActive(false);
            }
            if (CountBridge == 5)
            {
                idel.SetActive(false);
                perfect.SetActive(true);
                TrunoffP.SetActive(false);
                TrunoffB.SetActive(false);
            }
        }
        public void AnimetionFeedback(float Check)
        {
            if (Check == 1)
            {
                Debug.Log("ASASDASDASD");
                idel.SetActive(false);
                goodAnime.SetActive(true);
                
            }

            if (Check == 0)
            {
                idel.SetActive(false);
                badAnime.SetActive(true);
            }
        }
    }
}