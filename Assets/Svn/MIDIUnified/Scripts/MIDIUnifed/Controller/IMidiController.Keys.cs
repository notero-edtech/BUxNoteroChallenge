using System.Collections.Generic;
using UnityEngine;

namespace ForieroEngine.MIDIUnified.Interfaces
{
    public enum KeyType { None, Black, White }
    public class MidiKeys : IMidiKeys
    {
        public string Id { get; private set; } = "";
        public void SetId(string id) { Id = id; }
        public SortedDictionary<int, IMidiKey> Keys { get; } = new();
        
    }
    public interface IMidiKeys : IMidiObjects
    {
        SortedDictionary<int, IMidiKey> Keys { get; }
        public bool KeyExists(int i) =>  Keys.ContainsKey(i);
        public void KeyDown(int i, int aVolume) { if (Keys.ContainsKey(i)) Keys[i].SetKeyDown(); }
        public void KeyUp(int i) { if (Keys.ContainsKey(i)) Keys[i].SetKeyUp(); }
        public Vector3 GetKeyPosition(int i) { return Keys.ContainsKey(i) ? Keys[i].GetPosition() : Vector3.zero; }
        public Vector3 GetKeyLocalPosition(int i) { return Keys.ContainsKey(i) ? Keys[i].GetLocalPosition() : Vector3.zero; }
        public void ColorKey(int i, Color aColor) { if (Keys.ContainsKey(i)) Keys[i].ColorKey(aColor); }
        public Color GetKeyDownColor(int i) { return Keys.ContainsKey(i) ? Keys[i].GetDownColor() : Color.white; }
        public Color GetKeyUpColor(int i) { return Keys.ContainsKey(i) ? Keys[i].GetUpColor() : Color.white; }
        public Color GetKeyDownColorDefault(int i) { return Keys.ContainsKey(i) ? Keys[i].GetDownColorDefault() : Color.white; }
        public Color GetKeyUpColorDefault(int i) { return Keys.ContainsKey(i) ? Keys[i].GetUpColorDefault() : Color.white; }
        public void SetKeyDownColor(int i, Color aColor) { if (Keys.ContainsKey(i)) Keys[i].SetDownColor(aColor);}
        public void SetKeyUpColor(int i, Color aColor)  { if (Keys.ContainsKey(i)) Keys[i].SetUpColor(aColor);}
        public void ColorKeyDown(int i) { if (Keys.ContainsKey(i)) Keys[i].ColorOn();}
        public void ColorKeyUp(int i) { if (Keys.ContainsKey(i)) Keys[i].ColorOff();}
        public bool IsKeyDown(int i) { return Keys.ContainsKey(i) && Keys[i].GetIsDown(); }
        public KeyType GetKeyType(int i) { return Keys.ContainsKey(i) ? Keys[i].GetKeyType() : KeyType.None; }
        public void SetTheorySystem(TheorySystemEnum theorySystem, KeySignatureEnum keySignature)
        {
            foreach (var key in Keys) { key.Value?.SetTheorySystem(theorySystem, keySignature); }
        }
        public void OctaveSetWhiteKeysUpColor(int anOctave, Color aColor)
        {
            for (var i = anOctave * 12; i < anOctave * 12 + 13; i++)
            {
                if (GetKeyType(i) != KeyType.White) continue;
                SetKeyUpColor(i, aColor);
                if (!IsKeyDown(i)) ColorKeyUp(i);
            }
        }

        public void OctaveSetWhiteKeysUpColorDefault(int anOctave)
        {
            for (var i = anOctave * 12; i < anOctave * 12 + 13; i++)
            {
                if (GetKeyType(i) != KeyType.White) continue;
                SetKeyUpColor(i, GetKeyUpColorDefault(i));
                if (!IsKeyDown(i)) ColorKeyUp(i);
            }
        }
        
        public void AllKeysUp() { foreach (var key in Keys) { key.Value?.SetKeyUp(); } }
        public void AllKeysDown() { foreach (var key in Keys) { key.Value?.SetKeyDown(); } }
    }

    public interface IMidiKey : IMidiObject
    {
        public ToneEnum Tone { get; }
        public KeyType KeyType { get; }
        public bool IsDown { get; }
        public Color DefaultDownColor { get; }
        public Color DefaultUpColor { get; }
        public Color DownColor { get; }
        public Color UpColor { get; }
        public Color HighlightColor { get; }
        public Color DefaultUpColorWhiteKey { get; }
        public Color DefaultUpColorBlackKey { get; }
        
        void ColorKey(Color aColor);
        void ColorOn();
        void ColorOff();
        void SetKeyUp();
        void SetKeyDown();
        void TurnOffFace();
        void TurnOnFace();
        KeyType GetKeyType();
        Vector3 GetPosition();
        Vector3 GetLocalPosition();
        Color GetDownColor();
        Color GetUpColor();
        Color GetDownColorDefault();
        Color GetUpColorDefault();
        void SetDownColor(Color aColor);
        void SetUpColor(Color aColor);
        void SetDownColorDefault(Color aColor);
        void SetUpColorDefault(Color aColor);
        bool GetIsDown();
        GameObject GetGameObject();
        void SetTheorySystem(TheorySystemEnum theorySystem, KeySignatureEnum keySignatureEnum);
    }
}
