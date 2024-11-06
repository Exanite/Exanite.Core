using System;
using System.Numerics;

namespace Exanite.Core.Numerics
{
    public struct Vector3Int : IEquatable<Vector3Int>, IFormattable
    {
        /// <inheritdoc cref="Vector3.X"/>
        public int X;

        /// <inheritdoc cref="Vector3.Y"/>
        public int Y;

        /// <inheritdoc cref="Vector3.Z"/>
        public int Z;

        /// <inheritdoc cref="Vector3.Zero"/>
        public static Vector3Int Zero => default;

        /// <inheritdoc cref="Vector3.One"/>
        public static Vector3Int One => new Vector3Int(1);

        /// <inheritdoc cref="Vector3.UnitX"/>
        public static Vector3Int UnitX => new Vector3Int(1, 0, 0);

        /// <inheritdoc cref="Vector3.UnitY"/>
        public static Vector3Int UnitY => new Vector3Int(0, 1, 0);

        /// <inheritdoc cref="Vector3.UnitZ"/>
        public static Vector3Int UnitZ => new Vector3Int(0, 0, 1);

        public Vector3Int(int value) : this(value, value, value) {}

        public Vector3Int(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static explicit operator Vector3Int(Vector3 value)
        {
            return new Vector3Int((int)value.X, (int)value.Y, (int)value.Z);
        }

        public static implicit operator Vector3(Vector3Int value)
        {
            return new Vector3(value.X, value.Y, value.Z);
        }

        public static Vector3Int operator +(Vector3Int left, Vector3Int right)
        {
            return new Vector3Int(
                left.X + right.X,
                left.Y + right.Y,
                left.Z + right.Z
            );
        }

        public static Vector3Int operator -(Vector3Int left, Vector3Int right)
        {
            return new Vector3Int(
                left.X - right.X,
                left.Y - right.Y,
                left.Z - right.Z
            );
        }

        public static Vector3Int operator -(Vector3Int value)
        {
            return Zero - value;
        }

        public static bool operator ==(Vector3Int left, Vector3Int right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3Int left, Vector3Int right)
        {
            return !left.Equals(right);
        }

        public bool Equals(Vector3Int other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object? obj)
        {
            return obj is Vector3Int other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return ((Vector3)this).ToString(format, formatProvider);
        }
    }
}
