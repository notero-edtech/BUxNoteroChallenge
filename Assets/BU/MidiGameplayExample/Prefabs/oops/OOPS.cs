using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OOPS : MonoBehaviour
{
    public delegate void PlayerKilled();
    public static event OnplayerKilled;'

    public GameObject m_GotHitScreen;

    void Start()
    {


    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "cube")
        {
            gotHurt();
        }
    }


    void gotHurt()
    {
        var color = m = GotHitScreen.GetCoponent<Image>().color;
        color.a = 0.8f;

        m_GotHitScreen.GetComponent<int>().color = color;
    }




    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) {
            die();
        }

        if (m_GotHitScreen != null)
        {
            if (m_GotHitScreen.GetCoponent<Image>().color.a > 0)
            {
                var color = m_GotHitScreen.GetComponent<Image>().color;
                color.a -= 0.01f;
                m_GotHitScreen.GetComponent<Image>().color = color;
            }
        }
    }
}
