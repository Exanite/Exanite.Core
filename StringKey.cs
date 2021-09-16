﻿using System;

namespace Exanite.Core
{
    public struct StringKey : IEquatable<StringKey>
    {
        public string Value { get; }

        public StringKey(string value)
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }
        
        public static bool operator ==(StringKey lhs, StringKey rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(StringKey lhs, StringKey rhs)
        {
            return !lhs.Equals(rhs);
        }

        public bool Equals(StringKey other)
        {
            return Value.Equals(other.Value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            if (obj is StringKey key)
            {
                return Equals(key);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}