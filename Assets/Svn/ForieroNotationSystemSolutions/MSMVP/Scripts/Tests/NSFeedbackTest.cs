using ForieroEngine.Music.NotationSystem.Classes;
using TMPro;
using UnityEngine;

public class NSFeedbackTest : MonoBehaviour
{
    public TextMeshProUGUI on;
    public TextMeshProUGUI off;

    private volatile string onString = "";
    private volatile bool onUpdate = false;
    private volatile string offString = "";
    private volatile bool offUpdate = false;
        
    void Awake()
    {
        NSObject.MidiMessageData.OnToneOnFeedbackThreaded += data =>
        {
            onString = data.ToneOnFeedback.ToString() + " " + data.ToneOnDiffMS + "ms";
            onUpdate = true;
        };
        
        NSObject.MidiMessageData.OnToneOffFeedbackThreaded += data =>
        {
            offString = data.ToneOffFeedback.ToString() + " " + data.ToneOffDiffMS + "ms";
            offUpdate = true;
        };
    }

    private void Update()
    {
        if (onUpdate)
        {
            on.text = onString;
            onUpdate = false;
        }

        if (offUpdate)
        {
            off.text = offString;
            offUpdate = false;
        }
    }
}
