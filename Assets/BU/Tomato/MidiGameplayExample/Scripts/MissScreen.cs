using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissScreen : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject failPanel;
    [SerializeField] private GameObject bgPerfect;
    [SerializeField] private GameObject bgOops;
    [SerializeField] private GameObject bgNormal;

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
            bgPerfect.SetActive(false);
            bgNormal.SetActive(false);
            failPanel.SetActive(true);

                if(slider.succeed == true)
                {
                    bgOops.SetActive(true);
                    
                
                
                }

               if (GameObject.Find("vfx_perfect_old") != null)
                    {
                        bgOops.SetActive(false);
                    if (slider.succeed == true)
                   {

                    bgPerfect.SetActive(true);  
                   }
               }

        }

        else
        {
            failPanel.SetActive(false);

          
        }

        if (GameObject.Find("vfx_perfect_old") != null)
        {
            bgOops.SetActive(false);
            bgNormal.SetActive(false);
            if (slider.succeed == true)
            {

                bgPerfect.SetActive(true);
            }
        }


        if (GameObject.Find("vfx_perfect_old") != null && GameObject.Find("vfx_oops_old") != null)
        {
                bgPerfect.SetActive(false);
                bgOops.SetActive(false);
            if (slider.succeed == true)
            {

                bgNormal.SetActive(true);
            }
        }







    }




}
