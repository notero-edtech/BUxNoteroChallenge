using System.IO;
using UnityEngine;

public partial class CSharpSynth : MonoBehaviour
{
    public class MFile : AudioSynthesis.IResource
    {
        private readonly byte[] _file;
        private readonly string _fileName;
        
        public MFile(byte[] file, string fileName)
        {
            this._file = file;
            this._fileName = fileName;
        }
        
        public string GetName() => _fileName;
        public bool DeleteAllowed() => false;
        public bool ReadAllowed() => true;
        public bool WriteAllowed() => false;
        public void DeleteResource() { }
        public Stream OpenResourceForRead() => new MemoryStream(_file);
        public Stream OpenResourceForWrite() => null;
    }
}
