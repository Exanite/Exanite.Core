using System;
using System.Diagnostics;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

public partial struct Fixed128
{
    // Implementation note:
    // The fast methods are called "fast" because they are designed
    // to be used for Q48.16, which does not need as much precision

    public static Fixed128 SqrtFast(Fixed128 x)
    {
        if (x < 0)
        {
            GuardUtility.Throw("Cannot take the square root of a negative number");
        }

        if (x == 0)
        {
            return 0;
        }

        // Use Q86.42 for better precision
        // This must be even
        const int internalShift = 42;
        const int shiftInputToInternal = internalShift - Shift;

        // Normalize x using an even shift so that the shift can be safely halved later
        // This leads to x being in the interval [0.5, 2)
        var leadingZeroCount = (int)Int128.LeadingZeroCount(x.Raw) - shiftInputToInternal;
        var distanceToOneBit = leadingZeroCount - (BitCount - 1 - internalShift);
        var shiftNormalize = distanceToOneBit & ~1;
        var shiftInitial = shiftInputToInternal + shiftNormalize;

        var normalizedX = shiftInitial < 0 ? x.Raw >> -shiftInitial : x.Raw << shiftInitial;
        AssertExpectedRange(normalizedX, internalShift, 0.5M, 2M);

        // Calculate LUT index of initial guess
        // 2 is represented with (internalShift + 2) bits, but we are exclusive of 2
        const int availableBitCount = internalShift + 2 - 1;
        var lutIndex = (int)(normalizedX >> (availableBitCount - SqrtLutBits));
        var y = (Int128)SqrtLut[lutIndex - SqrtLutOffset] << (internalShift - Fixed.Shift);

        // Inverse Newton-Raphson method:
        // y = (y * (3 - x * y * y)) >> 1
        // y = 1 / sqrt(x)
        var three = (Int128)3 << internalShift;
        const int maxIterationCount = 3;
        for (var i = 0; i < maxIterationCount; i++)
        {
            var xyy = (normalizedX * y * y) >> (internalShift * 2);
            var threeMinusXyy = three - xyy;
            var yNext = (y * threeMinusXyy) >> (internalShift + 1);
            if (yNext == y)
            {
                break;
            }

            y = yNext;
        }

        // Cancel out normalization and conversion to internal
        const int shiftMultiplication = internalShift;
        var shiftDenormalize = shiftNormalize / 2;
        const int shiftInternalToOutput = internalShift - Shift;
        var shiftFinal = shiftMultiplication + shiftDenormalize + shiftInternalToOutput;

        var normalizedResult = normalizedX * y;
        var fixed128Value = shiftFinal < 0 ? normalizedResult << -shiftFinal : normalizedResult >> shiftFinal;
        return new Fixed128(fixed128Value);
    }

    public static Fixed128 CbrtFast(Fixed128 x)
    {
        // This algorithm is adapted from SqrtFast
        // Key differences:
        // Normalize shift is a multiple of 3 instead of 2
        // Use of direct Newton-Raphson (instead of the inverse) to avoid overflow when cubing y

        if (x == 0)
        {
            return 0;
        }

        // Use Q86.42 for better precision
        // This must be a multiple of 3
        const int internalShift = 42;
        const int shiftInputToInternal = internalShift - Shift;

        // Handle negative inputs
        var isNegative = IsNegative(x);
        var absX = isNegative ? -x.Raw : x.Raw;

        // Normalize x to be in the interval [0.25, 2)
        var leadingZeroCount = (int)Int128.LeadingZeroCount(absX) - shiftInputToInternal;
        var distanceToOneBit = leadingZeroCount - (BitCount - 1 - internalShift);
        var shiftNormalize = distanceToOneBit - (distanceToOneBit % 3 + 3) % 3;
        var shiftInitial = shiftInputToInternal + shiftNormalize;

        var normalizedX = shiftInitial < 0 ? absX >> -shiftInitial : absX << shiftInitial;
        AssertExpectedRange(normalizedX, internalShift, 0.25M, 2M);

        // TODO
        // Calculate LUT index of initial guess
        // 1 is represented with (internalShift + 1) bits, but we are exclusive of 1
        // const int availableBitCount = internalShift + 1 - 1;
        // var lutIndex = (int)(normalizedX >> (availableBitCount - SqrtLutBits));
        // var y = (Int128)SqrtLut[lutIndex - SqrtLutOffset] << (internalShift - Fixed.Shift);

        // TODO: Temporary initial guess
        var y = (Int128)1 << internalShift;

        // Direct Newton-Raphson method:
        // y = (2y + x/y^2) / 3
        // y = cbrt(x)
        var threeReciprocal = ((Int128)1 << (internalShift * 2)) / ((Int128)3 << internalShift);
        const int maxIterationCount = 7; // TODO: Lower this
        for (var i = 0; i < maxIterationCount; i++)
        {
            var yy = y * y;
            var xOverYy = (normalizedX << (internalShift * 2)) / yy;
            var twoY = y << 1;
            var yNext = ((twoY + xOverYy) * threeReciprocal) >> internalShift;
            if (yNext == y)
            {
                break;
            }

            AssertUtility.IsFalse(i == maxIterationCount - 1, "Didn't converge"); // TODO: Remove. This technically restricts iterations to maxIterationsCount - 1
            y = yNext;
        }

        // Cancel out normalization and conversion to internal
        var shiftDenormalize = shiftNormalize / 3;
        const int shiftInternalToOutput = internalShift - Shift;
        var shiftFinal = shiftDenormalize + shiftInternalToOutput;

        var normalizedResult = y;
        var fixed128Value = shiftFinal < 0 ? normalizedResult << -shiftFinal : normalizedResult >> shiftFinal;
        fixed128Value = isNegative ? -fixed128Value : fixed128Value;
        return new Fixed128(fixed128Value);
    }

    [Conditional("DEBUG")]
    private static void AssertExpectedRange(Int128 x, int shift, decimal inclusiveMin, decimal exclusiveMax)
    {
        AssertUtility.IsFalse((decimal)x / (1L << shift) < inclusiveMin, "Internal: Value is less than the required minimum");
        AssertUtility.IsFalse((decimal)x / (1L << shift) >= exclusiveMax, "Internal: Value is greater than the required maximum");
    }
}
