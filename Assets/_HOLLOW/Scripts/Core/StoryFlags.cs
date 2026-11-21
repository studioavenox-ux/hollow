using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hollow.Core
{
    /// <summary>
    /// Lightweight story-flag store. Flags record story progression
    /// (e.g. "met_gopinath", "collected_photograph") without hard-coding
    /// progression into scene scripts (Master §15.3).
    /// Persisted by <see cref="SaveSystem"/>.
    /// </summary>
    public static class StoryFlags
    {
        private static readonly Dictionary<string, bool> _flags = new Dictionary<string, bool>();

        /// <summary>Raised when a flag changes. Args: flag name, new value.</summary>
        public static event Action<string, bool> OnFlagChanged;

        public static void Set(string flagName, bool value = true)
        {
            if (string.IsNullOrEmpty(flagName)) return;
            bool previous = _flags.TryGetValue(flagName, out bool v) && v;
            _flags[flagName] = value;
            if (previous != value)
            {
                OnFlagChanged?.Invoke(flagName, value);
            }
        }

        public static bool Get(string flagName)
        {
            return !string.IsNullOrEmpty(flagName) && _flags.TryGetValue(flagName, out bool v) && v;
        }

        public static bool Has(string flagName) => Get(flagName);

        public static void ClearAll()
        {
            _flags.Clear();
        }

        /// <summary>Returns a snapshot copy for saving.</summary>
        public static Dictionary<string, bool> Snapshot()
        {
            return new Dictionary<string, bool>(_flags);
        }

        /// <summary>Restores flags from a save snapshot.</summary>
        public static void Restore(Dictionary<string, bool> snapshot)
        {
            _flags.Clear();
            if (snapshot == null) return;
            foreach (var kvp in snapshot)
            {
                _flags[kvp.Key] = kvp.Value;
            }
        }
    }
}
