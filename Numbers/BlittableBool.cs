﻿using System;

namespace Exanite.Core.Numbers
{
    /// <summary>
    /// A blittable bool
    /// </summary>
    public struct BlittableBool : IEquatable<BlittableBool>
    {
        // Code found at: https://raw.githubusercontent.com/Unity-Technologies/Unity.Mathematics/0.0.12-preview.2/src/Unity.Mathematics/bool1.cs

        private readonly int value;

        /// <summary>
        /// Creates a new <see cref="BlittableBool"/>
        /// </summary>
        public BlittableBool(bool value)
        {
            this.value = value ? 1 : 0;
        }

        public static implicit operator bool(BlittableBool value)
        {
            return value.value != 0;
        }

        public static implicit operator BlittableBool(bool value)
        {
            return new BlittableBool(value);
        }

        public bool Equals(BlittableBool other)
        {
            return value == other.value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return obj is BlittableBool && Equals((BlittableBool)obj);
        }

        public override int GetHashCode()
        {
            return value;
        }

        public static bool operator ==(BlittableBool left, BlittableBool right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BlittableBool left, BlittableBool right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return ((bool)this).ToString();
        }
    }
}