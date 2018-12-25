using System;
using System.Collections.Generic;
using System.Linq;

namespace Exanite.Utility
{
	[Serializable]
	public class EnumData<T> where T : struct, IComparable, IConvertible, IFormattable
	{
		#region Fields

		/// <summary>
		/// Array returned by Enum.GetValue(typeof(T2))
		/// </summary>
		public Array array;
		/// <summary>
		/// Max value in T2
		/// </summary>
		public int max;
		/// <summary>
		/// Min value in T2
		/// </summary>
		public int min;
		/// <summary>
		/// Used for serialization
		/// </summary>
		public List<string> lastEnumValueData;

		#endregion

		#region Constructor

		/// <summary>
		/// Creates a new EnumData<T2>
		/// </summary>
		public EnumData()
		{
			if (!typeof(T).IsEnum) throw new ArgumentException(string.Format("{0} is not an Enum Type", typeof(T)));

			array = Enum.GetValues(typeof(T));
			IEnumerable<int> enumerable = array.Cast<int>();
			max = enumerable.Max();
			min = enumerable.Min();

			SetEnumValueData();
		}

		protected void SetEnumValueData()
		{
			lastEnumValueData = new List<string>();
			foreach (Enum enumValue in array)
			{
				lastEnumValueData.Add(enumValue.ToString());
			}
		}

		#endregion
	}
}
