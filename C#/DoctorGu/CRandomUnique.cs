using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoctorGu
{
	public class CRandomUnique
	{
		/// <summary>
		/// Min부터 (Max - 1)까지의 랜덤하게 정렬된 모든 수를 리턴함.
		/// </summary>
		/// <param name="Min"></param>
		/// <param name="Max"></param>
		/// <returns></returns>
		public static int[] GenerateRandomlyOrdered(Random Rnd, int Min, int Max)
		{
			List<int> aRemained = new List<int>();
			for (int i = Min; i < Max; i++)
			{
				aRemained.Add(i);
			}

			List<int> aNew = new List<int>();

			int Value = 0;
			while (aRemained.Count > 0)
			{
				Value = Rnd.Next(Min, Max);
				if (aRemained.IndexOf(Value) != -1)
				{
					aRemained.Remove(Value);
					aNew.Add(Value);
				}
			}

			return aNew.ToArray();
		}
		public static int[] GenerateRandomlyOrdered(int Min, int Max)
		{
			return GenerateRandomlyOrdered(new Random(), Min, Max);
		}

		public static int[] GenerateOrdered(Random Rnd, int Min, int Max, int Count)
		{
			if (Count > (Max - Min))
				throw new Exception(string.Format("Count: {0}가 {1}보다 큽니다.", Count, (Max - Min)));

			List<int> aNew = new List<int>();

			int Value = 0;
			while (aNew.Count < Count)
			{
				Value = Rnd.Next(Min, Max);
				if (aNew.IndexOf(Value) == -1)
				{
					aNew.Add(Value);
				}
			}

			aNew.Sort();

			return aNew.ToArray();
		}
		public static int[] GenerateOrdered(int Min, int Max, int Count)
		{
			return GenerateOrdered(new Random(), Min, Max, Count);
		}
	}
}
