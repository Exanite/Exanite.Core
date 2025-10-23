#nullable enable

using System;
using System.Numerics;
using Exanite.Core.Numerics;

namespace Exanite.Core.Utilities;

public static partial class M
{
    /// <summary>
    /// Interpolates from one vector to another by <see cref="t"/>.
    /// <see cref="t"/> will be clamped in the range [0, 1]
    /// </summary>
    public static Vector2 Lerp(Vector2 from, Vector2 to, float t)
    {
        t = Clamp01(t);
        return from + (to - from) * t;
    }

    /// <summary>
    /// Interpolates from one vector to another by <see cref="t"/>.
    /// </summary>
    public static Vector2 LerpUnclamped(Vector2 from, Vector2 to, float t)
    {
        return from + (to - from) * t;
    }

    /// <inheritdoc cref="SmoothDamp{T}"/>/>
    public static Vector2 SmoothDamp(Vector2 current, Vector2 target, float smoothTime, float deltaTime, ref Vector2 currentVelocity, float maxSpeed = float.PositiveInfinity)
    {
        var result = new Vector2(SmoothDamp(current.X, target.X, smoothTime, deltaTime, ref currentVelocity.X, maxSpeed), SmoothDamp(current.Y, target.Y, smoothTime, deltaTime, ref currentVelocity.Y, maxSpeed));
        currentVelocity = ClampMagnitude(currentVelocity, maxSpeed);

        return result;
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
    /// Component-wise clamps the provided vector to the bounds given by <see cref="min"/> and <see cref="max"/>.
    /// </summary>
    public static void Clamp(ref this Vector2 vector, Vector2 min, Vector2 max)
    {
        vector.X = Clamp(vector.X, min.X, max.X);
        vector.X = Clamp(vector.Y, min.Y, max.Y);
    }

    /// <summary>
    /// Normalizes the vector.
    /// <br/>
    /// This handles the case where the provided vector is zero,
    /// returning zero rather than NaN.
    /// </summary>
    public static Vector2 AsNormalizedSafe(this Vector2 value)
    {
        return value.AsNormalizedSafe(Vector2.Zero);
    }

    /// <summary>
    /// Normalizes the vector.
    /// <br/>
    /// This handles the case where the provided vector is zero,
    /// returning the provided <see cref="fallback"/> rather than NaN.
    /// </summary>
    public static Vector2 AsNormalizedSafe(this Vector2 value, Vector2 fallback)
    {
        return value == Vector2.Zero ? fallback : Vector2.Normalize(value);
    }

    /// <summary>
    /// Checks if two vectors are approximately the same value based on the provided <see cref="tolerance"/>.
    /// </summary>
    public static bool ApproximatelyEquals(Vector2 a, Vector2 b, float tolerance = 0.000001f)
    {
        return ApproximatelyEquals(a.X, b.X, tolerance) && ApproximatelyEquals(a.Y, b.Y, tolerance);
    }
    /// <summary>
    /// Interpolates from one vector to another by <see cref="t"/>.
    /// <see cref="t"/> will be clamped in the range [0, 1]
    /// </summary>
    public static Vector3 Lerp(Vector3 from, Vector3 to, float t)
    {
        t = Clamp01(t);
        return from + (to - from) * t;
    }

    /// <summary>
    /// Interpolates from one vector to another by <see cref="t"/>.
    /// </summary>
    public static Vector3 LerpUnclamped(Vector3 from, Vector3 to, float t)
    {
        return from + (to - from) * t;
    }

    /// <inheritdoc cref="SmoothDamp{T}"/>/>
    public static Vector3 SmoothDamp(Vector3 current, Vector3 target, float smoothTime, float deltaTime, ref Vector3 currentVelocity, float maxSpeed = float.PositiveInfinity)
    {
        var result = new Vector3(SmoothDamp(current.X, target.X, smoothTime, deltaTime, ref currentVelocity.X, maxSpeed), SmoothDamp(current.Y, target.Y, smoothTime, deltaTime, ref currentVelocity.Y, maxSpeed), SmoothDamp(current.Z, target.Z, smoothTime, deltaTime, ref currentVelocity.Z, maxSpeed));
        currentVelocity = ClampMagnitude(currentVelocity, maxSpeed);

        return result;
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
    /// Component-wise clamps the provided vector to the bounds given by <see cref="min"/> and <see cref="max"/>.
    /// </summary>
    public static void Clamp(ref this Vector3 vector, Vector3 min, Vector3 max)
    {
        vector.X = Clamp(vector.X, min.X, max.X);
        vector.X = Clamp(vector.Y, min.Y, max.Y);
        vector.X = Clamp(vector.Z, min.Z, max.Z);
    }

    /// <summary>
    /// Normalizes the vector.
    /// <br/>
    /// This handles the case where the provided vector is zero,
    /// returning zero rather than NaN.
    /// </summary>
    public static Vector3 AsNormalizedSafe(this Vector3 value)
    {
        return value.AsNormalizedSafe(Vector3.Zero);
    }

    /// <summary>
    /// Normalizes the vector.
    /// <br/>
    /// This handles the case where the provided vector is zero,
    /// returning the provided <see cref="fallback"/> rather than NaN.
    /// </summary>
    public static Vector3 AsNormalizedSafe(this Vector3 value, Vector3 fallback)
    {
        return value == Vector3.Zero ? fallback : Vector3.Normalize(value);
    }

    /// <summary>
    /// Checks if two vectors are approximately the same value based on the provided <see cref="tolerance"/>.
    /// </summary>
    public static bool ApproximatelyEquals(Vector3 a, Vector3 b, float tolerance = 0.000001f)
    {
        return ApproximatelyEquals(a.X, b.X, tolerance) && ApproximatelyEquals(a.Y, b.Y, tolerance) && ApproximatelyEquals(a.Z, b.Z, tolerance);
    }
    /// <summary>
    /// Interpolates from one vector to another by <see cref="t"/>.
    /// <see cref="t"/> will be clamped in the range [0, 1]
    /// </summary>
    public static Vector4 Lerp(Vector4 from, Vector4 to, float t)
    {
        t = Clamp01(t);
        return from + (to - from) * t;
    }

    /// <summary>
    /// Interpolates from one vector to another by <see cref="t"/>.
    /// </summary>
    public static Vector4 LerpUnclamped(Vector4 from, Vector4 to, float t)
    {
        return from + (to - from) * t;
    }

    /// <inheritdoc cref="SmoothDamp{T}"/>/>
    public static Vector4 SmoothDamp(Vector4 current, Vector4 target, float smoothTime, float deltaTime, ref Vector4 currentVelocity, float maxSpeed = float.PositiveInfinity)
    {
        var result = new Vector4(SmoothDamp(current.X, target.X, smoothTime, deltaTime, ref currentVelocity.X, maxSpeed), SmoothDamp(current.Y, target.Y, smoothTime, deltaTime, ref currentVelocity.Y, maxSpeed), SmoothDamp(current.Z, target.Z, smoothTime, deltaTime, ref currentVelocity.Z, maxSpeed), SmoothDamp(current.W, target.W, smoothTime, deltaTime, ref currentVelocity.W, maxSpeed));
        currentVelocity = ClampMagnitude(currentVelocity, maxSpeed);

        return result;
    }

    /// <summary>
    /// Clamps the length of the provided vector to between [0, maxLength].
    /// </summary>
    public static Vector4 ClampMagnitude(Vector4 value, float maxLength)
    {
        return ClampMagnitude(value, 0, maxLength);
    }

    /// <summary>
    /// Clamps the length of the provided vector to between [minLength, maxLength].
    /// If a zero vector is provided, then the result is a zero vector.
    /// </summary>
    public static Vector4 ClampMagnitude(Vector4 value, float minLength, float maxLength)
    {
        return value.AsNormalizedSafe() * Clamp(value.Length(), minLength, maxLength);
    }

    /// <summary>
    /// Component-wise clamps the provided vector to the bounds given by <see cref="min"/> and <see cref="max"/>.
    /// </summary>
    public static void Clamp(ref this Vector4 vector, Vector4 min, Vector4 max)
    {
        vector.X = Clamp(vector.X, min.X, max.X);
        vector.X = Clamp(vector.Y, min.Y, max.Y);
        vector.X = Clamp(vector.Z, min.Z, max.Z);
        vector.X = Clamp(vector.W, min.W, max.W);
    }

    /// <summary>
    /// Normalizes the vector.
    /// <br/>
    /// This handles the case where the provided vector is zero,
    /// returning zero rather than NaN.
    /// </summary>
    public static Vector4 AsNormalizedSafe(this Vector4 value)
    {
        return value.AsNormalizedSafe(Vector4.Zero);
    }

    /// <summary>
    /// Normalizes the vector.
    /// <br/>
    /// This handles the case where the provided vector is zero,
    /// returning the provided <see cref="fallback"/> rather than NaN.
    /// </summary>
    public static Vector4 AsNormalizedSafe(this Vector4 value, Vector4 fallback)
    {
        return value == Vector4.Zero ? fallback : Vector4.Normalize(value);
    }

    /// <summary>
    /// Checks if two vectors are approximately the same value based on the provided <see cref="tolerance"/>.
    /// </summary>
    public static bool ApproximatelyEquals(Vector4 a, Vector4 b, float tolerance = 0.000001f)
    {
        return ApproximatelyEquals(a.X, b.X, tolerance) && ApproximatelyEquals(a.Y, b.Y, tolerance) && ApproximatelyEquals(a.Z, b.Z, tolerance) && ApproximatelyEquals(a.W, b.W, tolerance);
    }
}
