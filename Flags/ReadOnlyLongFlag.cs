﻿using System;
using System.Collections.Generic;

namespace Exanite.Flags
{
    /// <summary>
    /// <see cref="LongFlag"/> that is <see langword="readonly"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ReadOnlyLongFlag<T> where T : struct, IComparable, IConvertible, IFormattable
    {
        private LongFlag<T> longFlag;

        /// <summary>
        /// How many flags are stored
        /// </summary>
        public int Count
        {
            get
            {
                return longFlag.Count;
            }
        }

        /// <summary>
        /// Creates a new <see cref="ReadOnlyLongFlag{T}"/>
        /// </summary>
        /// <param name="longFlag"><see cref="LongFlag{T}"/> to use</param>
        public ReadOnlyLongFlag(LongFlag<T> longFlag)
        {
            if (longFlag == null)
            {
                throw new ArgumentNullException(nameof(longFlag));
            }

            this.longFlag = longFlag;
        }

        /// <summary>
        /// Returns the internal <see cref="LongFlag{T}"/>
        /// </summary>
        public static explicit operator LongFlag<T>(ReadOnlyLongFlag<T> readOnlyLongFlag)
        {
            return readOnlyLongFlag.longFlag;
        }

        /// <summary>
        /// Returns true if this <see cref="ReadOnlyLongFlag{T}"/> has the provided flag
        /// </summary>
        /// <param name="flag">Flag to check</param>
        /// <returns>True or false</returns>
        public bool HasFlag(T flag)
        {
            return longFlag.HasFlag(flag);
        }

        /// <summary>
        /// Returns true if this <see cref="ReadOnlyLongFlag{T}"/> matches the provided flags
        /// </summary>
        /// <param name="matchType">How to match the flags</param>
        /// <param name="flags">Flags to check</param>
        /// <returns>True or false</returns>
        public bool HasFlags(FlagMatchType matchType, params T[] flags)
        {
            return longFlag.HasFlags(matchType, flags);
        }

        /// <summary>
        /// Returns true if this <see cref="ReadOnlyLongFlag{T}"/> matches the provided flags
        /// </summary>
        /// <param name="matchType">How to match the flags</param>
        /// <param name="longFlag"><see cref="LongFlag{T}"/> to check</param>
        /// <returns>True or false</returns>
        public bool HasFlags(FlagMatchType matchType, LongFlag<T> longFlag)
        {
            return this.longFlag.HasFlags(matchType, longFlag);
        }

        /// <summary>
        /// Returns true if the other <see cref="LongFlag{T}"/> has the flags of this <see cref="LongFlag{T}"/>
        /// </summary>
        /// <param name="matchType">How to match the flags</param>
        /// <param name="longFlag">Other <see cref="LongFlag{T}"/></param>
        /// <returns>True or false</returns>
        public virtual bool HasFlagsReverse(FlagMatchType matchType, LongFlag<T> longFlag)
        {
            return this.longFlag.HasFlagsReverse(matchType, longFlag);
        }

        /// <summary>
        /// Returns a list of all true flags
        /// </summary>
        /// <returns>List of all true flags</returns>
        public virtual List<T> GetAllTrueFlags()
        {
            return longFlag.GetAllTrueFlags();
        }
    }
}