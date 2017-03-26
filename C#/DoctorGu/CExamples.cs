namespace DoctorGu
{
	using System;

	/// <summary>
	/// Class 자체를 통해서만 static 메쏘드를 호출할 수 있음.
	/// </summary>
	/// <example>
	/// <code>
	/// Console.WriteLine("Static value s: {0}", StaticVSInstance.GetStaticValue());
	/// 
	/// StaticVSInstance si = new StaticVSInstance();
	/// Console.WriteLine("Instance value i: {0}", si.GetInstanceValue());
	/// 
	/// Console.WriteLine("Static and instance value s + i: {0}", si.GetStaticInstanceValue());
	/// 
	/// Console.ReadLine();
	/// </code>
	/// </example>
	public class StaticVSInstance
	{
		private static string s = "static";
		private string i = "instance";

		/// <summary>
		/// </summary>
		public static string GetStaticValue()
		{
			//static 메쏘드에서 instance 멤버는 사용할 수 없음.
			return s;
		}

		/// <summary>
		/// </summary>
		public string GetInstanceValue()
		{
			return i;
		}

		/// <summary>
		/// </summary>
		public string GetStaticInstanceValue()
		{
			return s + " " + i;
		}
	}


	/// <summary>
	/// set, get 테스트
	/// </summary>
	/// <example>
	/// <code>
	/// Radio radio = new Radio();
	/// 
	/// radio.Volume = 30;
	/// Console.WriteLine("volume: {0}", radio.Volume);
	/// 
	/// Console.ReadLine();
	/// 
	/// 
	/// //150을 넘었으므로 에러 발생함.
	/// try
	/// {
	/// radio.Volume = 200;
	/// }
	/// catch(Exception e)
	/// {
	/// Console.WriteLine(e.Message);
	/// }
	/// 
	/// Console.WriteLine("volume: {0}", radio.Volume);
	/// 
	/// Console.ReadLine();
	/// </code>
	/// </example>
	public class Radio
	{
		private const int CVolumeSoLoud = 100;
		private int volume = 0;

		/// <summary>
		/// </summary>
		public int Volume 
		{
			get 
			{
				return volume;
			}

			set
			{
				if (value < 0 || value > CVolumeSoLoud)
				{
					throw new ArgumentException("볼륨이 너무 크면 주위 사람들이 싫어합니다.");
				}

				volume = value;
			}
		}
	}
}
