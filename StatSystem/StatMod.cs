using Exanite.Core.Extensions;
using Exanite.Core.Flags;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;

namespace Exanite.Core.StatSystem
{
    /// <summary>
    /// Class used in the StatSystem to modify existing stats
    /// </summary>
    public sealed class StatMod<T> where T : Enum
    {
        #region Fields and Properties

        private string name;

        [HideInInspector] [OdinSerialize] private float value;
        [HideInInspector] [OdinSerialize] private StatModType type;
        [HideInInspector] [OdinSerialize] private object source;
        [HideInInspector] [OdinSerialize] private LongFlag<T> flags;
        [HideInInspector] [OdinSerialize] private ReadOnlyLongFlag<T> readOnlyFlags;

        /// <summary>
        /// Bool used for caching if the provided type is an enum <para/>
        /// True = is an enum, False = not an enum, Null = Needs to check
        /// </summary>

        /// <summary>
        /// Flags of the StatMod as a string, used in the inspector
        /// </summary>
        [ShowInInspector]
        public string Name
        {
            get
            {
                if(name == null)
                {
                    name = string.Empty;

                    foreach (var item in flags.GetAllTrueFlags())
                    {
                        name += $"{item} ";
                    }

                    name.Trim();
                }

                return name;
            }
        }

        /// <summary>
        /// What flags the modifier has
        /// </summary>
        public ReadOnlyLongFlag<T> Flags
        {
            get
            {
                return readOnlyFlags;
            }
        }

        /// <summary>
        /// Value of the mod
        /// </summary>
        [ShowInInspector]
        public float Value
        {
            get
            {
                return value;
            }

            private set
            {
                this.value = value;
            }
        }

        /// <summary>
        /// How the modifier is applied to existing stats
        /// </summary>
        [ShowInInspector]
        public StatModType Type
        {
            get
            {
                return type;
            }

            private set
            {
                type = value;
            }
        }

        /// <summary>
        /// Where the mod came from
        /// </summary>
        [ShowInInspector]
        public object Source
        {
            get
            {
                return source;
            }

            private set
            {
                source = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new StatMod
        /// </summary>
        /// <param name="value">Value of the mod</param>
        /// <param name="type">How the modifier is applied to existing stats</param>
        /// <param name="source">Where the mod came from, usually "this"</param>
        /// <param name="flags">What flags the modifier has</param>
        public StatMod(float value, StatModType type, object source, params T[] flags)
        {
            if (flags.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(flags));
            }

            Value = value;
            Type = type;
            Source = source;
            this.flags = new LongFlag<T>(flags);
            readOnlyFlags = new ReadOnlyLongFlag<T>(this.flags);
        }

        #endregion
    }
}