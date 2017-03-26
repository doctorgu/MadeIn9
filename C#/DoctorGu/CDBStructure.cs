using System;
using System.Collections;
using System.Collections.Generic;

namespace DoctorGu
{
	/// <summary>
	/// 여러 개의 DbTable 개체를 가지는 DbTable의 상위 클래스
	/// </summary>
	[Serializable]
	public class DbTables
	{
		private Hashtable mht;
		private List<string> KeyIndex;

		public DbTables()
		{
			mht = new Hashtable();
			KeyIndex = new List<string>();
		}

		/// <summary>
		/// 테이블 개수
		/// </summary>
		public int Count
		{
			get {return mht.Count;}
		}

		/// <summary>
		/// 테이블 이름을 키로 가진 DbTable 개체를 리턴함.
		/// </summary>
		/// <param name="Key">테이블 이름</param>
		/// <returns>찾아진 DbTable 개체</returns>
		public DbTable this [string Key]
		{
			get {return (DbTable)mht[Key];}
			set {mht[Key] = value;}
		}
		/// <summary>
		/// Index를 가진 DbTable 개체를 리턴함.
		/// (Index는 Add 메쏘드로 추가된 순서를 뜻함.)
		/// </summary>
		/// <param name="Index">테이블의 순서</param>
		/// <returns>찾아진 DbTable 개체</returns>
		public DbTable this [int Index]
		{
			get
			{
				string Key = KeyIndex[Index].ToString();
				return (DbTable)mht[Key];
			}
			set 
			{
				string Key = KeyIndex[Index].ToString();
				mht[Key] = value;
			}
		}

		/// <summary>
		/// DbTable 개체를 DbTables 개체에 추가함.
		/// </summary>
		/// <param name="tbl">추가할 DbTable 개체</param>
		/// <returns>추가된 DbTable 개체</returns>
		public DbTable Add(DbTable tbl)
		{
			mht.Add(tbl.Name, tbl);
			KeyIndex.Add(tbl.Name);

			return tbl;
		}
		/// <summary>
		/// DbTables 개체에 있는 DbTable 개체 중 <paramref name="Name"/> 값을 가진 DbTable 개체를 삭제함.
		/// </summary>
		/// <param name="Name">삭제할 테이블 이름</param>
		public void Remove(string Name)
		{
			mht.Remove(Name);
			KeyIndex.Remove(Name);
		}
	}

	/// <summary>
	/// 테이블 또는 뷰를 의미함.
	/// </summary>
	[Serializable]
	public class DbTable
	{
		/// <summary>테이블(뷰) 이름</summary>
		public string Name = "";
		/// <summary>테이블(뷰) 설명</summary>
		public string Description = "";
		/// <summary>테이블인 지, 뷰인 지의 여부</summary>
		public EntityType Type = EntityType.Table;

		/// <summary>기본키인 열 목록</summary>
		private DbColumn[] mPrimaryKey;
		/// <summary>인덱스가 설정된 열 목록</summary>
		private DbColumn[] mIndex;
		/// <summary>테이블에 속한 모든 열 목록</summary>
		private DbColumns mDbColumns = new DbColumns();

		/// <summary>
		/// DbTable 개체를 생성할 때 테이블의 이름, 표시용 이름을 만듦.
		/// </summary>
		/// <param name="Name">테이블 이름</param>
		/// <param name="Description">테이블의 설명(주로 한글 이름)</param>
		public DbTable(string Name, string Description)
		{
			this.Name = Name;
			this.Description = Description;
		}

		/// <summary>
		/// 테이블의 기본키를 설정하거나 리턴함.
		/// </summary>
		public DbColumn[] PrimaryKey
		{
			get {return mPrimaryKey;}
			set 
			{
				mPrimaryKey = value;

				for (int i = 0, i2 = mDbColumns.Count; i < i2; i++)
				{
					mDbColumns[i].IsPrimaryKey = false;
				}

				for (int i = 0, i2 = mPrimaryKey.Length; i < i2; i++)
				{
					mPrimaryKey[i].IsPrimaryKey = true;
				}
			}
		}

		/// <summary>
		/// 테이블의 인덱스를 설정하거나 리턴함.
		/// </summary>
		public DbColumn[] Index
		{
			get {return mIndex;}
			set 
			{
				mIndex = value;

				for (int i = 0, i2 = mDbColumns.Count; i < i2; i++)
				{
					mDbColumns[i].IsIndex = false;
				}

				for (int i = 0, i2 = mIndex.Length; i < i2; i++)
				{
					mIndex[i].IsIndex = true;
				}
			}
		}

		/// <summary>
		/// 현재 테이블에 속하는 모든 DbColumn을 설정하거나 리턴함.
		/// </summary>
		public DbColumns DbColumns
		{
			get {return mDbColumns;}
			set {mDbColumns = value;}
		}
	}

	/// <summary>
	/// 여러 개의 DbColumn 개체를 가지는 DbColumn의 상위 클래스
	/// </summary>
	[Serializable]
	public class DbColumns
	{
		private Hashtable mht;
		private List<string> KeyIndex;

		/// <summary>
		/// 
		/// </summary>
		public DbColumns()
		{
			mht = new Hashtable();
			KeyIndex = new List<string>();
		}

		/// <summary>
		/// DbColumn 개체의 개수를 리턴함.
		/// </summary>
		public int Count
		{
			get {return mht.Count;}
		}

		/// <summary>
		/// 열 이름을 키로 가진 DbColumn 개체를 리턴하거나 설정함.
		/// </summary>
		/// <param name="Key">열 이름</param>
		/// <returns>찾아진 DbColumn 개체</returns>
		public DbColumn this [string Key]
		{
			get {return (DbColumn)mht[Key];}
			set {mht[Key] = value;}
		}
		/// <summary>
		/// Index를 가진 DbColumn 개체를 리턴함.
		/// (Index는 Add 메쏘드로 추가된 순서를 뜻함.)
		/// </summary>
		/// <param name="Index">열의 순서</param>
		/// <returns>찾아진 DbColumn 개체</returns>
		public DbColumn this [int Index]
		{
			get
			{
				string Key = KeyIndex[Index].ToString();
				return (DbColumn)mht[Key];
			}
			set 
			{
				string Key = KeyIndex[Index].ToString();
				mht[Key] = value;
			}
		}

		/// <summary>
		/// DbColumn 개체를 추가함.
		/// </summary>
		/// <param name="c">추가할 DbColumn 개체</param>
		/// <returns>추가된 DbColumn 개체</returns>
		public DbColumn Add(DbColumn c)
		{
			mht.Add(c.Name, c);
			KeyIndex.Add(c.Name);

			return c;
		}
		/// <summary>
		/// DbColumns 개체에 있는 DbColumn 개체 중 <paramref name="Name"/> 값을 가진 DbColumn 개체를 삭제함.
		/// </summary>
		/// <param name="Name">삭제할 열의 이름</param>
		public void Remove(string Name)
		{
			mht.Remove(Name);
			KeyIndex.Remove(Name);
		}

		/// <summary>
		/// DbColumn 배열의 모든 항목의 열 이름을 배열 형식으로 리턴함.
		/// </summary>
		/// <param name="cols">DbColumn 배열</param>
		/// <returns>열 이름</returns>
		public static string[] GetNames(DbColumn[] cols)
		{
			string[] names = new string[cols.Length];
			for (int i = 0, i2 = cols.Length; i < i2; i++)
			{
				names[i] = cols[i].Name;
			}

			return names;
		}
	}

	/// <summary>
	/// 테이블 또는 뷰의 열을 의미하는 개체
	/// </summary>
	[Serializable]
	public class DbColumn
	{
		/// <summary>기본키</summary>
		public bool IsPrimaryKey = false;
		/// <summary>인덱스인 지 여부</summary>
		public bool IsIndex = false;
		/// <summary>ID 열인 지 여부</summary>
		public bool IsID = false;
		/// <summary>열 이름</summary>
		public string Name = "";
		/// <summary>데이터 형식</summary>
		public string Type = "";
		/// <summary>Null 허용 여부</summary>
		public bool IsNullable = false;
		/// <summary>기본값</summary>
		public string DefaultValue = "";
		/// <summary>열 설명(주로 한글 이름)</summary>
		public string Description = "";
		
		/// <summary>
		/// DbColumn 생성자
		/// </summary>
		/// <param name="IsPrimaryKey">기본키</param>
		/// <param name="IsIndex">인덱스인 지 여부</param>
		/// <param name="IsID">ID 열인 지 여부</param>
		/// <param name="Name">열 이름</param>
		/// <param name="Type">데이터 형식</param>
		/// <param name="IsNullable">Null 허용 여부</param>
		/// <param name="DefaultValue">기본값</param>
		/// <param name="Description">열 설명(주로 한글 이름)</param>
		public DbColumn(bool IsPrimaryKey, bool IsIndex, bool IsID,
			string Name, string Type,
			bool IsNullable, string DefaultValue,
			string Description)
		{
			this.IsPrimaryKey = IsPrimaryKey;
			this.IsIndex = IsIndex;
			this.IsID = IsID;
			this.Name = Name;
			this.Type = Type;
			this.IsNullable = IsNullable;
			this.DefaultValue = DefaultValue;
			this.Description = Description;
		}
	}

	/// <summary>
	/// 여러 개의 DbReference 개체를 가지는 DbReference의 상위 클래스 
	/// </summary>
	[Serializable]
	public class DbReferences
	{
		private Hashtable htReferences = new Hashtable();
		private List<string> KeyIndex = new List<string>();

		/// <summary>
		/// Reference를 생성할 때 사용된 Constraint 이름에 해당하는 DbReference를 리턴함.
		/// </summary>
		/// <param name="ConstraintName">Constraint 이름</param>
		/// <returns>찾아진 DbReference 개체</returns>
		public DbReference this[string ConstraintName]
		{
			get
			{
				return (DbReference)htReferences[ConstraintName];
			}
		}
		/// <summary>
		/// Index에 해당하는 DbReference를 리턴함.
		/// (Index는 Add 메쏘드로 추가된 순서를 뜻함.)
		/// </summary>
		/// <param name="Index">Reference의 순서</param>
		/// <returns>찾아진 DbReference 개체</returns>
		public DbReference this[int Index]
		{
			get
			{
				string Key = (string)KeyIndex[Index];
				return (DbReference)htReferences[Key];
			}
		}

		/// <summary>
		/// DbReference 개체의 개수를 리턴함.
		/// </summary>
		public int Count
		{
			get {return htReferences.Count;}
		}

		/// <summary>
		/// DbReference 개체를 추가함.
		/// </summary>
		/// <param name="refer">추가할 DbReference 개체</param>
		/// <returns>추가된 DbReference 개체</returns>
		public DbReference Add(DbReference refer)
		{
			htReferences.Add(refer.ConstraintName, refer);
			KeyIndex.Add(refer.ConstraintName);
			return refer;
		}
		/// <summary>
		/// Index에 해당하는 DbReference 개체를 삭제함.
		/// </summary>
		/// <param name="Index">삭제할 DbReference의 Index</param>
		public void Remove(int Index)
		{
			string Key = (string)KeyIndex[Index];
			htReferences.Remove(Key);
		}
	}

	/// <summary>
	/// 각 테이블(뷰) 간의 연결 관계를 표현하기 위함.
	/// </summary>
	[Serializable]
	public class DbReference
	{
		/// <summary>Constraint 이름</summary>
		public string ConstraintName;
		/// <summary>참조 대상인 외부 테이블의 필드명 목록</summary>
		public string[] ForeignKeyList;
		/// <summary>참조의 주체인 테이블명</summary>
		public string ReferenceTable;
		/// <summary>참조의 주체인 테이블의 필드명 목록</summary>
		public string[] ReferenceFieldList;

		/// <summary>참조된 테이블 간의 연결 방법</summary>
		public JoinType JoinType = JoinType.Inner;
		/// <summary>참조된 테이블의 필드 간의 연결 방법</summary>
		public Operator Operator = Operator.Equal;
		
		/// <summary>
		/// DbReference 생성자
		/// </summary>
		/// <param name="ConstraintName">Constraint 이름</param>
		/// <param name="ForeignKeyList">참조 대상인 외부 테이블의 필드명 목록</param>
		/// <param name="ReferenceTable">참조의 주체인 테이블명</param>
		/// <param name="ReferenceFieldList">참조의 주체인 테이블의 필드명 목록</param>
		public DbReference(string ConstraintName, string[] ForeignKeyList, string ReferenceTable, string[] ReferenceFieldList)
		{
			if (ForeignKeyList.Length != ReferenceFieldList.Length)
			{
				throw new Exception("ForeignKeyList와 ReferenceFieldList의 개수는 같아야 합니다.");
			}

			this.ConstraintName = ConstraintName;
			this.ForeignKeyList = ForeignKeyList;
			this.ReferenceTable = ReferenceTable;
			this.ReferenceFieldList = ReferenceFieldList;
		}
	}

	/// <summary>
	/// Entity의 형식
	/// </summary>
	public enum EntityType
	{
		Table, View
	}

	/// <summary>
	/// 조인 형식
	/// </summary>
	public enum JoinType
	{
		Inner, Left, Right, Full
	}

	/// <summary>
	/// 조인한 필드를 연결하는 Operator 종류
	/// </summary>
	public enum Operator
	{
		Equal, NotEqual, Less, LessEqual, Greater, GreaterEqual
	}
}
