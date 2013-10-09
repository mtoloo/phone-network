using System;
using NUnit.Framework;
using System.Data.SQLite;
using System.Collections.Generic;

namespace Model
{
	[TestFixture]
	public class NodesTest
	{
		Database database;
//		NodeModel nodeModel;

		Nodes nodes {
			get;
			set;
		}
		public NodesTest ()
		{
			this.database = new Database (":memory:");
		}

		[SetUp]
		public void Init ()
		{
			this.database.CreateOrOpen ();
			this.database.DropTableIfExists ("nodes");
			this.nodes = new Nodes(this.database, true);
//			this.nodeModel = new NodeModel (this.database, true);
		}

		[TearDown]
		public void Dest ()
		{
			this.database.Close ();
		}

		[Test]
		public void CreateTest ()
		{
			Node node = this.nodes.CreateNew("TestCreate", 1, 2);
			string sql = "select * from nodes where id = " + node.id.ToString ();
			SQLiteDataReader reader = this.database.ExecuteReader (sql);
			Assert.AreEqual (true, reader.Read ());
			Assert.AreEqual ("TestCreate", reader ["name"]);
		}

		[Test]
		public void SelectAtPositionTest()
		{
			Node node1 = this.nodes.CreateNew("Node1", 10, 20);
			Node node2 = this.nodes.CreateNew("Node2", 20, 0);
			Assert.AreEqual(true, this.nodes.SelectAtPoisition(9, 19, 5));
			Assert.AreEqual(node1.id, this.nodes.FirstSelected().id);

			this.nodes.ClearSelection();
			Assert.AreEqual(true, this.nodes.SelectAtPoisition(30, -10, 100));
			Assert.AreEqual(node2.id, this.nodes.FirstSelected().id);

			Assert.AreEqual(false, this.nodes.SelectAtPoisition(30, -10, 10));
		}

		[Test]
		public void MoveTest()
		{
			Node node = this.nodes.CreateNew("Node1", 10, 20);
			Assert.AreEqual(true, this.nodes.Select(node));
			this.nodes.MoveFirstSelectedNode(12, -4.5);
			Assert.AreEqual(12, this.nodes.FirstSelected().left);
			Assert.AreEqual(-4.5, this.nodes.FirstSelected().top);
		}

		[Test]
		public void DeleteTest()
		{
			this.nodes.CreateNew("Node1", 10, 20);
			this.nodes.CreateNew("Node2", 20, 0);
			Nodes.minimumDistance = 5;
			Assert.AreEqual(true, this.nodes.DeleteAtPoisition(new PointD(9, 19)));
			string sql = "select * from nodes";
			SQLiteDataReader reader = this.database.ExecuteReader (sql);
			Assert.AreEqual (true, reader.Read ());
			Assert.AreEqual ("Node2", reader ["name"]);
			Assert.AreEqual (false, reader.Read ());
		}

		[Test]
		public void LoadAllTest()
		{
			Node node1 = this.nodes.CreateNew("Node1", 10.1, 20);
			Node node2 = this.nodes.CreateNew("Node2", 100.2, 200);
			this.nodes.CreateNew("Node3", 100, 200.3);
			Nodes nodes2 = new Nodes(this.database, false);
			nodes2.LoadAll();

			nodes2.SelectAtPoisition(10, 20, 1);
			Assert.AreEqual(node1.name, nodes2.FirstSelected().name);

			nodes2.Select(node2);
			Assert.AreEqual(node2.name, nodes2.FirstSelected().name);
		}
	}
}

