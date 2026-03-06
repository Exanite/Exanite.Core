#nullable enable

using System;
using System.Numerics;
using Exanite.Core.Numerics;

namespace Exanite.Core.Utilities;

public static partial class M
{
    extension(Matrix4x4)
    {
        /// <summary>
        /// Creates a matrix for skewing positions on the X-axis based on the Y-axis.
        /// </summary>
        public static Matrix4x4 CreateSkewXWithY(float amount)
        {
            var k = amount;
            return new Matrix4x4(
                1, k, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            );
        }

        /// <summary>
        /// Creates a matrix for skewing positions on the X-axis based on the Z-axis.
        /// </summary>
        public static Matrix4x4 CreateSkewXWithZ(float amount)
        {
            var k = amount;
            return new Matrix4x4(
                1, 0, k, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            );
        }

        /// <summary>
        /// Creates a matrix for skewing positions on the X-axis based on the W-axis.
        /// </summary>
        public static Matrix4x4 CreateSkewXWithW(float amount)
        {
            var k = amount;
            return new Matrix4x4(
                1, 0, 0, k,
                0, 1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            );
        }

        /// <summary>
        /// Creates a matrix for skewing positions on the Y-axis based on the X-axis.
        /// </summary>
        public static Matrix4x4 CreateSkewYWithX(float amount)
        {
            var k = amount;
            return new Matrix4x4(
                1, 0, 0, 0,
                k, 1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            );
        }

        /// <summary>
        /// Creates a matrix for skewing positions on the Y-axis based on the Z-axis.
        /// </summary>
        public static Matrix4x4 CreateSkewYWithZ(float amount)
        {
            var k = amount;
            return new Matrix4x4(
                1, 0, 0, 0,
                0, 1, k, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            );
        }

        /// <summary>
        /// Creates a matrix for skewing positions on the Y-axis based on the W-axis.
        /// </summary>
        public static Matrix4x4 CreateSkewYWithW(float amount)
        {
            var k = amount;
            return new Matrix4x4(
                1, 0, 0, 0,
                0, 1, 0, k,
                0, 0, 1, 0,
                0, 0, 0, 1
            );
        }

        /// <summary>
        /// Creates a matrix for skewing positions on the Z-axis based on the X-axis.
        /// </summary>
        public static Matrix4x4 CreateSkewZWithX(float amount)
        {
            var k = amount;
            return new Matrix4x4(
                1, 0, 0, 0,
                0, 1, 0, 0,
                k, 0, 1, 0,
                0, 0, 0, 1
            );
        }

        /// <summary>
        /// Creates a matrix for skewing positions on the Z-axis based on the Y-axis.
        /// </summary>
        public static Matrix4x4 CreateSkewZWithY(float amount)
        {
            var k = amount;
            return new Matrix4x4(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, k, 1, 0,
                0, 0, 0, 1
            );
        }

        /// <summary>
        /// Creates a matrix for skewing positions on the Z-axis based on the W-axis.
        /// </summary>
        public static Matrix4x4 CreateSkewZWithW(float amount)
        {
            var k = amount;
            return new Matrix4x4(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, k,
                0, 0, 0, 1
            );
        }

        /// <summary>
        /// Creates a matrix for skewing positions on the W-axis based on the X-axis.
        /// </summary>
        public static Matrix4x4 CreateSkewWWithX(float amount)
        {
            var k = amount;
            return new Matrix4x4(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                k, 0, 0, 1
            );
        }

        /// <summary>
        /// Creates a matrix for skewing positions on the W-axis based on the Y-axis.
        /// </summary>
        public static Matrix4x4 CreateSkewWWithY(float amount)
        {
            var k = amount;
            return new Matrix4x4(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                0, k, 0, 1
            );
        }

        /// <summary>
        /// Creates a matrix for skewing positions on the W-axis based on the Z-axis.
        /// </summary>
        public static Matrix4x4 CreateSkewWWithZ(float amount)
        {
            var k = amount;
            return new Matrix4x4(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                0, 0, k, 1
            );
        }

    }
}
