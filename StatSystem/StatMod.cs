using System;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using Sirenix.Serialization;
using Exanite.Utility;
using Exanite.Flags;

namespace Exanite.StatSystem
{
    /// <summary>
    /// Class used in the StatSystem to modify existing stats
    /// </summary>
    public class StatMod<T> where T : struct, IComparable, IConvertible, IFormattable
    {
        #region Fields and Properties

        protected string name;

        [HideInInspector] [OdinSerialize] protected float value;
        [HideInInspector] [OdinSerialize] protected StatModType type;
        [HideInInspector] [OdinSerialize] protected object source;
        [HideInInspector] [OdinSerialize] protected LongFlag<T> flags;

        protected bool? isEnum = null;

        /// <summary>
        /// Automatically generated name for this modifier
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        public string Name
        {
            get
            {
                if(string.IsNullOrEmpty(name))
                {
                    foreach(T flag in Flags.GetAllTrueFlags())
                    {
                        name += $"{flag} ";
                        name.Trim();
                    }
                }
                return name;
            }

            protected set
            {
                name = value;
            }
        }

        /// <summary>
        /// What flags the modifier has
        /// </summary>
        public LongFlag<T> Flags
        {
            get
            {
                return flags;
            }

            protected set
            {
                flags = value;
            }
        }
        /// <summary>
        /// Value of the mod
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        public float Value
        {
            get
            {
                return value;
            }

            protected set
            {
                this.value = value;
            }
        }
        /// <summary>
        /// How the modifier is applied to existing stats
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        public StatModType Type
        {
            get
            {
                return type;
            }

            protected set
            {
                type = value;
            }
        }
        /// <summary>
        /// Where the mod came from
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        public object Source
        {
            get
            {
                return source;
            }

            protected set
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

            switch(isEnum)
            {
                case (null):
                    isEnum = typeof(T).IsEnum;
                    if(isEnum == false)
                    {
                        throw new ArgumentException(string.Format("{0} is not an Enum Type", typeof(T)));
                    }
                    break;
                case (true):
                    break;
                case (false):
                    throw new ArgumentException(string.Format("{0} is not an Enum Type", typeof(T)));
            }

            Value = value;
            Type = type;
            Source = source;
            Flags = new LongFlag<T>(flags);
        }

        #endregion

        #region Matching

        /// <summary>
        /// Returns true if the LongFlag provided matches the flags of this modifier
        /// </summary>
        /// <param name="mod">LongFlag to compare</param>
        /// <returns>True or false</returns>
        public virtual bool IsMatch(LongFlag<T> flags)
        {
            bool hasBaseFlag = Flags.HasFlag(default(T));

            if (hasBaseFlag)
            {
                Flags.SetFlag(false, default(T));
            }

            bool matchSuccess = flags.HasFlags(FlagMatchType.And, Flags);

            if (hasBaseFlag)
            {
                Flags.SetFlag(true, default(T));
            }

            return matchSuccess;
        }

        #endregion
    }
}