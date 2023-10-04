using System.Collections.Generic;

namespace ForieroEngine.MIDIUnified.Interfaces
{
    public class MidiFaders : IMidiFaders
    {
        public string Id { get; private set; } = "";
        public void SetId(string id) { Id = id; }
        public SortedDictionary<int, IMidiFader> Faders { get; } = new();
    }
    public interface IMidiFaders : IMidiObjects
    {
        SortedDictionary<int, IMidiFader> Faders { get; }
    }

    public interface IMidiFader : IMidiObject
    {
        
    }    
}

