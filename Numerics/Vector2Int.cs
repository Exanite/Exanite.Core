using System;
using System.Numerics;

namespace Exanite.Core.Numerics
{
    public struct Vector2Int : IEquatable<Vector2Int>, IFormattable
    {
        /// <inheritdoc cref="Vector2.X"/>
        public int X;

        /// <inheritdoc cref="Vector2.Y"/>
        public int Y;

        /// <inheritdoc cref="Vector2.Zero"/>
        public static Vector2Int Zero => default;

        /// <inheritdoc cref="Vector2.One"/>
        public static Vector2Int One => new Vector2Int(1);

        /// <inheritdoc cref="Vector2.UnitX"/>
        public static Vector2Int UnitX => new Vector2Int(1, 0);

        /// <inheritdoc cref="Vector2.UnitY"/>
        public static Vector2Int UnitY => new Vector2Int(0, 1);

        public Vector2Int(int value) : this(value, value) {}

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static explicit operator Vector2Int(Vector2 value)
        {
            return new Vector2Int((int)value.X, (int)value.Y);
        }

        public static implicit operator Vector2(Vector2Int value)
        {
            return new Vector2(value.X, value.Y);
        }

        public static Vector2Int operator +(Vector2Int left, Vector2Int right)
        {
            return new Vector2Int(
                left.X + right.X,
                left.Y + right.Y
            );
        }

        public static Vector2Int operator -(Vector2Int left, Vector2Int right)
        {
            return new Vector2Int(
                left.X - right.X,
                left.Y - right.Y
            );
        }

        public static Vector2Int operator -(Vector2Int value)
        {
            return Zero - value;
        }

        public static bool operator ==(Vector2Int left, Vector2Int right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2Int left, Vector2Int right)
        {
            return !left.Equals(right);
        }

        public bool Equals(Vector2Int other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object? obj)
        {
            return obj is Vector2Int other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return ((Vector2)this).ToString(format, formatProvider);
        }
    }
}
