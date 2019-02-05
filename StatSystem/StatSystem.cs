using Exanite.Flags;
using Exanite.Utility;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Exanite.StatSystem
{
    /// <summary>
    /// Handles everything you need for a stat system
    /// </summary>
    [Serializable]
    public class StatSystem<T> where T : struct, IComparable, IConvertible, IFormattable
    {
        #region Fields and Properties

        /// <summary>
        /// Modifiers of this <see cref="StatSystem{T}"/>
        /// </summary>
        [OdinSerialize]
        [ReadOnly]
        [TabGroup("Modifiers")]
        public readonly List<StatMod<T>> Modifiers;

        [OdinSerialize]
        [ReadOnly]
        [TabGroup("Tracked Stats")]
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
        protected Dictionary<string, TrackedStat<T>> trackedStats;

        protected static LongFlag<T> flagCache = new LongFlag<T>();

        /// <summary>
        /// Called when a mod is added
        /// </summary>
        [HideInInspector] public Action<StatMod<T>> ModAdded;
        /// <summary>
        /// Called when a mod is removed
        /// </summary>
        [HideInInspector] public Action<StatMod<T>> ModRemoved;

        protected static EnumData<T> enumData;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a <see langword="new"/> <see cref="StatSystem{T}"/>
        /// </summary>
        public StatSystem()
        {
            if (enumData == null) enumData = EnumData<T>.Instance;

            Modifiers = new List<StatMod<T>>();
            trackedStats = new Dictionary<string, TrackedStat<T>>();
        }

        #endregion

        #region Modifiers

        /// <summary>
        /// Adds a modifier to this <see cref="StatSystem{T}"/>
        /// </summary>
        /// <param name="mod">Mod to add</param>
        public virtual void AddModifier(StatMod<T> mod)
        {
            Modifiers.Add(mod);
            ModAdded?.Invoke(mod);
        }

        /// <summary>
        /// Adds multiple modifiers to this <see cref="StatSystem{T}"/>
        /// </summary>
        /// <param name="mods">Mods to add</param>
        public virtual void AddModifiers(params StatMod<T>[] mods)
        {
            if (mods == null)
            {
                throw new ArgumentNullException(nameof(mods));
            }

            foreach (StatMod<T> mod in mods)
            {
                AddModifier(mod);
            }
        }

        /// <summary>
        /// Creates a new mod, adds the mod to this <see cref="StatSystem{T}"/>, and then returns the created mod
        /// </summary>
        /// <param name="value">Value of the mod</param>
        /// <param name="type">How the modifier is applied to existing stats</param>
        /// <param name="source">Where the mod came from, usually "this"</param>
        /// <param name="flags">What flags the modifier has</param>
        /// <returns>The created <see cref="StatMod{T}"/></returns>
        [Button(ButtonHeight = 25, Expanded = true)]
        [TabGroup("Utility")]
        public virtual StatMod<T> AddModifier(float value, StatModType type, object source, params T[] flags)
        {
            if (flags == null)
            {
                throw new ArgumentNullException(nameof(flags));
            }

            StatMod<T> mod = new StatMod<T>(value, type, source, flags);

            AddModifier(mod);

            return mod;
        }

        /// <summary>
        /// Removes the provided mod from the StatSystem
        /// </summary>
        /// <param name="mod">Mod to remove</param>
        /// <returns>Did the mod get removed</returns>
        public virtual bool RemoveModifier(StatMod<T> mod)
        {
            if (Modifiers.Remove(mod))
            {
                ModRemoved?.Invoke(mod);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the provided mods from the StatSystem
        /// </summary>
        /// <param name="mods">Mods to remove</param>
        /// <returns>Did all the mods get removed</returns>
        public virtual bool RemoveModifiers(params StatMod<T>[] mods)
        {
            if (mods == null)
            {
                throw new ArgumentNullException(nameof(mods));
            }

            bool removedAll = true;

            foreach (StatMod<T> mod in mods)
            {
                if (!RemoveModifier(mod)) removedAll = false;
            }

            return removedAll;
        }

        /// <summary>
        /// Removes all mods from a source
        /// </summary>
        /// <param name="source">Where the mod came from, usually "this"</param>
        /// <returns>Did a mod get removed</returns>
        [Button(ButtonHeight = 25, Expanded = true)]
        [TabGroup("Utility")]
        public virtual bool RemoveAllModifiersBySource(object source)
        {
            bool didRemove = false;

            for (int i = Modifiers.Count - 1; i >= 0; i--)
            {
                if (Modifiers[i].Source == source)
                {
                    didRemove = true;
                    RemoveModifier(Modifiers[i]);
                }
            }

            return didRemove;
        }

        /// <summary>
        /// Removes all modifiers without flag "Base", has an optional parameter to choose the exclusion flag instead
        /// </summary>
        /// <param name="flag">Modifiers with this flag will be kept</param>
        [Button(ButtonHeight = 25, Expanded = true)]
        [TabGroup("Utility")]
        public virtual void RemoveAllNonBaseModifiers()
        {
            for (int i = Modifiers.Count - 1; i >= 0; i--)
            {
                if (!Modifiers[i].Flags.HasFlag(default(T)))
                {
                    RemoveModifier(Modifiers[i]);
                }
            }
        }

        /// <summary>
        /// Removes all modifiers from the StatSystem
        /// </summary>
        [Button(ButtonHeight = 25, Expanded = true)]
        [TabGroup("Utility")]
        public virtual void RemoveAllModifiers()
        {
            for (int i = Modifiers.Count - 1; i >= 0; i--)
            {
                RemoveModifier(Modifiers[i]);
            }
        }

        #endregion

        #region TrackedStat

        /// <summary>
        /// Adds a <see cref="TrackedStat{T}"/> to the <see cref="StatSystem{T}"/>
        /// </summary>
        /// <param name="name">Name of the <see cref="TrackedStat{T}"/></param>
        /// <param name="trackedStat"><see cref="TrackedStat{T}"/> to add</param>
        /// <returns>Added <see cref="TrackedStat{T}"/></returns>
        [Button(ButtonHeight = 25, Expanded = true)]
        [TabGroup("Utility")]
        public virtual TrackedStat<T> AddTrackedStat(string name, TrackedStat<T> trackedStat)
        {
            if (!trackedStats.ContainsKey(name))
            {
                trackedStats.Add(name, trackedStat);
#pragma warning disable 612, 618
                trackedStat.OnStatSystemAdded(this, name);
#pragma warning restore 612, 618
                return trackedStat;
            }
            else
            {
                throw new ArgumentException(nameof(name));
            }
        }

        /// <summary>
        /// Gets a <see cref="TrackedStat{T}"/> by name
        /// </summary>
        /// <param name="name">Name of the <see cref="TrackedStat{T}"/></param>
        /// <returns><see cref="TrackedStat{T}"/> of name, <see langword="null"/> if not found</returns>
        public virtual TrackedStat<T> GetTrackedStat(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("message", nameof(name));
            }

            if (trackedStats.ContainsKey(name))
            {
                return trackedStats[name];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a name using the provided <see cref="TrackedStat{T}"/>, <see langword="null"/> if not found
        /// </summary>
        /// <param name="trackedStat"><see cref="TrackedStat{T}"/> to use</param>
        /// <returns>Name of the provided <see cref="TrackedStat{T}"/>, <see langword="null"/> if not found</returns>
        public virtual string GetTrackedStatName(TrackedStat<T> trackedStat)
        {
            return trackedStats.FirstOrDefault(x => x.Value == trackedStat).Key;
        }

        /// <summary>
        /// Removes a <see cref="TrackedStat{T}"/> by <paramref name="name"/>
        /// </summary>
        /// <param name="name">Name of the <see cref="TrackedStat{T}"/> to remove</param>
        /// <returns>True if a <see cref="TrackedStat{T}"/> was successfully removed</returns>
        [Button(ButtonHeight = 25, Expanded = true)]
        [TabGroup("Utility")]
        public virtual bool RemoveTrackedStat(string name)
        {
            if (trackedStats.ContainsKey(name))
            {
#pragma warning disable 612, 618
                trackedStats[name].OnStatSystemRemoved();
#pragma warning restore 612, 618
                trackedStats.Remove(name);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removes a <see cref="TrackedStat{T}"/> <para/>
        /// Note: Slower than removing by name
        /// </summary>
        /// <param name="trackedStat"><see cref="TrackedStat{T}"/> to remove</param>
        /// <returns>True if a <see cref="TrackedStat{T}"/> was successfully removed</returns>
        public virtual bool RemoveTrackedStat(TrackedStat<T> trackedStat)
        {
            string name = GetTrackedStatName(trackedStat);

            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            return RemoveTrackedStat(name);
        }

        #endregion

        #region Internal

        /// <summary>
        /// Called before the TrackedStat is added to the StatSystem <para/>
        /// Example: Use this for setting mods with TrackedStatAddFlagCheck()
        /// </summary>
        protected virtual void BeforeTrackedStatAdded() { }

        /// <summary>
        /// Checks if the TrackedStat being added matches the parameters given, if true, add flagToAdd
        /// </summary>
        /// <param name="flagToAdd">Flag to add to the TrackedStat if parameters are matched</param>
        /// <param name="matchType">How to match the flags</param>
        /// <param name="flags">Flags to match with</param>
        protected virtual void TrackedStatAddFlagCheck(T flagToAdd, FlagMatchType matchType, T[] flags)
        {
            if (flags == null)
            {
                throw new ArgumentNullException(nameof(flags));
            }

            for (int i = 0; i < flags.Length; i++)
            {
                if (flagCache.HasFlag(flags[i]))
                {
                    flagCache.SetFlag(true, flagToAdd);
                    break;
                }
            }
        }

        #endregion
    }
}