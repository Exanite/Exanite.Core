using System;
using System.Numerics;
using Exanite.Core.Numerics;

namespace Exanite.Core.Utilities;

public static partial class M
{
    #region Vectors

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

    /// <summary>
    /// Returns the inversed version of the provided matrix, or returns the identity matrix if the provided matrix is not invertible.
    /// </summary>
    public static Matrix3x2 AsInversedOrDefault(this Matrix3x2 value)
    {
        return value.AsInversedOrDefault(Matrix3x2.Identity);
    }

    /// <summary>
    /// Returns the inversed version of the provided matrix, or returns the specified default value if the provided matrix is not invertible.
    /// </summary>
    public static Matrix3x2 AsInversedOrDefault(this Matrix3x2 value, Matrix3x2 defaultValue)
    {
        if (Matrix3x2.Invert(value, out var result))
        {
            return result;
        }

        return defaultValue;
    }

    /// <summary>
    /// Returns the transposed version of the provided matrix.
    /// </summary>
    public static Matrix4x4 AsTransposed(this Matrix4x4 value)
    {
        return Matrix4x4.Transpose(value);
    }

    /// <summary>
    /// Returns the inversed version of the provided matrix, or returns the identity matrix if the provided matrix is not invertible.
    /// </summary>
    public static Matrix4x4 AsInversedOrDefault(this Matrix4x4 value)
    {
        return value.AsInversedOrDefault(Matrix4x4.Identity);
    }

    /// <summary>
    /// Returns the inversed version of the provided matrix, or returns the specified default value if the provided matrix is not invertible.
    /// </summary>
    public static Matrix4x4 AsInversedOrDefault(this Matrix4x4 value, Matrix4x4 defaultValue)
    {
        if (Matrix4x4.Invert(value, out var result))
        {
            return result;
        }

        return defaultValue;
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
        normal = normal.AsNormalizedOrDefault();
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

        if (ApproximatelyEquals(vdot, 0f))
        {
            distance = 0f;

            return false;
        }

        distance = ndot / vdot;

        return distance >= 0 && distance <= ray.Length;
    }

    #endregion
}
