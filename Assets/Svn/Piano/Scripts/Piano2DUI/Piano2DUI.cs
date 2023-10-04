using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using ForieroEngine.Extensions;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Classes;using ForieroEngine.MIDIUnified.Interfaces;
using ForieroEngine.Music.NotationSystem;
using UnityEngine;
using KeySignatureEnum = ForieroEngine.MIDIUnified.KeySignatureEnum;

public class Piano2DUI : MidiController<Piano2DUI>, IMidiController, IMidiSender, IMidiReceiver
{
    [Tooltip("Start octave index from which we create keys.")]
    public int startOctaveIndex = 1;
    [Tooltip("Number of generated octaves.")]
    public int numberOfOctaves = 7;
    [Tooltip("C on the right of the keys.")]
    public bool includeUpperC = true;
    [Tooltip("Prepend A, #A/bB, B on the left of the keys.")]
    public bool includeLowerAB = true;
    
    private MidiEvents _receivedME = new();
  
    public RectTransform whiteKeysContainer;
    public RectTransform blackKeysContainer;

    public GameObject PREFAB_Key;

    public Material whiteKeyMaterial;
    public Material blackKeyMaterial;
    
    public bool faces = true;
    public TheorySystemEnum theorySystem = TheorySystemEnum.ToneNames;
    public KeySignatureEnum keySignature;
    public bool whiteKeysText = true;
    public float whiteKeysFontSizeRatio = 1.5f;
    [HideInInspector]
    public int whiteKeysFontSize = 14;
    [HideInInspector]
    public int whiteKeysSolfegeFontSize = 14;
    public bool blackKeysText = false;
    public float blackKeysFontSizeRatio = 2f;
    [HideInInspector]
    public int blackKeysFontSize = 10;

    public int ribbonWidth = 1;

    public RectTransform topRibbon;
    public RectTransform bottomRibbon;
    
    public bool highlightOctave = false;
    public Color highlightOctaveColor = Color.gray;
    public int highlightOctaveIndex = 4;
    
    public List<Piano2DUIKey> piano2DUIKeys = new List<Piano2DUIKey>();

    public static Dictionary<int, int> fingers = new Dictionary<int, int>();
    public static Dictionary<int, int> keyIds = new Dictionary<int, int>();
    
    protected override void Awake()
    {
        base.Awake();
        Piano.SelectedController = this;
        Piano.Controllers.Add(this);
        _receivedME.NoteOnEvent += ReceivedNoteON;
        _receivedME.NoteOffEvent += ReceivedNoteOFF;
    }

    protected override void OnDestroy()
    {
        if ((Piano2DUI)Piano.SelectedController == this) Piano.SelectedController = null;
        Piano.Controllers.Remove(this);
        base.OnDestroy();
    }
    private void Start() { CreateKeys(); }

    private void CreateKeys()
    {
       Piano2DUIKey key = null;

        int whiteKeyCount = 0;
        //int midiIndex = 0;

        int startIndex = startOctaveIndex * 12 - (includeLowerAB ? 3 : 0);
        int endIndex = startOctaveIndex * 12 + numberOfOctaves * 12 + (includeUpperC ? 1 : 0);

        //float totalKeyCount = (float)(endIndex - startIndex);
        float totalWhiteKeys = (float)((includeLowerAB ? 2 : 0) + numberOfOctaves * 7 + (includeUpperC ? 1 : 0));

        float whiteKeyWidth = whiteKeysContainer.GetSize().x / totalWhiteKeys;
        float whiteKeyHeight = whiteKeysContainer.GetSize().y;

        float blackKeyWidth = whiteKeyWidth * 2f / 3f;
        float blackKeyHeight = whiteKeyHeight * 4f / 6f;

        whiteKeysFontSize = (int)(whiteKeyWidth / whiteKeysFontSizeRatio);
        whiteKeysSolfegeFontSize = (int)(whiteKeysFontSize * 0.7f);
        blackKeysFontSize = (int)(blackKeyWidth / blackKeysFontSizeRatio);

        float startPosition = -whiteKeysContainer.GetSize().x / 2f + whiteKeyWidth / 2f;

        for (int i = startIndex; i < endIndex; i++)
        {
            RectTransform container = null;
            container = i.IsWhiteKey() ? whiteKeysContainer : blackKeysContainer;
          
            key = (Instantiate(PREFAB_Key, transform.position, Quaternion.identity, container) as GameObject).GetComponent<Piano2DUIKey>();
            piano2DUIKeys.Add(key);
            key.SetIndex(i);
            key.gameObject.transform.name = i.ToString() + "_" + MidiConversion.GetToneNameFromMidiIndex(i) + "_" + (startOctaveIndex + i.Octave()).ToString();
            keyIds.Add(key.gameObject.GetInstanceID(), i);
            Keys.Keys.Add(i, key);

            var keyRT = key.transform as RectTransform;
            var offset = Vector2.zero;

            ForieroEngineExtensions.SetPivotAndAnchors(keyRT, new Vector2(0.5f, 1f));
            ForieroEngineExtensions.SetPivotAndAnchors(key.text.rectTransform, new Vector2(0.5f, 1f));

            switch (key.KeyType)
            {
                case KeyType.Black:
                    offset = new Vector2(-whiteKeyWidth / 2f, 0);
                    keyRT.anchoredPosition = new Vector2(startPosition + whiteKeyCount * whiteKeyWidth + offset.x, offset.y);
                    keyRT.SetSize(new Vector2(blackKeyWidth, blackKeyHeight));
                    if (blackKeysText)
                    {
                        key.text.enabled = true;
                        key.text.fontSize = blackKeysFontSize;
                        key.text.rectTransform.anchoredPosition = new Vector2(0f, -blackKeyHeight + blackKeysFontSize * 3.5f);
                        key.text.fontMaterial = blackKeyMaterial;
                    }
                    else
                    {
                        key.text.enabled = false;
                    }
                    break;
                case KeyType.White:
                    offset = new Vector2(0f, 0);
                    keyRT.anchoredPosition = new Vector2(startPosition + whiteKeyCount * whiteKeyWidth, 0);
                    keyRT.SetSize(new Vector2(whiteKeyWidth, whiteKeyHeight));
                    if (whiteKeysText)
                    {
                        key.text.enabled = true;
                        key.text.fontSize = whiteKeysFontSize;
                        key.text.rectTransform.anchoredPosition = new Vector2(0f, -whiteKeyHeight + whiteKeysFontSize);
                        key.text.fontMaterial = whiteKeyMaterial;
                    }
                    else
                    {
                        key.text.enabled = false;
                    }
                    whiteKeyCount++;
                    break;
            }
        }
       
        SetTheorySystem(theorySystem, keySignature);
    }

#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER

    private void Update()
    {
        // Handle at Hendrix.Midi.MidiInputAdapter.cs
        return;
        var highlight = highlightOctave;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (highlightOctaveIndex > startOctaveIndex - (includeLowerAB ? 1 : 0))
            {
                ApplyHighlight(false);
                if (MidiKeyboardInput.singleton && MIDISettings.instance.keyboardSettings.muteTonesWhenChangingOctave)
                {
                    MidiKeyboardInput.singleton.MuteTones();
                }
                highlightOctaveIndex--;
                if (MidiKeyboardInput.singleton)
                    MIDISettings.instance.keyboardSettings.keyboardOctave = highlightOctaveIndex;
                ApplyHighlight(highlight);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (highlightOctaveIndex <= startOctaveIndex + numberOfOctaves - 2)
            {
                ApplyHighlight(false);
                if (MidiKeyboardInput.singleton && MIDISettings.instance.keyboardSettings.muteTonesWhenChangingOctave)
                {
                    MidiKeyboardInput.singleton.MuteTones();
                }
                highlightOctaveIndex++;
                if (MidiKeyboardInput.singleton)
                    MIDISettings.instance.keyboardSettings.keyboardOctave = highlightOctaveIndex;
                ApplyHighlight(highlight);
            }
        }
    }
#endif
    
    public void ApplyHighlight(bool aValue)
    {
        if (aValue) { Keys.OctaveSetWhiteKeysUpColor(highlightOctaveIndex, highlightOctaveColor); }
        else { Keys.OctaveSetWhiteKeysUpColorDefault(highlightOctaveIndex); }
        highlightOctave = aValue;
    }

    public void SetTheorySystem(TheorySystemEnum theorySystem, KeySignatureEnum keySignature)
    {
        for (var i = 0; i < piano2DUIKeys.Count; i++)
        {
            if (piano2DUIKeys[i].KeyType == KeyType.White && piano2DUIKeys[i].text)
            {
                switch (theorySystem)
                {
                    case TheorySystemEnum.Undefined:
                    case TheorySystemEnum.ToneNames:
                        piano2DUIKeys[i].text.fontSize = whiteKeysFontSize;
                        break;
                    case TheorySystemEnum.SolfegeFixed:
                    case TheorySystemEnum.SolfegeMovable:
                        piano2DUIKeys[i].text.fontSize = whiteKeysSolfegeFontSize;
                        break;
                }
            }
        }

        foreach (var key in Keys.Keys) { key.Value?.SetTheorySystem(theorySystem, keySignature); }
        this.theorySystem = theorySystem;
    }
    
    public override void OnMidiMessageReceived(int aCommand, int aData1, int aData2, int aDeviceId) => _receivedME.AddShortMessage(aCommand, aData1, aData2, aDeviceId);
    private void ReceivedNoteON(int index, int value, int channel) => Keys.KeyDown(index, value);
    private void ReceivedNoteOFF(int index, int value, int channel) => Keys.KeyUp(index);
    
    public override void Align(ControllerAlignment alignment)
    {
        base.Align(alignment);
        
        switch (alignment)
        {
            case ControllerAlignment.Bottom:
                RectTransform.pivot = new Vector2(0.5f, 0f);
                RectTransform.anchorMin = new Vector2(0f, 0f);
                RectTransform.anchorMax = new Vector2(1f, 0f);
                break;
            case ControllerAlignment.Top:
                RectTransform.pivot = new Vector2(0.5f, 1f);
                RectTransform.anchorMin = new Vector2(0f, 1f);
                RectTransform.anchorMax = new Vector2(1f, 1f);
                break;
        }
    }
    
    private TweenerCore<Vector2, Vector2, VectorOptions> _tweener;
    public override void Show(bool animated = true)
    {
        _tweener?.Kill();
        var d = 0.5f * (animated ? 1f : 0f);
        switch (Alignment)
        {
            case ControllerAlignment.Left:
            case ControllerAlignment.Right: _tweener = RectTransform.DOAnchorPosX(0, d); break;
            case ControllerAlignment.Top:
            case ControllerAlignment.Bottom: _tweener = RectTransform.DOAnchorPosY(0, d); break;
        }
        Hidden = false;
    }
    
    public override void Hide(bool animated = true)
    {
        _tweener?.Kill();
        var d = 0.5f * (animated ? 1f : 0f);
        switch (Alignment)
        {
            case ControllerAlignment.Left: _tweener = RectTransform.DOAnchorPosX(-RectTransform.GetWidth() * NSPlayback.Zoom,  d); break;
            case ControllerAlignment.Right: _tweener = RectTransform.DOAnchorPosX(RectTransform.GetWidth() * NSPlayback.Zoom, d); break;
            case ControllerAlignment.Top: _tweener = RectTransform.DOAnchorPosY(RectTransform.GetHeight() * NSPlayback.Zoom, d); break;
            case ControllerAlignment.Bottom: _tweener = RectTransform.DOAnchorPosY(-RectTransform.GetHeight() * NSPlayback.Zoom, d); break;
        }
        Hidden = true;
    }
}


