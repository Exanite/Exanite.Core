using Exanite.Utility;
using Sirenix.Serialization;
using System;
using UnityEngine;

namespace Exanite.StatSystem
{
    /// <summary>
    /// Tracks modifiers and other tracked stats to produce a FinalValue
    /// </summary>
    [Serializable]
    public abstract class TrackedStat<T> where T : struct, IComparable, IConvertible, IFormattable
    {
        [HideInInspector] [OdinSerialize] protected StatSystem<T> statSystem;

        protected static EnumData<T> enumData;

        /// <summary>
        /// Final value of this <see cref="TrackedStat{T}"/>
        /// </summary>
        public abstract float Value { get; }

        protected TrackedStat()
        {
            if (enumData == null) enumData = EnumData<T>.Instance;
        }

        /// <summary>
        /// Method that listens to new modifiers being added to the StatSystem
        /// </summary>
        /// <param name="mod">Added mod</param>
        protected abstract void ModAdded(StatMod<T> mod);

        /// <summary>
        /// Method that listens to modifiers being removed to the StatSystem
        /// </summary>
        /// <param name="mod">Removed mod</param>
        protected abstract void ModRemoved(StatMod<T> mod);

        /// <summary>
        /// Called internally by the <see cref="StatSystem{T}"/> to link the <see cref="StatSystem{T}"/> with this <see cref="TrackedStat{T}"/> <para/>
        /// Note: When overriding add the '[Obsolete("Internal method, please do not call OnStatSystemAdded unless you know what you are doing")]' attribute
        /// </summary>
        /// <param name="statSystem"></param>
        [Obsolete("Internal method, please do not call OnStatSystemAdded unless you know what you are doing")]
        public virtual void OnStatSystemAdded(StatSystem<T> statSystem)
        {
            if (statSystem == null)
            {
                this.statSystem = statSystem;
                statSystem.ModAdded += ModAdded;
                statSystem.ModRemoved += ModRemoved;

                foreach (var item in statSystem.Modifiers)
                {
                    ModAdded(item);
                }
            }
            else
            {
                throw new AlreadyLinkedException();
            }
        }

        /// <summary>
        /// Called internally by the <see cref="StatSystem{T}"/> to unlink the <see cref="StatSystem{T}"/> from this <see cref="TrackedStat{T}"/> <para/>
        /// Note: When overriding add the '[Obsolete("Internal method, please do not call OnStatSystemRemoved unless you know what you are doing")]' attribute
        /// </summary>
        [Obsolete("Internal method, please do not call OnStatSystemRemoved unless you know what you are doing")]
        public virtual void OnStatSystemRemoved()
        {
            if (statSystem != null)
            {
                statSystem = null;
                statSystem.ModAdded -= ModAdded;
                statSystem.ModRemoved -= ModRemoved;

                foreach (var item in statSystem.Modifiers)
                {
                    ModRemoved(item);
                }
            }
        }

        /// <summary>
        /// <see cref="Exception"/> thrown when attempting to add an already linked <see cref="TrackedStat{T}"/> to a second <see cref="StatSystem{T}"/>
        /// </summary>
        [Serializable]
        public class AlreadyLinkedException : Exception
        {
            public AlreadyLinkedException() : base($"This {nameof(TrackedStat<T>)} has already been linked with another {nameof(StatSystem<T>)}") { }
        }
    }
}