using System;

public struct PointD
{
	public double X, Y;

	public PointD (double x, double y)
	{
		this.X = x;
		this.Y = y;
	}

//	public static double distance (PointD p1, PointD p2)
//	{
//		double dx = p1.X - p2.X;
//		double dy = p1.Y - p2.Y;
//		double result = Math.Pow (dx, 2) + Math.Pow (dy, 2);
//		result = Math.Sqrt (result);
//		return result;
//	}

	//vector minus
	public static PointD operator - (PointD p1, PointD p2)
	{
		PointD result = new PointD(p1.X - p2.X, p1.Y - p2.Y);
		return result;
	}

	//dot product
	public static double operator * (PointD p1, PointD p2)
	{
		double result = p1.X * p2.X + p1.Y * p2.Y;
		return result;
	}

	//cross product
	public static double operator ^ (PointD p1, PointD p2)
	{
		double result = p1.X * p2.Y - p1.Y * p2.X;
		return result;
	}

}
