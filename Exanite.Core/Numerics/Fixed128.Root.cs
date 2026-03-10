using System;
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

        // Use Q88.40 for better precision
        // This must be even
        const int internalShift = 40;

        // Normalize x using an even shift so that the shift can be safely halved later
        // This leads to x being in the interval [0.5, 2)
        var leadingZeroCount = (int)Int128.LeadingZeroCount(x.Raw);
        var normalizeShift = (leadingZeroCount - (128 - 1 - internalShift)) & ~1;
        var normalizedX = normalizeShift >= 0 ? x.Raw << normalizeShift : x.Raw >> -normalizeShift;

        // Calculate LUT index of initial guess
        // 2 is represented with (internalShift + 2) bits, but we are exclusive of 2
        const int availableBitCount = internalShift + 2 - 1;
        var lutIndex = (int)(normalizedX >> (availableBitCount - SqrtLutBits));
        var y = (Int128)SqrtLut[lutIndex - SqrtLutOffset] << (internalShift - Fixed.Shift);

        // Inverse Newton-Raphson method:
        // y_n+1 = (y_n * (3 - x * y_n * y_n)) >> 1
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

        // We need to cancel out the normalization step we did above
        // The finalShift was originally 3 shifts
        // This declares the shifts in the order they originally occurred in
        const int shiftDueToMultiplication = internalShift;
        var shiftDueToDenormalization = ((normalizeShift - (internalShift - Shift)) / 2);
        const int shiftFromInternalToOutput = internalShift - Shift;
        var finalShift = shiftDueToMultiplication + shiftDueToDenormalization + shiftFromInternalToOutput;

        var normalizedResult = normalizedX * y;
        var fixed128Value = finalShift >= 0 ? normalizedResult >> finalShift : normalizedResult << -finalShift;
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

        // Handle negative inputs
        var isNegative = IsNegative(x);
        var absX = isNegative ? -x.Raw : x.Raw;

        // Use Q86.42 for better precision
        // This must be a multiple of 3
        const int internalShift = 42;

        // TODO
        // Normalize x to be in the interval [0.125, 1)
        var leadingZeroCount = (int)Int128.LeadingZeroCount(absX);
        var normalizeShift = leadingZeroCount - (128 - 1 - internalShift);
        normalizeShift = normalizeShift % 3;
        if (normalizeShift < 0)
        {
            normalizeShift += 3;
        }

        var normalizedX = normalizeShift >= 0 ? absX << normalizeShift : absX >> -normalizeShift;

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
        var threeReciprocal = ((Int128)1 << (internalShift * 2)) / ((Int128)3 << internalShift);
        const int maxIterationCount = 10; // TODO: Lower this
        for (var i = 0; i < maxIterationCount; i++)
        {
            var yy = (y * y) >> internalShift;
            var xOverYy = (normalizedX << internalShift) / yy;
            var twoY = y << 1;
            var yNext = ((twoY + xOverYy) * threeReciprocal) >> internalShift;
            if (yNext == y)
            {
                break;
            }

            y = yNext;
        }

        // TODO
        // We need to cancel out the normalization step we did above
        // The finalShift was originally 3 shifts
        // This declares the shifts in the order they originally occurred in
        const int shiftDueToMultiplication = internalShift;
        var shiftDueToDenormalization = ((normalizeShift - (internalShift - Shift)) / 2);
        const int shiftFromInternalToOutput = internalShift - Shift;
        var finalShift = shiftDueToMultiplication + shiftDueToDenormalization + shiftFromInternalToOutput;

        var normalizedResult = normalizedX * y;
        var fixed128Value = finalShift >= 0 ? normalizedResult >> finalShift : normalizedResult << -finalShift;
        return new Fixed128(fixed128Value);
    }
}
