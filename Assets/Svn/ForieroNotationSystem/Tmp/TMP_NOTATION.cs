/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using TMPro;
using UnityEngine;

public class TMP_NOTATION : MonoBehaviour { 

    public Camera camera;
    public float cameraSpeed = 0.1f;
    public float distance = 0.3f;
    public int count = 1000;
    public GameObject PREFAB_Tmp;
    
    public TextMeshPro tmp;
    public Transform top;
    public Transform bottom;

    TextMeshPro[] t = null;
    // Use this for initialization
    void Start()
    {
        t = new TextMeshPro[count];


        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(PREFAB_Tmp, this.transform) as GameObject;
            //var go = Instantiate(PREFAB_Tmp) as GameObject;
            var rt = go.transform as RectTransform;
            rt.SetPixelPosition(new Vector2(80 * i, 0));
            t[i] = go.GetComponent<TextMeshPro>();
        }

        SetLines();
    }

    // Update is called once per frame
    Color color;

    void Update()
    {
        camera.transform.Translate(Vector3.right * Time.deltaTime * cameraSpeed);

        for(int i = 0; i<10; i++)
        {
            color = t[i].color;
            color.r = Mathf.Repeat(t[i].color.r + 0.01f, 1f);
            t[i].color = color;
        }
    }

    void SetLines()
    {
        float unitHeight = NSCanvas.GetLineHeightInUnits(tmp.font, tmp.fontSize) / 2f;
        top.position = new Vector3(tmp.transform.position.x, tmp.transform.position.y + unitHeight, 0);
        bottom.position = new Vector3(tmp.transform.position.x, tmp.transform.position.y - unitHeight, 0);
    }

    //void OnGUI()
    //{
    //    GUILayout.Label(Screen.dpi.ToString());
    //    if (GUILayout.Button("Test"))
    //    {
    //        SetLines();
            
    //    }

    //    GUILayout.Label("Asc : " + tmp.font.fontInfo.Ascender);
    //    GUILayout.Label("Des : " + tmp.font.fontInfo.Descender);
    //    GUILayout.Label("PointSize : " + tmp.font.fontInfo.PointSize);
    //    GUILayout.Label("FontSize : " + tmp.fontSize);
    //}
}
