using System.Collections.Generic;

namespace ForieroEngine.MIDIUnified.Interfaces
{
    public class MidiKnobs : IMidiKnobs
    {
        public string Id { get; private set; } = "";
        public void SetId(string id) { Id = id; }
        public SortedDictionary<int, IMidiKnob> Knobs { get; } = new();
    }
    public interface IMidiKnobs : IMidiObjects
    {
        SortedDictionary<int, IMidiKnob> Knobs { get; }
    }
    
    public interface IMidiKnob : IMidiObject
    {

    }
}
