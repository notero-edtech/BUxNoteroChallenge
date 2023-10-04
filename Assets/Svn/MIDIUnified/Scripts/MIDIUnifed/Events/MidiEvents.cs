using System;
using System.Collections.Generic;
using UnityEngine;

namespace ForieroEngine.MIDIUnified
{
    public partial class MidiEvents
    {
        public string Name = "";
        public int DeviceId = -1;
        public bool log = false;
        
        public Action<string> OnLog;
        
        private List<IMidiSender> _generators = new List<IMidiSender>();
        private void ShortMessage(int aCommand, int aData1, int aData2, int deviceId)
        {
            if (this.DeviceId >= 0 && this.DeviceId != deviceId) return;
            AddShortMessage(aCommand, aData1, aData2, deviceId);
        }
       
        public bool HasSender(IMidiSender aSender) => _generators.Contains(aSender);
        
        public bool AddSender(IMidiSender aSender)
        {
            if (aSender != null)
            {
                if (!_generators.Contains(aSender))
                {
                    _generators.Add(aSender);
                    aSender.ShortMessageEvent += ShortMessage;                    
                    if (MIDISettings.IsDebug) Debug.Log("MU | IMidiSender ADDED");
                    return true;
                }
                else
                {
                    if (MIDISettings.IsDebug) Debug.LogWarning("MU | MidiEvents already contains this generate!");                    
                }
            }
            else
            {
                if (MIDISettings.IsDebug) Debug.LogError("MU | IMidieEvents IMidiSender is NULL!");                
            }

            return false;
        }

        public void RemoveSender(IMidiSender aSender)
        {
            if (_generators.Contains(aSender))
            {
                aSender.ShortMessageEvent -= ShortMessage;
                _generators.Remove(aSender);
                //Debug.Log("IMidiSender REMOVED");
            }
        }

        public void RemoveAllSenders()
        {
            foreach (IMidiSender generator in _generators)
            {
                if (generator != null) generator.ShortMessageEvent -= ShortMessage;
            }
            _generators = new List<IMidiSender>();
        }

        ~MidiEvents()
        {
            foreach (IMidiSender generator in _generators)
            {
                if (generator != null) generator.ShortMessageEvent -= ShortMessage;
            }
        }

        public void Dispose()
        {
            foreach (IMidiSender generator in _generators)
            {
                if (generator != null) generator.ShortMessageEvent -= ShortMessage;
            }
        }
    }
}
