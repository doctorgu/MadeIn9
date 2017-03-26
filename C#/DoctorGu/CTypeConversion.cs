using System;

namespace DoctorGu
{
	/// <summary>
	/// 각 언어 간에 호환되는 Type의 정보를 리턴하기 위한 기능 구현.
	/// </summary>
	/// <example>
	/// string NewType = CTypeConversion.ConvertType(TypeIndex.Col00ManagedClass, TypeIndex.Col01CSharp, "System.SByte");
	/// Console.WriteLine(NewType);
	///	int Index = CTypeConversion.GetByteOfType(TypeIndex.Col01CSharp, NewType);
	///	Console.WriteLine(Index);
	/// </example>
	public class CTypeConversion
	{
		public static object[][] TypeTable = 
			new object[][]{
							  new object[]{"System.Boolean", "bool", "Boolean", "", "", 2},
							  new object[]{"System.Byte", "byte", "Byte", "BYTE", "unsigned char", 1},
							  new object[]{"System.Char", "char", "Char", "CHAR", "char", 2},
							  new object[]{"System.Decimal", "decimal", "Decimal", "", "", 16},
							  new object[]{"System.Double", "double", "Double", "DOUBLE", "Double", 8},
							  new object[]{"System.Int16", "short", "Short", "SHORT", "short", 2},
							  new object[]{"System.Int32", "int", "Integer", "INT", "int", 4},
							  new object[]{"System.Int32", "int", "Integer", "LONG", "long", 4},
							  new object[]{"System.Int32", "int", "Integer", "BOOL", "long", 4},
							  new object[]{"System.Int64", "long", "Long", "", "", 8},
							  new object[]{"System.IntPtr", "", "", "HANDLE", "void*", 4},
							  new object[]{"System.Object", "object", "Object", "", "", 4},
							  new object[]{"System.SByte", "sbyte", "", "", "", 1},
							  new object[]{"System.Single", "float", "Single", "FLOAT", "Float", 4},
							  new object[]{"System.String", "string", "String", "LPSTR", "char*", 0},
							  new object[]{"System.String", "string", "String", "LPCSTR", "Const char*", 0},
							  new object[]{"System.String", "string", "String", "LPWSTR", "wchar_t*", 0},
							  new object[]{"System.String", "string", "String", "LPCWSTR", "Const wchar_t*", 0},
							  new object[]{"System.UInt16", "ushort", "", "WORD", "unsigned short", 2},
							  new object[]{"System.UInt32", "uint", "", "UINT", "unsigned int", 4},
							  new object[]{"System.UInt32", "uint", "", "DWORD", "unsigned long", 4},
							  new object[]{"System.UInt32", "uint", "", "ULONG", "unsigned long", 4},
							  new object[]{"System.UInt64", "ulong", "", "", "", 8}};
			
		public static string ConvertType(TypeIndex FromType, TypeIndex ToType, string TypeName)
		{
			if ((FromType == TypeIndex.Col05Byte) || (ToType == TypeIndex.Col05Byte))
			{
				throw new Exception("형식이 있는 열이 아닙니다.");
			}

			for (int i = 0, i2 = TypeTable.GetUpperBound(0); i < i2; i++)
			{
				if ((string)TypeTable[i][(int)FromType] == TypeName)
				{
					return (string)TypeTable[i][(int)ToType];
				}
			}

			throw new Exception(TypeName + "은 존재하지 않는 형식입니다.");
		}

		public static int GetByteOfType(TypeIndex Index, string TypeName)
		{
			for (int i = 0, i2 = TypeTable.GetUpperBound(0); i < i2; i++)
			{
				if ((string)TypeTable[i][(int)Index] == TypeName)
				{
					return (int)TypeTable[i][(int)TypeIndex.Col05Byte];
				}
			}

			throw new Exception(TypeName + "은 존재하지 않는 형식입니다.");
		}
	}

	public enum TypeIndex
	{
		Col00ManagedClass = 0,
		Col01CSharp = 1,
		Col02VB = 2,
		Col03WinApi = 3,
		Col04UnmanagedC = 4,
		Col05Byte = 5
	}
}
