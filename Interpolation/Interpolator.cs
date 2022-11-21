namespace Exanite.Core.Interpolation
{
    public abstract class Interpolator<T>
    {
        public T Current;
        public T Previous;

        private float lastPushTime;

        public T Update(float time, float timeBetweenPushes, float extrapolationMultiplier = 0.5f)
        {
            var timeSinceLastPush = time - lastPushTime;
            timeSinceLastPush += extrapolationMultiplier * timeBetweenPushes;

            var t = timeSinceLastPush / timeBetweenPushes;

            return Lerp(Previous, Current, t);
        }

        public void PushNext(T next, float time)
        {
            Previous = Current;
            Current = next;

            lastPushTime = time;
        }

        public abstract T Lerp(T previous, T current, float time);
    }
}