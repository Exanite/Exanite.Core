using System;
using UnityEngine;

namespace Exanite.Utility
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty(this Array array)
        {
            return (array == null || array.Length == 0);
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Collider collider) where T : Component
        {
            return collider.GetComponent<T>() ?? collider.gameObject.AddComponent<T>();
        }
    }
}