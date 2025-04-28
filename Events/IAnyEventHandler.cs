// This is the Unity specific implementation of the EventBus.
// See Exanite.Core.CsLatest for the general .NET version.
#if UNITY_2021_3_OR_NEWER
namespace Exanite.Core.Events
{
    public interface IAnyEventHandler
    {
        void OnAnyEvent<T>(T e);
    }
}
#endif
