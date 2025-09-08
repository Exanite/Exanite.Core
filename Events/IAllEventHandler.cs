namespace Exanite.Core.Events
{
    public interface IAllEventHandler
    {
        // TODO: Figure out how to filter ref structs out from non ref structs. Normal type casting and pattern matching does not work.

        /// <remarks>
        /// Warning: Because of the <c>allows ref struct</c> constraint on .NET Core, you cannot easily interact with the event's value.
        /// Type casting and pattern matching will not work, nor will methods like ToString().
        /// <br/>
        /// Because of this, it's best to treat <see cref="e"/> as an opaque value.
        /// </remarks>
        void OnEvent<T>(T e) where T : allows ref struct;
    }
}
