using System.Collections.Generic;

namespace ForieroEngine.MIDIUnified.Interfaces
{
    public class MidiButtons : IMidiButtons
    {
        public string Id { get; private set; } = "";
        public void SetId(string id) { Id = id; }
        public SortedDictionary<int, IMidiButton> Buttons { get; } = new();
    }
    public interface IMidiButtons : IMidiObjects
    {
        SortedDictionary<int, IMidiButton> Buttons { get; }
    }
    
    public interface IMidiButton : IMidiObject
    {

    }
}
