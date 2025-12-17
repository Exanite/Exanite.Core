using System;
using Exanite.Core.Utilities;

namespace Exanite.Core.Runtime;

/// <summary>
/// Exception thrown by <see cref="AssertUtility"/>.
/// </summary>
/// <remarks>
/// This exception is not meant to be caught.
/// If this exception is thrown, it implies that the entire program
/// is potentially in an invalid state and execution should not continue.
/// </remarks>
public class AssertException : Exception
{
    public AssertException(string message) : base(message) {}
}
