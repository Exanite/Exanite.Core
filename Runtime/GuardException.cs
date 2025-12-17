using System;
using Exanite.Core.Utilities;

namespace Exanite.Core.Runtime;

/// <summary>
/// Exception thrown by <see cref="GuardUtility"/>.
/// </summary>
/// <remarks>
/// This exception is not meant to be caught.
/// If this exception is thrown, it implies that the entire program
/// is potentially in an invalid state and execution should not continue.
/// </remarks>
public class GuardException : Exception
{
    public GuardException(string message) : base(message) {}
}
