using System;
using NUnit.Framework;
using System.Data.SQLite;

namespace Model
{
	[TestFixture]
	public class EdgeDataTest
	{
		public EdgeDataTest ()
		{
		}

		[SetUp]
		public void Init ()
		{
		}

		[TearDown]
		public void Dest ()
		{
		}

		[Test]
		public void DistanceTest ()
		{
			Node node1 = new Node (1, "Node1", 10, 100);
			Node node2 = new Node (2, "Node2", 20, 100);
			Edge edge = new Edge (0, node1, node2, 0);
			//Normal
			PointD point = new PointD (15, 90);
			double distance = edge - point;
			Assert.AreEqual(10, distance);
			//On node1
			PointD point2 = new PointD (10, 100);
			double distance2 = edge - point2;
			Assert.AreEqual(0, distance2);
			//On line
			PointD point3 = new PointD (15, 100);
			double distance3 = edge - point3;
			Assert.AreEqual(0, distance3);
			//not between nodes
			PointD point4 = new PointD (25, 100);
			double distance4 = edge - point4;
			Assert.AreEqual(5, distance4);
		}
	}
}