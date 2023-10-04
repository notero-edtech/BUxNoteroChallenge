using ForieroEngine.MIDIUnified;
using UnityEngine;


[AddComponentMenu("Piano/PianoPedal")]
public class PianoPedal : MonoBehaviour
{
    public bool isDown = false;
    public PedalEnum pianoPedal = PedalEnum.DamperPedal;

    public AnimationClip pedalDownClip;
    public AnimationClip pedalUpClip;

    Animation anim;

    void OnEnable()
    {
        anim = GetComponent<Animation>();
    }

    void Awake()
    {
        if (anim)
        {
            anim.AddClip(pedalUpClip, pedalUpClip.name);
            anim.AddClip(pedalDownClip, pedalDownClip.name);
        }
    }

    void OnMouseDown()
    {
        switch (pianoPedal)
        {
            case PedalEnum.DamperPedal:
                if (!isDown)
                {
                    PedalDown();
                    MidiOut.Pedal(64, 127);
                }
                break;
            case PedalEnum.Sostenuto:
                if (!isDown)
                {
                    PedalDown();
                    MidiOut.Pedal(66, 127);
                }
                break;
            case PedalEnum.SoftPedal:
                if (!isDown)
                {
                    PedalDown();
                    MidiOut.Pedal(67, 127);
                }
                break;
        }
    }

    bool hovering = false;

    void OnMouseEnter()
    {
        hovering = true;
    }

    void OnMouseExit()
    {
        hovering = false;
    }

    void OnMouseUp()
    {
        if (hovering)
        {
            switch (pianoPedal)
            {
                case PedalEnum.DamperPedal:
                    if (isDown)
                    {
                        PedalUp();
                        MidiOut.Pedal(64, 0);
                    }
                    break;
                case PedalEnum.Sostenuto:
                    if (isDown)
                    {
                        PedalUp();
                        MidiOut.Pedal(66, 0);
                    }
                    break;
                case PedalEnum.SoftPedal:
                    if (isDown)
                    {
                        PedalUp();
                        MidiOut.Pedal(67, 0);
                    }
                    break;
            }
        }
    }

    public void PedalDown()
    {
        if (anim && !isDown)
        {
            anim.PlayQueued(pedalDownClip.name);
        }
        isDown = true;
    }

    public void PedalUp()
    {
        if (anim && isDown)
        {
            anim.PlayQueued(pedalUpClip.name);
        }
        isDown = false;
    }

}
