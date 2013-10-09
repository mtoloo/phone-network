using System;
using NUnit.Framework;
using System.Data.SQLite;

namespace Model
{
	[TestFixture]
	public class NodeDataTest
	{

		public NodeDataTest ()
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
		public void MinusTest ()
		{
			Node node1 = new Node(1, "1", 10, 20);
			Node node2 = new Node(2, "2", 13, 20);
			double distance = node2 - node1;
			Assert.AreEqual(3, distance);
			Assert.AreEqual(distance, node1 - node2);
		}
	}
}

