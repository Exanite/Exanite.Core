using System;
using System.Collections.Generic;
using Sirenix.Serialization;

namespace Exanite.Core.DataStructures
{
    /// <summary>
    /// Used to store miscellaneous data
    /// </summary>
    [Serializable]
    public class DataBoard
    {
        /// <summary>
        /// Called whenever a value in the <see cref="DataBoard"/> is editted
        /// </summary>
        public event Action OnValueChanged;

        [OdinSerialize] private Dictionary<string, object> data = new Dictionary<string, object>();

        /// <summary>
        /// Is the <see cref="DataBoard"/> empty?
        /// </summary>
        public bool IsEmpty => data.Count == 0;

        /// <summary>
        /// Gets a stored value
        /// </summary>
        public T GetValue<T>(string key)
        {
            if (data.TryGetValue(key, out object value))
            {
                return (T)value;
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// Sets a stored value
        /// </summary>
        public void SetValue<T>(string key, T value)
        {
            data[key] = value;
            OnValueChanged?.Invoke();
        }

        /// <summary>
        /// Clears the <see cref="DataBoard"/> of any stored values
        /// </summary>
        public void Clear()
        {
            data.Clear();
            OnValueChanged?.Invoke();
        }
    }
}
