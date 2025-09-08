using System;
using System.Numerics;
using Exanite.Core.Numerics;

namespace Exanite.Core.Utilities;

public static partial class M
{
    #region Vectors

    /// <summary>
    /// Interpolates from one vector to another by <see cref="t"/>.
    /// <see cref="t"/> will be clamped in the range [0, 1]
    /// </summary>
    public static Vector2 Lerp(Vector2 from, Vector2 to, float t)
    {
        t = Clamp(t, 0, 1);
        return from + (to - from) * t;
    }

    /// <summary>
    /// Interpolates from one vector to another by <see cref="t"/>.
    /// <see cref="t"/> will be clamped in the range [0, 1]
    /// </summary>
    public static Vector3 Lerp(Vector3 from, Vector3 to, float t)
    {
        t = Clamp(t, 0, 1);
        return from + (to - from) * t;
    }

    /// <summary>
    /// Interpolates from one vector to another by <see cref="t"/>.
    /// </summary>
    public static Vector2 LerpUnclamped(Vector2 from, Vector2 to, float t)
    {
        return from + (to - from) * t;
    }

    /// <summary>
    /// Interpolates from one vector to another by <see cref="t"/>.
    /// </summary>
    public static Vector3 LerpUnclamped(Vector3 from, Vector3 to, float t)
    {
        return from + (to - from) * t;
    }

    /// <summary>
    /// Clamps the length of the provided vector to between [0, maxLength].
    /// </summary>
    public static Vector2 ClampMagnitude(Vector2 value, float maxLength)
    {
        return ClampMagnitude(value, 0, maxLength);
    }

    /// <summary>
    /// Clamps the length of the provided vector to between [minLength, maxLength].
    /// If a zero vector is provided, then the result is a zero vector.
    /// </summary>
    public static Vector2 ClampMagnitude(Vector2 value, float minLength, float maxLength)
    {
        return value.AsNormalizedSafe() * Clamp(value.Length(), minLength, maxLength);
    }

    /// <summary>
    /// Clamps the length of the provided vector to between [0, maxLength].
    /// </summary>
    public static Vector3 ClampMagnitude(Vector3 value, float maxLength)
    {
        return ClampMagnitude(value, 0, maxLength);
    }

    /// <summary>
    /// Clamps the length of the provided vector to between [minLength, maxLength].
    /// If a zero vector is provided, then the result is a zero vector.
    /// </summary>
    public static Vector3 ClampMagnitude(Vector3 value, float minLength, float maxLength)
    {
        return value.AsNormalizedSafe() * Clamp(value.Length(), minLength, maxLength);
    }

    /// <summary>
    /// Clamps the <see cref="Vector2"/> to the bounds given by
    /// <see cref="min"/> and <see cref="max"/>.
    /// </summary>
    public static void Clamp(ref this Vector2 vector, Vector2 min, Vector2 max)
    {
        vector.X = Clamp(vector.X, min.X, max.X);
        vector.Y = Clamp(vector.Y, min.Y, max.Y);
    }

    /// <summary>
    /// Clamps the <see cref="Vector3"/> to the bounds given by
    /// <see cref="min"/> and <see cref="max"/>.
    /// </summary>
    public static void Clamp(ref this Vector3 vector, Vector3 min, Vector3 max)
    {
        vector.X = Clamp(vector.X, min.X, max.X);
        vector.Y = Clamp(vector.Y, min.Y, max.Y);
        vector.Z = Clamp(vector.Z, min.Z, max.Z);
    }

    /// <summary>
    /// Normalizes the vector.
    /// <br/>
    /// This handles the case where the provided vector is Vector2.Zero,
    /// returning Vector2.Zero rather than NaN.
    /// </summary>
    public static Vector2 AsNormalizedSafe(this Vector2 value)
    {
        return value.AsNormalizedSafe(Vector2.Zero);
    }

    /// <summary>
    /// Normalizes the vector.
    /// <br/>
    /// This handles the case where the provided vector is Vector2.Zero,
    /// returning the provided <see cref="fallback"/> rather than NaN.
    /// </summary>
    public static Vector2 AsNormalizedSafe(this Vector2 value, Vector2 fallback)
    {
        return value == Vector2.Zero ? fallback : Vector2.Normalize(value);
    }

    /// <summary>
    /// Normalizes the vector.
    /// <br/>
    /// This handles the case where the provided vector is Vector3.Zero,
    /// returning Vector3.Zero rather than NaN.
    /// </summary>
    public static Vector3 AsNormalizedSafe(this Vector3 value)
    {
        return value.AsNormalizedSafe(Vector3.Zero);
    }

    /// <summary>
    /// Normalizes the vector.
    /// <br/>
    /// This handles the case where the provided vector is Vector3.Zero,
    /// returning the provided <see cref="fallback"/> rather than NaN.
    /// </summary>
    public static Vector3 AsNormalizedSafe(this Vector3 value, Vector3 fallback)
    {
        return value == Vector3.Zero ? fallback : Vector3.Normalize(value);
    }

    /// <summary>
    /// Checks if two vectors are approximately the same value based on the provided <see cref="tolerance"/>.
    /// </summary>
    public static bool IsApproximatelyEqual(Vector2 a, Vector2 b, float tolerance = 0.000001f)
    {
        return IsApproximatelyEqual(a.X, b.X, tolerance) && IsApproximatelyEqual(a.Y, b.Y, tolerance);
    }

    /// <summary>
    /// Checks if two vectors are approximately the same value based on the provided <see cref="tolerance"/>.
    /// </summary>
    public static bool IsApproximatelyEqual(Vector3 a, Vector3 b, float tolerance = 0.000001f)
    {
        return IsApproximatelyEqual(a.X, b.X, tolerance) && IsApproximatelyEqual(a.Y, b.Y, tolerance) && IsApproximatelyEqual(a.Z, b.Z, tolerance);
    }

    /// <summary>
    /// Swaps the component values of a <see cref="Vector3"/> from XYZ to
    /// the given format.
    /// </summary>
    public static Vector3 Swizzle(this Vector3 vector, Vector3Swizzle swizzle)
    {
        return swizzle switch
        {
            Vector3Swizzle.Xyz => vector,
            Vector3Swizzle.Xzy => new Vector3(vector.X, vector.Z, vector.Y),
            Vector3Swizzle.Yxz => new Vector3(vector.Y, vector.X, vector.Z),
            Vector3Swizzle.Yzx => new Vector3(vector.Y, vector.Z, vector.X),
            Vector3Swizzle.Zxy => new Vector3(vector.Z, vector.X, vector.Y),
            Vector3Swizzle.Zyx => new Vector3(vector.Z, vector.Y, vector.X),
            _ => throw ExceptionUtility.NotSupportedEnumValue(swizzle),
        };
    }

    /// <summary>
    /// Opposite of Swizzle. Swaps the component values of a
    /// <see cref="Vector3"/> in the given format back to XYZ.
    /// </summary>
    public static Vector3 InverseSwizzle(this Vector3 vector, Vector3Swizzle swizzle)
    {
        return swizzle switch
        {
            Vector3Swizzle.Xyz => vector,
            Vector3Swizzle.Xzy => new Vector3(vector.X, vector.Z, vector.Y),
            Vector3Swizzle.Yxz => new Vector3(vector.Y, vector.X, vector.Z),
            Vector3Swizzle.Yzx => new Vector3(vector.Z, vector.X, vector.Y),
            Vector3Swizzle.Zxy => new Vector3(vector.Y, vector.Z, vector.X),
            Vector3Swizzle.Zyx => new Vector3(vector.Z, vector.Y, vector.X),
            _ => throw ExceptionUtility.NotSupportedEnumValue(swizzle),
        };
    }

    #endregion

    #region Vector Conversion

    // Vector component count conversion
    // The goal is to keep these conversions minimal
    // so only the most common cases are handled

    // Vector2 <- Vector3

    /// <summary>
    /// Converts a Vector3 into a Vector2 by dropping the Z component.
    /// </summary>
    public static Vector2 Xy(this Vector3 value)
    {
        return new Vector2(value.X, value.Y);
    }

    // Vector2 -> Vector3

    /// <summary>
    /// Converts a Vector2 into a Vector3 using 0 for the Z component.
    /// </summary>
    public static Vector3 Xy0(this Vector2 value)
    {
        return new Vector3(value.X, value.Y, 0);
    }

    /// <summary>
    /// Converts a Vector2 into a Vector3 using 1 for the Z component.
    /// </summary>
    public static Vector3 Xy1(this Vector2 value)
    {
        return new Vector3(value.X, value.Y, 1);
    }

    // Vector3 <- Vector4

    /// <summary>
    /// Converts a Vector4 into a Vector3 by dropping the W component.
    /// </summary>
    public static Vector3 Xyz(this Vector4 value)
    {
        return new Vector3(value.X, value.Y, value.Z);
    }

    // Vector3 -> Vector4

    /// <summary>
    /// Converts a Vector3 into a Vector4 using 0 for the Z component.
    /// </summary>
    public static Vector4 Xyz0(this Vector3 value)
    {
        return new Vector4(value.X, value.Y, value.Z, 0);
    }

    /// <summary>
    /// Converts a Vector3 into a Vector4 using 1 for the Z component.
    /// </summary>
    public static Vector4 Xyz1(this Vector3 value)
    {
        return new Vector4(value.X, value.Y, value.Z, 1);
    }

    // Vector2Int <- Vector3Int

    /// <summary>
    /// Converts a Vector3Int into a Vector2Int by dropping the Z component.
    /// </summary>
    public static Vector2Int Xy(this Vector3Int value)
    {
        return new Vector2Int(value.X, value.Y);
    }

    // Vector2Int -> Vector3Int

    /// <summary>
    /// Converts a Vector2Int into a Vector3Int using 0 for the Z component.
    /// </summary>
    public static Vector3Int Xy0(this Vector2Int value)
    {
        return new Vector3Int(value.X, value.Y, 0);
    }

    /// <summary>
    /// Converts a Vector2Int into a Vector3Int using 1 for the Z component.
    /// </summary>
    public static Vector3Int Xy1(this Vector2Int value)
    {
        return new Vector3Int(value.X, value.Y, 1);
    }

    #endregion

    #region Matrices

    /// <summary>
    /// Same as <see cref="Matrix4x4.CreateOrthographic"/>, but allows for reversed near and far planes.
    /// </summary>
    public static Matrix4x4 CreateOrthographic(float width, float height, float nearPlane, float farPlane)
    {
        var range = 1 / (nearPlane - farPlane);

        return new Matrix4x4(
            2 / width, 0, 0, 0,
            0, 2 / height, 0, 0,
            0, 0, range, 0,
            0, 0, range * nearPlane, 1
        );
    }

    /// <summary>
    /// Same as <see cref="Matrix4x4.CreatePerspectiveFieldOfView"/>, but allows for reversed near and far planes.
    /// </summary>
    public static Matrix4x4 CreatePerspective(float fieldOfView, float aspectRatio, float nearPlane, float farPlane)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(fieldOfView, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(fieldOfView, float.Pi);

        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nearPlane, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(farPlane, 0);

        var height = 1 / float.Tan(fieldOfView * 0.5f);
        var width = height / aspectRatio;
        var range = float.IsPositiveInfinity(farPlane) ? -1 : farPlane / (nearPlane - farPlane);

        return new Matrix4x4(
            width, 0, 0, 0,
            0, height, 0, 0,
            0, 0, range, -1,
            0, 0, range * nearPlane, 0
        );
    }

    #endregion

    #region Planes

    /// <summary>
    /// Creates a plane from a position and a normal.
    /// </summary>
    /// <remarks>
    /// .NET doesn't provide this method of construction for some reason.
    /// </remarks>
    public static Plane CreatePlane(Vector3 normal, Vector3 position)
    {
        normal = normal.AsNormalizedSafe();
        var distance = -Vector3.Dot(normal, position);

        return new Plane(normal, distance);
    }

    /// <summary>
    /// Casts a ray against the specified plane.
    /// </summary>
    public static bool Raycast(this Plane plane, Ray ray, out float distance)
    {
        var vdot = Vector3.Dot(ray.Direction, plane.Normal);
        var ndot = -Vector3.Dot(ray.Origin, plane.Normal) - plane.D;

        if (IsApproximatelyEqual(vdot, 0f))
        {
            distance = 0f;

            return false;
        }

        distance = ndot / vdot;

        return distance >= 0 && distance <= ray.Length;
    }

    #endregion
}
