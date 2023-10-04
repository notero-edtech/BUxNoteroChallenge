using UnityEngine;

public class SMLogMessage : MonoBehaviour
{
    public float speed = 50;
    RectTransform rt;
    
    static SMLogMessage lastMessage;
    // Start is called before the first frame update
    void Start()
    {
        rt = this.transform as RectTransform;
        if (lastMessage && lastMessage.rt.anchoredPosition.y <= 20f){
           var offset = -20 + lastMessage.rt.anchoredPosition.y;
           rt.anchoredPosition = new Vector2(0, offset);
        }
        lastMessage = this;           
    }

    // Update is called once per frame
    void Update()
    {
        rt.anchoredPosition += Vector2.up * (Time.deltaTime * speed);

        if(rt.anchoredPosition.y > Screen.height / 2f)
        {
            Destroy(this.gameObject);
        }
    }
}
