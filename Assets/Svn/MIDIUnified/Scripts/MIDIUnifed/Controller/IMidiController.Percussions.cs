using System.Collections.Generic;

namespace ForieroEngine.MIDIUnified.Interfaces
{
    public class MidiPercussions : IMidiPercussions
    {
        public string Id { get; private set; } = "";
        public void SetId(string id) { Id = id; }
        public SortedDictionary<int, IMidiPercussion> Perscussions { get; } = new();
    }
    public interface IMidiPercussions : IMidiObjects
    {
        SortedDictionary<int, IMidiPercussion> Perscussions { get; }
    }
    
    public interface IMidiPercussion : IMidiObject
    {

    }
}
