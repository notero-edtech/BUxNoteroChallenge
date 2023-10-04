using System.Collections.Generic;

namespace ForieroEngine.MIDIUnified.Interfaces
{
    public interface IMidiInstrument{
        string Id { get; }
        
        void Show(bool animated = true);
        void Hide(bool animated = true);
    }
}
