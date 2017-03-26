using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoctorGu
{
	public class CEnumerable
	{
		public static Dictionary<TKey, TValue> OrderByRandom<TKey, TValue>(Dictionary<TKey, TValue> dic)
		{
			Random r = new Random();
			return dic
				.OrderBy(kv => r.Next())
				.ToDictionary(kv => kv.Key, kv => kv.Value);
		}

		public static IEnumerable<T> OrderByRandom<T>(IEnumerable<T> Value)
		{
			Random r = new Random();
			return Value.OrderBy(v => r.Next());
		}
	}
}
