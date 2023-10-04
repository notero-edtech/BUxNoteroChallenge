using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForieroEngine.MIDIUnified.Plugins
{

    public static class TimeProviders
    {
        public static ITimeProvider iTimeProvider { get; private set; }
        private static readonly List<ITimeProvider> _providers = new List<ITimeProvider>();

        public static void Register(this ITimeProvider i, bool ignoreNullOrEmptyId = true)
        {
            Debug.Log($"Registering Time Provider : {i.Id}");
            if (string.IsNullOrEmpty(i.Id) && ignoreNullOrEmptyId) return;
            if (_providers.Contains(i)) Debug.LogError($"IMidiGenerator instance with id {i.Id} already exists!!!");
            else if (!string.IsNullOrEmpty(i.Id)) _providers.Add(i);
        }

        public static ITimeProvider GetById(string id) => _providers.FirstOrDefault(i => i != null && i.Id == id);

        public static void Unregister(this ITimeProvider i)
        {
            if (!_providers.Contains(i)) return;
            if (i == iTimeProvider) iTimeProvider = null;
            _providers.Remove(i);
        }

        public static void Init(string id)
        {
            iTimeProvider = GetById(id);
            foreach (var p in _providers)
            {
                if (p == iTimeProvider) p.EnableTimeProvider();
                else p.DisableTimeProvider();
            }
        }
    }

    public interface ITimeProvider
    {
        string Id { get; }
        float GetTime();
        void SetTime(float value);
        void EnableTimeProvider();
        void DisableTimeProvider();
    }
}

