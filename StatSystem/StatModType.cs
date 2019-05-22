namespace Exanite.Core.StatSystem
{
    /// <summary>
    /// How the modifier is applied to existing <see cref="TrackedStat"/>s
    /// </summary>
    [System.Serializable]
    public enum StatModType
    {
        Flat,
        Inc,
        Mult,
    }
}
