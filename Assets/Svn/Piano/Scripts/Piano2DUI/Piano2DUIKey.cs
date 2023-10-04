using System.Collections.Generic;
using ForieroEngine.Extensions;
using ForieroEngine.MIDIUnified;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ForieroEngine.MIDIUnified.Interfaces;
using ForieroEngine.MIDIUnified.Plugins;

public class Piano2DUIKey : MonoBehaviour, IMidiKey, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
	public int index = 0;
	public int Index => index;
	
	private static List<int> touchIds = new List<int>();
	private static List<Piano2DUIKey> downKeys = new List<Piano2DUIKey>();

	private int touchId = int.MinValue;

	#region IPointerExitHandler implementation

	public void OnPointerExit(PointerEventData eventData)
	{
		if (IsDown && touchIds.Contains(eventData.pointerId))
		{
			if (downKeys.Contains(this)) { downKeys.Remove(this); }
			touchId = int.MinValue;
			IsDown = false;
			MidiOut.NoteOff(Index);
			SetKeyUp();
		}
	}

	#endregion

	#region IPointerEnterHandler implementation

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!IsDown && touchIds.Contains(eventData.pointerId))
		{
			if (!downKeys.Contains(this)) { downKeys.Add(this); }
			touchId = eventData.pointerId;
			IsDown = true;
			MidiOut.NoteOn(Index);
			SetKeyDown();
		}
	}

	#endregion

	#region IPointerDownHandler implementation

	private MIDISettings.MidiInstrumentSettings Settings => MIDISettings.instance.instrumentSettings; 
	public void OnPointerDown(PointerEventData eventData)
	{
		if (!touchIds.Contains(eventData.pointerId)) { touchIds.Add(eventData.pointerId); }
		if (!downKeys.Contains(this)) { downKeys.Add(this); }
		touchId = eventData.pointerId;
		IsDown = true;
		if(Settings.active && Settings.evaluate) MidiINPlugin.DSP.ToneOn(Index);
		MidiOut.NoteOn(Index);
		SetKeyDown();
	}

	#endregion

	#region IPointerUpHandler implementation

	public void OnPointerUp(PointerEventData eventData)
	{
		if (touchIds.Contains(eventData.pointerId)) { touchIds.Remove(eventData.pointerId); }

		for (var i = downKeys.Count - 1; i >= 0; i--)
		{
			if (downKeys[i].touchId != eventData.pointerId) continue;
			
			downKeys[i].touchId = int.MinValue;
			downKeys[i].IsDown = false;
			if(Settings.active && Settings.evaluate) MidiINPlugin.DSP.ToneOff(downKeys[i].Index);
			MidiOut.NoteOff(downKeys[i].Index);
			downKeys[i].SetKeyUp();
			downKeys.Remove(downKeys[i]);
		}
	}

	#endregion

	public Sprite whiteKeyUp;
	public Sprite whiteKeyDown;
	public Sprite whiteKeyDownShadow;

	public Sprite blackKeyUp;
	public Sprite blackKeyUpShadow;
	public Sprite blackKeyDown;
	public Sprite blackKeyDownShadow;

	public float blackVolumeMultiplicator = 2.2f;
	public float whiteVolumeMultiplicator = 1.2f;
	
	public Image keyImage;

	public TextMeshProUGUI text;

	public Image shadowImage;

	public ToneEnum Tone { get; protected set; } = ToneEnum.A;
	public KeyType KeyType { get; set; } = KeyType.White;
	public bool IsDown { get; protected set; } = false;
	public Color DefaultDownColor { get; protected set; } = Color.grey;
	public Color DefaultUpColor { get; protected set; } = Color.white;
	public Color DownColor { get; protected set; } = Color.grey;
	public Color UpColor { get; protected set; } = Color.white;
	public Color HighlightColor { get; protected set; } = Color.grey;
	public Color DefaultUpColorWhiteKey { get; protected set; } = Color.white;
	public Color DefaultUpColorBlackKey { get; protected set; } = Color.white;
	
	protected void Awake()
	{
		
	}
	
	public GameObject GetGameObject()
	{
		return this.gameObject;
	}

	public void SetTheorySystem(TheorySystemEnum theorySystem, KeySignatureEnum keySignature)
	{
		if (text) text.text = MidiConversion.GetToneNameFromMidiIndex(Index, '\n', theorySystem, keySignature);
	}
	
	public KeyType GetKeyType() => KeyType;
	public Vector3 GetPosition() => transform.position;
	public Vector3 GetLocalPosition() => transform.localPosition;
	public Color GetDownColor() => DownColor;
	public Color GetUpColor() => UpColor;
	public Color GetDownColorDefault() => DefaultDownColor;
	public Color GetUpColorDefault() => DefaultUpColor;
	public void SetDownColor(Color aColor) => DownColor = aColor; 
	public void SetUpColor(Color aColor) => UpColor = aColor;
	public void SetDownColorDefault(Color aColor) => DefaultDownColor = aColor;
	public void SetUpColorDefault(Color aColor) => DefaultUpColor = aColor; 
	public bool GetIsDown() => IsDown;
	
	private void ApplyWhiteDownShadow() { }

	private void SetupWhiteKey()
	{
		UpColor = DefaultUpColorWhiteKey;

		if (text) text.text = Tone.ToString();

		KeyType = KeyType.White;
		shadowImage.enabled = false;

		if (IsDown)
		{
			var i = Index.PrevWhiteKey();
			if (Piano2DUI.Instance.Keys.IsKeyDown(i)) { keyImage.sprite = whiteKeyDown; }
			else { keyImage.sprite = whiteKeyDownShadow; }

			keyImage.color = DownColor;

			if (text) { text.color = Color.white; }
		}
		else
		{
			keyImage.sprite = whiteKeyUp;
			keyImage.color = UpColor;

			if (text) { text.color = Tone.ToColor().Brightness(0.8f); }
		}
	}

	private void SetupBlackKey()
	{
		UpColor = DefaultUpColorBlackKey;

		KeyType = KeyType.Black;

		if (text) text.text = Tone.ToString();

		shadowImage.enabled = true;

		if (IsDown)
		{
			keyImage.sprite = blackKeyDown;
			shadowImage.sprite = blackKeyDownShadow;
			keyImage.color = DownColor;

			if (text) text.color = Color.white;
		}
		else
		{
			keyImage.sprite = blackKeyUp;
			shadowImage.sprite = blackKeyUpShadow;
			keyImage.color = UpColor;

			if (text) text.color = Tone.ToColor().Brightness(0.8f);
		}
	}

	public void SetIndex(int i)
	{
		Tone = MidiConversion.GetBaseToneFromMidiIndex(i);
		KeyType = i.IsWhiteKey() ? KeyType.White : KeyType.Black;
		this.index = i;
		
		switch (KeyType)
		{
			case KeyType.White: SetupWhiteKey(); break;
			case KeyType.Black: SetupBlackKey(); break;
		}
		
		DownColor = Piano2DUI.Instance.Colored ? Tone.ToColor().Brightness(0.8f) : DefaultDownColor;
	}
	
	public void ColorKey(Color aColor) { keyImage.color = aColor; }

	public void ColorOn()
	{
		keyImage.color = Piano2DUI.Instance.Colored ? DownColor : DefaultDownColor;
		if (text && text.gameObject.activeSelf && !Application.isMobilePlatform) { text.color = Color.white; }
	}

	public void ColorOff()
	{
		keyImage.color = UpColor;
		if (text && text.gameObject.activeSelf && !Application.isMobilePlatform) { text.color = Tone.ToColor().Brightness(0.8f); }
	}
	
	public void SetKeyUp()
	{
		ColorOff();
		switch (KeyType)
		{
			case KeyType.Black:
				keyImage.sprite = blackKeyUp;
				shadowImage.sprite = blackKeyUpShadow;
				break;
			case KeyType.White:
				keyImage.sprite = whiteKeyUp;
				var i = Index.NextWhiteKey();
				if (Piano2DUI.Instance.Keys.IsKeyDown(i)) { ((Piano2DUIKey)Piano2DUI.Instance.Keys.Keys[i]).keyImage.sprite = whiteKeyDownShadow; }
				break;
		}

		IsDown = false;
	}

	public void SetKeyDown()
	{
		ColorOn();
		switch (KeyType)
		{
			case KeyType.Black:
				keyImage.sprite = blackKeyDown;
				shadowImage.sprite = blackKeyDownShadow;
				break;
			case KeyType.White:
				var i = Index.PrevWhiteKey();
				if (Piano2DUI.Instance.Keys.IsKeyDown(i)) { keyImage.sprite = whiteKeyDown; }
				else { keyImage.sprite = whiteKeyDownShadow; }

				i = Index.NextWhiteKey();
				if (Piano2DUI.Instance.Keys.IsKeyDown(i)) { ((Piano2DUIKey)Piano2DUI.Instance.Keys.Keys[i]).keyImage.sprite = whiteKeyDown; }
				break;
		}
		IsDown = true;
	}

	public void TurnOffFace() { if (text) { text.enabled = false; } }
	public void TurnOnFace() { if (KeyType != KeyType.White) return; if (text) { text.enabled = true; } }
}
