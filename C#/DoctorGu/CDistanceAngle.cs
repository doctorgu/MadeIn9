using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DoctorGu
{
	public class CDistanceAngle
	{
		public static double AngleBetweenTwoPoint(Point p1, Point p2)
		{
			double Angle = (Math.Atan2(p2.Y - p1.Y, p2.X - p1.X) * 180.0 / Math.PI);
			if (Angle < 0)
				Angle = 270 + (90 - Math.Abs(Angle));

			return Angle;
		}

		/// <summary>
		/// Point의 X 값을 맞바꿈.
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		public static void SwapX(ref Point p1, ref Point p2)
		{
			int XTemp = p1.X;
			p1.X = p2.X;
			p2.X = XTemp;
		}
		/// <summary>
		/// Point의 Y 값을 맞바꿈.
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		public static void SwapY(ref Point p1, ref Point p2)
		{
			int YTemp = p1.Y;
			p1.Y = p2.Y;
			p2.Y = YTemp;
		}
		public static void SwapPoint(ref Point p1, ref Point p2)
		{
			Point Temp = new Point(p1.X, p1.Y);
			p1.X = p2.X;
			p1.Y = p2.Y;

			p2.X = Temp.X;
			p2.Y = Temp.Y;
		}

		/// <summary>
		/// Radian 값을 Degree 값으로 변경해서 리턴함.
		/// </summary>
		/// <remarks>
		/// radian : 호도, hypotenuse
		/// cos(radian) = width / hypotenuse
		/// sin(radian) = width / hypotenuse
		/// tan(radian) = height / width
		/// </remarks>
		/// <param name="Radian">Radian 값</param>
		/// <returns>Degree 값</returns>
		public static double RadianToDegree(double Radian)
		{
			return Radian * (180.0 / Math.PI);
		}
		/// <summary>
		/// Degree 값을 Radian 값으로 변경해서 리턴함.
		/// </summary>
		/// <remarks>
		/// radian : 호도, hypotenuse
		/// cos(radian) = width / hypotenuse
		/// sin(radian) = width / hypotenuse
		/// tan(radian) = height / width
		/// </remarks>
		/// <param name="Degree">Degree 값</param>
		/// <returns>Radian 값</returns>
		public static double DegreeToRadian(double Degree)
		{
			return Math.PI * Degree / 180.0;
		}

		/// <summary>
		/// X, Y 위치와 거리, 각도를 이용해 새로운 X, Y 위치를 리턴함.
		/// </summary>
		/// <param name="pt">X, Y 위치</param>
		/// <param name="Distance">거리</param>
		/// <param name="Angle">각도</param>
		/// <returns>새로운 X, Y 위치 정보</returns>
		public static Point PointByDegree(Point pt, double Distance, double Angle)
		{
			double Radian = DegreeToRadian(Angle);
			return PointByRadian(pt, Distance, Radian);
		}
		/// <summary>
		/// X, Y 위치와 거리, 호도를 이용해 새로운 X, Y 위치를 리턴함.
		/// </summary>
		/// <param name="pt">X, Y 위치</param>
		/// <param name="Distance">거리</param>
		/// <param name="Radian">호도</param>
		/// <returns>새로운 X, Y 위치 정보</returns>
		public static Point PointByRadian(Point pt, double Distance, double Radian)
		{
			double x = pt.X + (Distance * Math.Cos(Radian));
			double y = pt.Y + (Distance * Math.Sin(Radian));

			return new Point(Convert.ToInt32(x), Convert.ToInt32(y));
		}
		public static double DistanceBetweenPoint(Point p1, Point p2)
		{
			double Width = p2.X - p1.X;
			double Height = p2.Y - p1.Y;

			return Math.Sqrt(Math.Pow(Width, 2) + Math.Pow(Height, 2));
		}

		public static Point Rotate(Point Center, Point Outer, double Angle)
		{
			double Distance = DistanceBetweenPoint(Center, Outer);
			double AngleOld = AngleBetweenTwoPoint(Center, Outer);
			return PointByDegree(Center, Distance, AngleOld + Angle);
		}

		/// <summary>
		/// Parent 안의 Child가 공유하는 부분 없이 포함되도록 함.
		/// </summary>
		/// <param name="Parent"></param>
		/// <param name="Child"></param>
		/// <returns></returns>
		public static Rectangle ContainWithNoIntersection(Rectangle Parent, Rectangle Child)
		{
			if (Child.Left < Parent.Left)
				Child.X = Parent.Left;
			if (Child.Right > Parent.Right)
				Child.X -= (Child.Right - Parent.Right);
			if (Child.Top < Parent.Top)
				Child.Y = Parent.Top;
			if (Child.Bottom > Parent.Bottom)
				Child.Y -= (Child.Bottom - Parent.Bottom);

			return Child;
		}

		public static Point[] GetPolygon(int Sides, Point ptCenter, double Radius)
		{
			List<Point> ptCol = new List<Point>();

			double AngleEach = 360 / Sides;
			for (int i = 0; i < Sides; i++)
			{
				ptCol.Add(CDistanceAngle.PointByDegree(ptCenter, Radius, ((i * AngleEach) - 90)));
			}

			return ptCol.ToArray();
		}
	}
}
