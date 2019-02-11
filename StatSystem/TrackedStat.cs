using Exanite.Utility;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;

namespace Exanite.StatSystem
{
    /// <summary>
    /// Base class of all TrackedStats
    /// </summary>
    [Serializable]
    public abstract class TrackedStat<T> where T : struct, IComparable, IConvertible, IFormattable
    {
        [HideInInspector, OdinSerialize] private string name;
        [HideInInspector, OdinSerialize] private StatSystem<T> statSystem;

        public event Action<float> ValueChanged;

        /// <summary>
        /// Final value of this <see cref="TrackedStat{T}"/>
        /// </summary>
        public abstract float Value { get; }

        /// <summary>
        /// Name given when this <see cref="TrackedStat{T}"/> was added to a <see cref="StatSystem{T}"/>
        /// </summary>
        [ShowInInspector, ReadOnly]
        public string Name
        {
            get
            {
                return name;
            }

            protected set
            {
                name = value;
            }
        }

        /// <summary>
        /// <see cref="StatSystem{T}"/> this <see cref="TrackedStat{T}"/> is linked to
        /// </summary>
        public StatSystem<T> StatSystem
        {
            get
            {
                return statSystem;
            }

            protected set
            {
                statSystem = value;
            }
        }

        static TrackedStat()
        {
            int init = EnumData<T>.Max; // only used to initialize the EnumData class
        }

        /// <summary>
        /// Method that listens to new modifiers being added to the StatSystem
        /// </summary>
        /// <param name="mod">Added mod</param>
        protected abstract void ModAdded(StatMod<T> mod);

        private void ModAddedInternal(StatMod<T> mod)
        {
            ModAdded(mod);
            ValueChanged?.Invoke(Value);
        }

        /// <summary>
        /// Method that listens to modifiers being removed to the StatSystem
        /// </summary>
        /// <param name="mod">Removed mod</param>
        protected abstract void ModRemoved(StatMod<T> mod);

        private void ModRemovedInternal(StatMod<T> mod)
        {
            ModRemoved(mod);
            ValueChanged?.Invoke(Value);
        }

        /// <summary>
        /// Called internally by the <see cref="StatSystem{T}"/> to link the <see cref="StatSystem{T}"/> with this <see cref="TrackedStat{T}"/> <para/>
        /// Note: When overriding add the '[Obsolete("Internal method, please do not call OnStatSystemAdded unless you know what you are doing")]' attribute
        /// </summary>
        /// <param name="statSystem"></param>
        [Obsolete("Internal method, please do not call OnStatSystemAdded unless you know what you are doing")]
        public virtual void OnStatSystemAdded(StatSystem<T> statSystem, string name)
        {
            if (this.statSystem == null)
            {
                Name = name;
                StatSystem = statSystem;
                statSystem.ModAdded += ModAddedInternal;
                statSystem.ModRemoved += ModRemovedInternal;

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
            if (StatSystem != null)
            {
                Name = null;
                StatSystem = null;
                StatSystem.ModAdded -= ModAddedInternal;
                StatSystem.ModRemoved -= ModRemovedInternal;

                foreach (var item in StatSystem.Modifiers)
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