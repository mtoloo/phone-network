using System;
using NUnit.Framework;
using System.Data.SQLite;

namespace Model
{
	[TestFixture]
	public class EdgesTest
	{
		Database database;

		Nodes nodes {
			get;
			set;
		}
		Edges edges {
			get;
			set;
		}
		public EdgesTest ()
		{
			this.database = new Database (":memory:");
		}

		[SetUp]
		public void Init ()
		{
			this.database.CreateOrOpen ();
			this.database.DropTableIfExists ("edges");
//			this.nodeModel = new NodeModel (this.database, true);
			this.nodes = new Nodes(this.database, true);
//			this.edgeModel = new EdgeModel (this.database, true);
			this.edges = new Edges(this.database, true, this.nodes);
		}

		[TearDown]
		public void Dest ()
		{
			this.database.Close ();
		}

		[Test]
		public void LoadAllTest() 
		{
			Node node1 = this.nodes.CreateNew("Node1", 10, 10);
			Node node2 = this.nodes.CreateNew("Node2", 20, 10);
			Edge edge = this.edges.CreateNew(node1, node2);
			this.edges.LoadAll();
			Assert.AreEqual(node1.id, this.edges.edges[edge.id].node1.id);
			Assert.AreEqual(node2.name, this.edges.edges[edge.id].node2.name);
		}

		[Test]
		public void createNewTest()
		{
			Node node1 = this.nodes.CreateNew("Node1", 10, 10);
			Node node2 = this.nodes.CreateNew("Node2", 20, 10);
			Edge edge = this.edges.CreateNew(node1, node2);
			Assert.AreEqual(edge.node1.id, node1.id);
			Assert.AreEqual(edge.node2.id, node2.id);
			Assert.AreEqual(edge.distance, 10);
			var actual = this.database.ExecuteScalarQuery ("select distance from edges");
			Assert.AreEqual(10, actual);
		}

		[Test]
		public void deleteTest ()
		{
			Node node1 = this.nodes.CreateNew ("Node1", 10, 10);
			Node node2 = this.nodes.CreateNew ("Node2", 20, 10);
			Node node3 = this.nodes.CreateNew ("Node3", 20, 20);
			Edge edge1 = this.edges.CreateNew (node1, node2);
			Edge edge2 = this.edges.CreateNew (node2, node3);
			this.edges.DeleteEdge (edge1);
			Assert.AreEqual (1, this.edges.edges.Count);
			Assert.AreEqual (edge2.id, this.edges.edges [0].id);
		}

		[Test]
		public void deleteNodeTest ()
		{
			Node node1 = this.nodes.CreateNew ("Node1", 10, 10);
			Node node2 = this.nodes.CreateNew ("Node2", 20, 10);
			Node node3 = this.nodes.CreateNew ("Node3", 20, 20);
			this.edges.CreateNew (node1, node2);
			this.edges.CreateNew (node2, node3);
			Edge edge = this.edges.CreateNew (node1, node3);
			this.edges.DeleteNodeEdges(node2);
			Assert.AreEqual(1, this.edges.edges.Count);
			var actual = this.database.ExecuteScalarQuery ("select node1_id from edges");
			Assert.AreEqual(edge.node1.id, actual);
		}

	}
}
