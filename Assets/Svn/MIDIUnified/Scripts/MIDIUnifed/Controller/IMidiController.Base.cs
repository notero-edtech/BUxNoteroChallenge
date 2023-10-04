namespace ForieroEngine.MIDIUnified.Interfaces
{
    public interface IMidiObjects
    {
        string Id { get; }
        void SetId(string id);
    }

    public interface IMidiObject
    {
        int Index { get; }
        void SetIndex(int index);
    } 
}

