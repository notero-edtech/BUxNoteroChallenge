using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissScreen : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject failPanel;

   
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
            if (GameObject.Find("vfx_oops_old") != null)
            {
                failPanel.SetActive(true);
            }

            else
            {
                failPanel.SetActive(false);
            }

        

    }

    void FixedUpdate()
    {
      

    }
    








}
