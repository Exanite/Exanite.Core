namespace Exanite.Core.Events
{
    public class EventRaisedRecorder
    {
        public bool IsRaised { get; private set; }

        public void RecordEvent()
        {
            IsRaised = true;
        }
    }
}
