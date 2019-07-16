using Exanite.Core.Flags;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using UnityEngine;

namespace Exanite.Core.StatSystem
{
    /// <summary>
    /// Tracks modifiers in a <see cref="StatSystem{T}"/> to produce a Value
    /// </summary>
    [Serializable]
    public class FlaggedTrackedStat<T> : TrackedStat<T> where T : Enum
    {
        [HideInInspector] [OdinSerialize] protected float flatValue = 0f;
        [HideInInspector] [OdinSerialize] protected float incValue = 1f;
        [HideInInspector] [OdinSerialize] protected float multValue = 1f;
        [HideInInspector] [OdinSerialize] protected LongFlag<T> flags;

        /// <summary>
        /// Flat value of the stat
        /// </summary>
        public float FlatValue
        {
            get
            {
                return flatValue;
            }
        }

        /// <summary>
        /// Inc value of the stat
        /// </summary>
        public float IncValue
        {
            get
            {
                return incValue;
            }
        }

        /// <summary>
        /// Mult value of the stat
        /// </summary>
        public float MultValue
        {
            get
            {
                return multValue;
            }
        }

        /// <summary>
        /// Final value of the stat (Flat * Inc * Mult)
        /// </summary>
        [ShowInInspector]
        public override float Value
        {
            get
            {
                return FlatValue * IncValue * MultValue;
            }
        }

        /// <summary>
        /// Creates a <see langword="new"/> <see cref="FlaggedTrackedStat{T}"/>
        /// </summary>
        /// <param name="flags">Flags of this <see cref="FlaggedTrackedStat{T}"/></param>
        public FlaggedTrackedStat(params T[] flags) : base()
        {
            if (flags == null)
            {
                throw new ArgumentNullException(nameof(flags));
            }

            this.flags = new LongFlag<T>(flags);
        }

        /// <summary>
        /// Creates a <see langword="new"/> <see cref="FlaggedTrackedStat{T}"/> <para/>
        /// NOTE: Don't use unless you know what you are doing
        /// </summary>
        /// <param name="flags"><see cref="BitArray"/> of same length as the <see cref="Enum"/></param>
        public FlaggedTrackedStat(BitArray flags) : base()
        {
            if (flags == null)
            {
                throw new ArgumentNullException(nameof(flags));
            }

            this.flags = new LongFlag<T>(flags);
        }

        /// <summary>
        /// Method that listens to new modifiers being added to the StatSystem
        /// </summary>
        /// <param name="mod">Added mod</param>
        protected override void ModAdded(StatMod<T> mod)
        {
            if (!CheckModMatch(mod)) return;

            switch (mod.Type)
            {
                case (StatModType.Flat):
                    flatValue += mod.Value;
                    break;
                case (StatModType.Inc):
                    incValue += mod.Value;
                    break;
                case (StatModType.Mult):
                    multValue *= mod.Value;
                    break;
            }
        }

        /// <summary>
        /// Method that listens to modifiers being removed to the StatSystem
        /// </summary>
        /// <param name="mod">Removed mod</param>
        protected override void ModRemoved(StatMod<T> mod)
        {
            if (!CheckModMatch(mod)) return;

            switch (mod.Type)
            {
                case (StatModType.Flat):
                    flatValue -= mod.Value;
                    break;
                case (StatModType.Inc):
                    incValue -= mod.Value;
                    break;
                case (StatModType.Mult):
                    multValue /= mod.Value;
                    break;
            }
        }

        /// <summary>
        /// Checks if the mod being added/removed matches the flags of this TrackedStat
        /// </summary>
        /// <param name="mod">Modifier to check</param>
        /// <returns>True or false</returns>
        protected virtual bool CheckModMatch(StatMod<T> mod)
        {
            bool hasBaseFlag = mod.Flags.HasFlag(default(T));

            if (hasBaseFlag)
            {
                ((LongFlag<T>)mod.Flags).SetFlag(false, default(T));
            }

            bool isMatch = flags.HasFlags(FlagMatchType.And, ((LongFlag<T>)mod.Flags));

            if (hasBaseFlag)
            {
                ((LongFlag<T>)mod.Flags).SetFlag(true, default(T));
            }

            return isMatch;
        }
    }
}