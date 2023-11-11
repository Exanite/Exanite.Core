namespace Exanite.Core.HighPerformance
{
    /// <summary>
    /// Stores a value type inside of a reference type.
    /// </summary>
    /// <remarks>
    /// This is mainly intended to be used as a way to avoid repeated GC allocations when storing value types as objects.
    /// </remarks>
    public class BoxedValue<T> where T : struct
    {
        public T Value;

        public BoxedValue(T value)
        {
            Value = value;
        }
    }
}
