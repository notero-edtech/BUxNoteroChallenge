using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarSlider : MonoBehaviour
{
    public int progress = 0;
    public bool succeed = false;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private TMP_Text currentValueText;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject barColor;
    [SerializeField] private GameObject handle;
    [SerializeField] private GameObject thaiBG;
    [SerializeField] private GameObject normalBG;


    Color lowScore = Color.red;
    Color midScore = Color.yellow;
    Color highScore = Color.green;

    void FixedUpdate()
    {
      
            UpdateProgress();
        
       
    }

   
  

    public void UpdateProgress()
    { 
        
        if(valueText.text == ("2000") || valueText.text == ("2500"))
        {
            progress = 1;
            slider.value = progress;
            barColor.gameObject.GetComponent<Image>().color = lowScore;
            handle.gameObject.GetComponent<Image>().color = lowScore;
         

      
        }

        if (valueText.text == ("4000") || valueText.text == ("4500"))
        {
            progress = 2;
            slider.value = progress;
            barColor.gameObject.GetComponent<Image>().color = lowScore;
            handle.gameObject.GetComponent<Image>().color = lowScore;
        }

     

        if (valueText.text == ("6000") || valueText.text == ("6500"))
        {
            progress = 3;
            slider.value = progress;
            barColor.gameObject.GetComponent<Image>().color = lowScore;
            handle.gameObject.GetComponent<Image>().color = lowScore;

        }

     

        if (valueText.text == ("8000") || valueText.text == ("8500"))
        {
            progress = 4;
            slider.value = progress;
            barColor.gameObject.GetComponent<Image>().color = midScore;
            handle.gameObject.GetComponent<Image>().color = midScore;
        }


        if (valueText.text == ("10000") || valueText.text == ("10500"))
        {
            progress = 5;
            slider.value = progress;
            barColor.gameObject.GetComponent<Image>().color = midScore;
            handle.gameObject.GetComponent<Image>().color = midScore;
        }

    

        if (valueText.text == ("12000") || valueText.text == ("12500"))
        {
            progress = 6;
            slider.value = progress;
            barColor.gameObject.GetComponent<Image>().color = midScore;
            handle.gameObject.GetComponent<Image>().color = midScore;

        }


        if (valueText.text == ("14000") || valueText.text == ("14500"))
        {
            progress = 7;
            slider.value = progress;
            barColor.gameObject.GetComponent<Image>().color = midScore;
            handle.gameObject.GetComponent<Image>().color = midScore;

        }


        if (valueText.text == ("16000") || valueText.text == ("16500"))
        {
            progress = 8;
            slider.value = progress;
            barColor.gameObject.GetComponent<Image>().color = highScore;
            handle.gameObject.GetComponent<Image>().color = highScore;

        }

       
        if (valueText.text == ("18000") || valueText.text == ("18500"))
        {
            progress = 9;
            slider.value = progress;
            barColor.gameObject.GetComponent<Image>().color = highScore;
            handle.gameObject.GetComponent<Image>().color = highScore;
        }


        if (valueText.text == ("20000") || valueText.text == ("20500"))
        {
            progress = 10;
            slider.value = progress;
            barColor.gameObject.GetComponent<Image>().color = highScore;
            handle.gameObject.GetComponent<Image>().color = highScore;
            normalBG.SetActive(true);
            thaiBG.SetActive(false);
            succeed = true;
        }
      
    }
    
}
