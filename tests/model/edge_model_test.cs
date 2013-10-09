using System;
using NUnit.Framework;
using System.Data.SQLite;

namespace Model
{
	[TestFixture]
	public class EdgeModelTest
	{
		Database database;
		NodeModel nodeModel;
		EdgeModel edgeModel;

		public EdgeModelTest ()
		{
			this.database = new Database (":memory:");
		}

		[SetUp]
		public void Init ()
		{
			this.database.CreateOrOpen ();
			this.database.DropTableIfExists ("edges");
			this.nodeModel = new NodeModel (this.database, true);
			this.edgeModel = new EdgeModel (this.database, true);
		}

		[TearDown]
		public void Dest ()
		{
			this.database.Close ();
		}

		Node createAndSaveNode (int number)
		{
			Node node = new Node (number, "Node" + number.ToString (), number * 2, number * 4);
			node.id = this.nodeModel.InsertAndGetId (node);
			return node;
		}

		Edge createAndSaveTwoNodesAndEdge ()
		{
			Node node1 = createAndSaveNode (1);
			Node node2 = createAndSaveNode (2);
			Edge edge = new Edge (0, node1, node2, node2 - node1);
			edge.id = this.edgeModel.InsertAndGetId (edge);
			return edge;
		}

		[Test]
		public void InsertTest ()
		{
			Edge edge = createAndSaveTwoNodesAndEdge ();
			string sql = "select * from edges where id = " + edge.id.ToString ();
			SQLiteDataReader reader = this.database.ExecuteReader (sql);
			Assert.AreEqual (true, reader.Read ());
			Assert.AreEqual (edge.node1_id, reader ["node1_id"]);
			Assert.AreEqual (edge.node2_id, reader ["node2_id"]);
			Assert.AreEqual (edge.distance, (double)reader ["distance"]);
		}

		[Test]
		public void UpdateTest ()
		{
			Edge edge = createAndSaveTwoNodesAndEdge ();

			edge.distance = 1000;
			this.edgeModel.Update (edge);
			string sql = "select * from edges where id = " + edge.id.ToString ();
			SQLiteDataReader reader = this.database.ExecuteReader (sql);
			Assert.AreEqual (true, reader.Read ());
			Assert.AreEqual (edge.distance, (double)reader ["distance"]);
		}

		[Test]
		[ExpectedException]
		public void UpdateEmptyTest ()
		{
			Edge edge = createAndSaveTwoNodesAndEdge ();
			edge.id = 0;
			this.nodeModel.Update (edge);
		}

		[Test]
		public void DeleteTest ()
		{
			Edge edge = createAndSaveTwoNodesAndEdge ();
			this.edgeModel.Delete (edge.id);
			string sql = "select * from edges where id = " + edge.id.ToString ();
			SQLiteDataReader reader = this.database.ExecuteReader (sql);
			Assert.AreEqual (false, reader.Read ());
		}

		[Test]
		[ExpectedException]
		public void DeleteEmptyTest ()
		{
			this.nodeModel.Delete (-1);
		}
	}
}

