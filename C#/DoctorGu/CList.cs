using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoctorGu
{
	public class CList
	{
		/// <summary>
		/// 
		/// </summary>
		/// <example>
		/// <![CDATA[
		/// List<CHanja> aHanjaNew = new List<CHanja>();
		/// aHanjaNew = CList.SortByRandom(aHanjaNew);
		/// ]]>
		/// </example>
		/// <typeparam name="T"></typeparam>
		/// <param name="aList"></param>
		/// <returns></returns>
		[Obsolete("Use CEnumerable.OrerByRandom")]
		public static List<T> SortByRandom<T>(Random Rnd, List<T> aList)
		{
			List<T> aNew = new List<T>();

			int[] aOrder = CRandomUnique.GenerateRandomlyOrdered(Rnd, 0, aList.Count);
			for (int i = 0; i < aOrder.Length; i++)
			{
				aNew.Add(aList[aOrder[i]]);
			}

			return aNew;
		}
		[Obsolete("Use CEnumerable.OrerByRandom")]
		public static List<T> SortByRandom<T>(List<T> aList)
		{
			return SortByRandom(new Random(), aList);
		}

		[Obsolete("Use CEnumerable.OrerByRandom")]
		public static Dictionary<TKey, TValue> SortByRandom<TKey, TValue>(Random Rnd, Dictionary<TKey, TValue> aDictionary)
		{
			Dictionary<TKey, TValue> aNew = new Dictionary<TKey, TValue>();

			int[] aOrder = CRandomUnique.GenerateRandomlyOrdered(Rnd, 0, aDictionary.Count);
			List<TKey> aKey = aDictionary.Keys.ToList();

			for (int i = 0; i < aOrder.Length; i++)
			{
				TKey Key = aKey[aOrder[i]];
				aNew.Add(Key, aDictionary[Key]);
			}

			return aNew;
		}
		[Obsolete("Use CEnumerable.OrerByRandom")]
		public static Dictionary<TKey, TValue> SortByRandom<TKey, TValue>(Dictionary<TKey, TValue> aDictionary)
		{
			return SortByRandom(new Random(), aDictionary);
		}
	}
}
