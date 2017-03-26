using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoctorGu
{
	public class CSqlInfoInsert
	{
		public string Table;
		public string[] Field;
		public string[] Delim;
		public string[] Value;

		private bool[] mIsPrimaryKey;
		public bool[] IsPrimaryKey
		{
			get { return this.mIsPrimaryKey; }
		}

		private string[] mPrimaryKey;
		public string[] PrimaryKey
		{
			get { return mPrimaryKey; }

			set
			{
				bool[] IsPrimaryKey = new bool[this.Field.Length];

				for (int i = 0; i < value.Length; i++)
				{
					int Index = CArray.IndexOf(this.Field, value[i], true);
					IsPrimaryKey[Index] = true;
				}

				this.mPrimaryKey = value;
				this.mIsPrimaryKey = IsPrimaryKey;
			}
		}

		public string WhereByPrimaryKey
		{
			get
			{
				string Where = "";
				for (int i = 0; i < this.Field.Length; i++)
				{
					if (this.mIsPrimaryKey[i])
					{
						Where += " and " + this.Field[i] + " = " + this.Delim[i] + this.Value[i] + this.Delim[i];
					}
				}
				if (!string.IsNullOrEmpty(Where))
					Where = Where.Substring(" and ".Length);

				return Where;
			}
		}
	}
}
