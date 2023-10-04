using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForieroEngine.MIDIUnified
{
    public interface IMidiReceiver
    {
        string Id { get; }
        void OnMidiMessageReceived(int aCommand, int aData1, int aData2, int aDeviceId);
    }

    public static class MidiReceivers
    {
        private static readonly List<IMidiReceiver> Receivers = new List<IMidiReceiver>();

        public static void Register(this IMidiReceiver i, bool ignoreNullOrEmptyId = true)
        {
            if (i == null || (string.IsNullOrEmpty(i.Id) && ignoreNullOrEmptyId)) return;

            if (Receivers.Contains(i)) Debug.LogError($"IMidiGenerator instance with id {i.Id} already exists!!!");
            else if (!string.IsNullOrEmpty(i.Id)) Receivers.Add(i);
        }

        public static IMidiReceiver GetById(string id) => Receivers.FirstOrDefault(i => i != null && i.Id == id);

        public static void Unregister(this IMidiReceiver i)
        {
            if (Receivers.Contains(i)) Receivers.Remove(i);
        }
    }
}