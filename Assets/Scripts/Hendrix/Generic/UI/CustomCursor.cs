using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomCursor : MonoBehaviour
{
    private bool isAndroid;
    private void Awake()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            isAndroid = true;
            Destroy(gameObject);
            return;
        }
        Cursor.visible = false;

        DontDestroyOnLoad(this);
    }

    private void Update()
    {  
        ReplaceCursor();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!isAndroid)
        {
            Cursor.visible = !focus;   
        }    
    }

    private void ReplaceCursor()
    {
        Vector2 m_MousePos = Input.mousePosition;
        
        transform.position = m_MousePos;
    }

}
