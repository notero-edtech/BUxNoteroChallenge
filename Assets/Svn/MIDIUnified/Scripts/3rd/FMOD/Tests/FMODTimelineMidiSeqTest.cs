using ForieroEngine.MIDIUnified;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;


public class FMODTimelineMidiSeqTest : MonoBehaviour
{
    [RestrictInterface(typeof(IMidiSender))]
    public Object generator;

    public FMODTimelineMidiSeq timelineMidiSeq;

    #if FMOD
    [FMODUnity.EventRef]
    #endif 
    public string one, two, three, four;
    
    
    public Slider slider;
    public TextMeshProUGUI text; 
    
    private MidiEvents _me = new MidiEvents();

    void Awake()
    {
        _me.AddSender(generator as IMidiSender);
        _me.NoteOnEvent += (id, value, channel) =>
        {
            //Debug.Log($"Id : {id} | Value :  {value} | Channel: {channel}");
            if (channel != 2) return;
            #if FMOD
            if(id == 72) FMODUnity.RuntimeManager.PlayOneShot(one);
            if(id == 74) FMODUnity.RuntimeManager.PlayOneShot(two);
            if(id == 76) FMODUnity.RuntimeManager.PlayOneShot(three);
            if(id == 77) FMODUnity.RuntimeManager.PlayOneShot(four);
            #endif
        };

        if (slider)
        {
            slider.onValueChanged.AddListener((v) =>
            {
                text.text = v.ToString();
                timelineMidiSeq.timeOffset = v;
            });
        }
    }

    private void OnDestroy()
    {
        _me.RemoveAllSenders();
    }
}
