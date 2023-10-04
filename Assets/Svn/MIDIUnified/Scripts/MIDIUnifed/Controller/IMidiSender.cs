using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForieroEngine.MIDIUnified
{
    public delegate void ShortMessageEventHandler(int aCommand, int aData1, int aData2, int deviceId);

    public interface IMidiSender
    {
        string Id { get; }
        event ShortMessageEventHandler ShortMessageEvent;
    }

    public static class MidiSenders
    {
        private static readonly List<IMidiSender> Senders = new List<IMidiSender>();

        public static void Register(this IMidiSender i, bool ignoreNullOrEmptyId = true)
        {
            if (i == null || (string.IsNullOrEmpty(i.Id) && ignoreNullOrEmptyId)) return;

            if (Senders.Contains(i)) Debug.LogError($"IMidiGenerator instance with id {i.Id} already exists!!!");
            else if (!string.IsNullOrEmpty(i.Id)) Senders.Add(i);
        }

        public static IMidiSender GetById(string id) => Senders.FirstOrDefault(i => i != null && i.Id == id);

        public static void Unregister(this IMidiSender i)
        {
            if (Senders.Contains(i)) Senders.Remove(i);
        }
    }
}
