using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForieroEngine.MIDIUnified.Interfaces
{
    public enum ControllerAlignment { Left = 0, Right = 1, Top = 2, Bottom = 3 }
    public enum ControllerPosition { Left = 0, Right = 1, Top = 2, Bottom = 3 }
    
    public interface IMidiController
    {
        string Id { get; }
        ControllerAlignment Alignment { get; }
        bool Colored { get; }
        Transform Transform { get; }
        RectTransform RectTransform { get; }

        //Bounds Bounds { get; }
        IMidiKeys Keys { get; }
        IMidiPercussions Percussions { get; }
        IMidiButtons Buttons { get; }
        IMidiKnobs Knobs { get; }
        IMidiFaders Faders { get; }
        bool Hidden { get; }
        void Show(bool animated = true);
        void Hide(bool animated = true);
        void Align(ControllerAlignment alignment);
        Vector3 GetWorldPosition(ControllerPosition point);
    }

    public static class MidiControllers
    {
        public static IMidiController Selected = null;
        
        private static readonly List<IMidiController> Controllers = new List<IMidiController>();
	
        public static void Register(this IMidiController i, bool ignoreNullOrEmptyId = true)
        {
            if (i == null || (string.IsNullOrEmpty(i.Id) && ignoreNullOrEmptyId)) return;
		
            if (Controllers.Contains(i)) Debug.LogError($"IMidiController instance with id {i.Id} already exists!!!");
            else if(!string.IsNullOrEmpty(i.Id)) Controllers.Add(i);
        }

        public static IMidiController GetById(string id) => Controllers.FirstOrDefault(i => i != null && i.Id == id);
	
        public static void Unregister(this IMidiController i)
        {
            if(Controllers.Contains(i)) Controllers.Remove(i);
        }
    }
}