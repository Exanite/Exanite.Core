#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Exanite.Core.Numerics;

public struct Vector2Int : IEquatable<Vector2Int>, IFormattable
{
    /// <inheritdoc cref="Vector2.X"/>
    public int X;

    /// <inheritdoc cref="Vector2.Y"/>
    public int Y;

    /// <inheritdoc cref="Vector2.Zero"/>
    public static Vector2Int Zero => default;

    /// <inheritdoc cref="Vector2.UnitX"/>
    public static Vector2Int UnitX => new(1, 0);

    /// <inheritdoc cref="Vector2.UnitY"/>
    public static Vector2Int UnitY => new(0, 1);

    public int this[int index]
    {
        readonly get
        {
            switch (index)
            {
                case 0: return X;
                case 1: return Y;
                default: throw new IndexOutOfRangeException(nameof(index));
            }
        }

        set
        {
            switch (index)
            {
                case 0: X = value; break;
                case 1: Y = value; break;
                default: throw new IndexOutOfRangeException(nameof(index));
            }
        }
    }

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

    public static Vector2Int operator *(Vector2Int value, int scalar)
    {
        return new Vector2Int(value.X * scalar, value.Y * scalar);
    }

    public static Vector2 operator *(Vector2Int value, float scalar)
    {
        return new Vector2(value.X * scalar, value.Y * scalar);
    }

    public static Vector2Int operator /(Vector2Int value, int scalar)
    {
        return new Vector2Int(value.X * scalar, value.Y * scalar);
    }

    public static Vector2 operator /(Vector2Int value, float scalar)
    {
        return new Vector2(value.X * scalar, value.Y * scalar);
    }

    public static Vector2Int operator +(Vector2Int left, Vector2Int right)
    {
        return new Vector2Int(left.X + right.X, left.Y + right.Y);
    }

    public static Vector2Int operator -(Vector2Int left, Vector2Int right)
    {
        return new Vector2Int(left.X - right.X, left.Y - right.Y);
    }

    public static Vector2Int operator *(Vector2Int left, Vector2Int right)
    {
        return new Vector2Int(left.X * right.X, left.Y * right.Y);
    }

    public static Vector2Int operator /(Vector2Int left, Vector2Int right)
    {
        return new Vector2Int(left.X / right.X, left.Y / right.Y);
    }

    public static Vector2Int operator <<(Vector2Int left, Vector2Int right)
    {
        return new Vector2Int(left.X << right.X, left.Y << right.Y);
    }

    public static Vector2Int operator >>(Vector2Int left, Vector2Int right)
    {
        return new Vector2Int(left.X >> right.X, left.Y >> right.Y);
    }

    public static Vector2Int operator >>>(Vector2Int left, Vector2Int right)
    {
        return new Vector2Int(left.X >>> right.X, left.Y >>> right.Y);
    }

    public static Vector2Int operator &(Vector2Int left, Vector2Int right)
    {
        return new Vector2Int(left.X & right.X, left.Y & right.Y);
    }

    public static Vector2Int operator |(Vector2Int left, Vector2Int right)
    {
        return new Vector2Int(left.X | right.X, left.Y | right.Y);
    }

    public static Vector2Int operator ^(Vector2Int left, Vector2Int right)
    {
        return new Vector2Int(left.X ^ right.X, left.Y ^ right.Y);
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

    public override string ToString()
    {
        return ToString("G", CultureInfo.CurrentCulture);
    }

    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format)
    {
        return ToString(format, CultureInfo.CurrentCulture);
    }

    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format, IFormatProvider? formatProvider)
    {
        return ((Vector2)this).ToString(format, formatProvider);
    }
}

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

    /// <inheritdoc cref="Vector3.UnitX"/>
    public static Vector3Int UnitX => new(1, 0, 0);

    /// <inheritdoc cref="Vector3.UnitY"/>
    public static Vector3Int UnitY => new(0, 1, 0);

    /// <inheritdoc cref="Vector3.UnitZ"/>
    public static Vector3Int UnitZ => new(0, 0, 1);

    public int this[int index]
    {
        readonly get
        {
            switch (index)
            {
                case 0: return X;
                case 1: return Y;
                case 2: return Z;
                default: throw new IndexOutOfRangeException(nameof(index));
            }
        }

        set
        {
            switch (index)
            {
                case 0: X = value; break;
                case 1: Y = value; break;
                case 2: Z = value; break;
                default: throw new IndexOutOfRangeException(nameof(index));
            }
        }
    }

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

    public static Vector3Int operator *(Vector3Int value, int scalar)
    {
        return new Vector3Int(value.X * scalar, value.Y * scalar, value.Z * scalar);
    }

    public static Vector3 operator *(Vector3Int value, float scalar)
    {
        return new Vector3(value.X * scalar, value.Y * scalar, value.Z * scalar);
    }

    public static Vector3Int operator /(Vector3Int value, int scalar)
    {
        return new Vector3Int(value.X * scalar, value.Y * scalar, value.Z * scalar);
    }

    public static Vector3 operator /(Vector3Int value, float scalar)
    {
        return new Vector3(value.X * scalar, value.Y * scalar, value.Z * scalar);
    }

    public static Vector3Int operator +(Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    }

    public static Vector3Int operator -(Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    }

    public static Vector3Int operator *(Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
    }

    public static Vector3Int operator /(Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
    }

    public static Vector3Int operator <<(Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X << right.X, left.Y << right.Y, left.Z << right.Z);
    }

    public static Vector3Int operator >>(Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X >> right.X, left.Y >> right.Y, left.Z >> right.Z);
    }

    public static Vector3Int operator >>>(Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X >>> right.X, left.Y >>> right.Y, left.Z >>> right.Z);
    }

    public static Vector3Int operator &(Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X & right.X, left.Y & right.Y, left.Z & right.Z);
    }

    public static Vector3Int operator |(Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X | right.X, left.Y | right.Y, left.Z | right.Z);
    }

    public static Vector3Int operator ^(Vector3Int left, Vector3Int right)
    {
        return new Vector3Int(left.X ^ right.X, left.Y ^ right.Y, left.Z ^ right.Z);
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

    public override string ToString()
    {
        return ToString("G", CultureInfo.CurrentCulture);
    }

    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format)
    {
        return ToString(format, CultureInfo.CurrentCulture);
    }

    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format, IFormatProvider? formatProvider)
    {
        return ((Vector3)this).ToString(format, formatProvider);
    }
}

public struct Vector4Int : IEquatable<Vector4Int>, IFormattable
{
    /// <inheritdoc cref="Vector4.X"/>
    public int X;

    /// <inheritdoc cref="Vector4.Y"/>
    public int Y;

    /// <inheritdoc cref="Vector4.Z"/>
    public int Z;

    /// <inheritdoc cref="Vector4.W"/>
    public int W;

    /// <inheritdoc cref="Vector4.Zero"/>
    public static Vector4Int Zero => default;

    /// <inheritdoc cref="Vector4.UnitX"/>
    public static Vector4Int UnitX => new(1, 0, 0, 0);

    /// <inheritdoc cref="Vector4.UnitY"/>
    public static Vector4Int UnitY => new(0, 1, 0, 0);

    /// <inheritdoc cref="Vector4.UnitZ"/>
    public static Vector4Int UnitZ => new(0, 0, 1, 0);

    /// <inheritdoc cref="Vector4.UnitW"/>
    public static Vector4Int UnitW => new(0, 0, 0, 1);

    public int this[int index]
    {
        readonly get
        {
            switch (index)
            {
                case 0: return X;
                case 1: return Y;
                case 2: return Z;
                case 3: return W;
                default: throw new IndexOutOfRangeException(nameof(index));
            }
        }

        set
        {
            switch (index)
            {
                case 0: X = value; break;
                case 1: Y = value; break;
                case 2: Z = value; break;
                case 3: W = value; break;
                default: throw new IndexOutOfRangeException(nameof(index));
            }
        }
    }

    public Vector4Int(int value) : this(value, value, value, value) {}

    public Vector4Int(int x, int y, int z, int w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public static explicit operator Vector4Int(Vector4 value)
    {
        return new Vector4Int((int)value.X, (int)value.Y, (int)value.Z, (int)value.W);
    }

    public static implicit operator Vector4(Vector4Int value)
    {
        return new Vector4(value.X, value.Y, value.Z, value.W);
    }

    public static Vector4Int operator *(Vector4Int value, int scalar)
    {
        return new Vector4Int(value.X * scalar, value.Y * scalar, value.Z * scalar, value.W * scalar);
    }

    public static Vector4 operator *(Vector4Int value, float scalar)
    {
        return new Vector4(value.X * scalar, value.Y * scalar, value.Z * scalar, value.W * scalar);
    }

    public static Vector4Int operator /(Vector4Int value, int scalar)
    {
        return new Vector4Int(value.X * scalar, value.Y * scalar, value.Z * scalar, value.W * scalar);
    }

    public static Vector4 operator /(Vector4Int value, float scalar)
    {
        return new Vector4(value.X * scalar, value.Y * scalar, value.Z * scalar, value.W * scalar);
    }

    public static Vector4Int operator +(Vector4Int left, Vector4Int right)
    {
        return new Vector4Int(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
    }

    public static Vector4Int operator -(Vector4Int left, Vector4Int right)
    {
        return new Vector4Int(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
    }

    public static Vector4Int operator *(Vector4Int left, Vector4Int right)
    {
        return new Vector4Int(left.X * right.X, left.Y * right.Y, left.Z * right.Z, left.W * right.W);
    }

    public static Vector4Int operator /(Vector4Int left, Vector4Int right)
    {
        return new Vector4Int(left.X / right.X, left.Y / right.Y, left.Z / right.Z, left.W / right.W);
    }

    public static Vector4Int operator <<(Vector4Int left, Vector4Int right)
    {
        return new Vector4Int(left.X << right.X, left.Y << right.Y, left.Z << right.Z, left.W << right.W);
    }

    public static Vector4Int operator >>(Vector4Int left, Vector4Int right)
    {
        return new Vector4Int(left.X >> right.X, left.Y >> right.Y, left.Z >> right.Z, left.W >> right.W);
    }

    public static Vector4Int operator >>>(Vector4Int left, Vector4Int right)
    {
        return new Vector4Int(left.X >>> right.X, left.Y >>> right.Y, left.Z >>> right.Z, left.W >>> right.W);
    }

    public static Vector4Int operator &(Vector4Int left, Vector4Int right)
    {
        return new Vector4Int(left.X & right.X, left.Y & right.Y, left.Z & right.Z, left.W & right.W);
    }

    public static Vector4Int operator |(Vector4Int left, Vector4Int right)
    {
        return new Vector4Int(left.X | right.X, left.Y | right.Y, left.Z | right.Z, left.W | right.W);
    }

    public static Vector4Int operator ^(Vector4Int left, Vector4Int right)
    {
        return new Vector4Int(left.X ^ right.X, left.Y ^ right.Y, left.Z ^ right.Z, left.W ^ right.W);
    }

    public static Vector4Int operator -(Vector4Int value)
    {
        return Zero - value;
    }

    public static bool operator ==(Vector4Int left, Vector4Int right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Vector4Int left, Vector4Int right)
    {
        return !left.Equals(right);
    }

    public bool Equals(Vector4Int other)
    {
        return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector4Int other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z, W);
    }

    public override string ToString()
    {
        return ToString("G", CultureInfo.CurrentCulture);
    }

    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format)
    {
        return ToString(format, CultureInfo.CurrentCulture);
    }

    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format, IFormatProvider? formatProvider)
    {
        return ((Vector4)this).ToString(format, formatProvider);
    }
}
