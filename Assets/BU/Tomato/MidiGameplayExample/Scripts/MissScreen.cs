using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissScreen : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject failPanel;
    [SerializeField] private GameObject m_BGPerfect;
    [SerializeField] private GameObject m_BGOops;
    [SerializeField] private GameObject m_BGNormal;

    private BarSlider slider;

     void Start()
    {
        slider = GetComponent<BarSlider>();
    }

    // Update is called once per frame
    void Update()
    {

        if (GameObject.Find("vfx_oops_old") != null)
            {
            m_BGPerfect.SetActive(false);
            m_BGNormal.SetActive(false);
            failPanel.SetActive(true);

                if(slider.succeed == true)
                {
                    m_BGOops.SetActive(true);
                    
                
                
                }

               if (GameObject.Find("vfx_perfect_old") != null)
                    {
                    m_BGOops.SetActive(false);
                    if (slider.succeed == true)
                   {

                    m_BGPerfect.SetActive(true);  
                   }
               }

        }

        else
        {
            failPanel.SetActive(false);

          
        }

        if (GameObject.Find("vfx_perfect_old") != null)
        {
            m_BGOops.SetActive(false);
            m_BGNormal.SetActive(false);
            if (slider.succeed == true)
            {

                m_BGPerfect.SetActive(true);
            }
        }


        if (GameObject.Find("vfx_perfect_old") != null && GameObject.Find("vfx_oops_old") != null)
        {
                m_BGPerfect.SetActive(false);
                m_BGOops.SetActive(false);
            if (slider.succeed == true)
            {

                m_BGOops.SetActive(true);
            }
        }







    }




}
