using System;
using NUnit.Framework;
using System.Data.SQLite;

namespace Model
{
	[TestFixture]
	public class NodeModelTest
	{
		Database database;
		NodeModel nodeModel;

		public NodeModelTest ()
		{
			this.database = new Database (":memory:");
		}

		[SetUp]
		public void Init ()
		{
			this.database.CreateOrOpen ();
			this.database.DropTableIfExists ("nodes");
			this.nodeModel = new NodeModel (this.database, true);
		}

		[TearDown]
		public void Dest ()
		{
			this.database.Close ();
		}

		[Test]
		public void InsertTest ()
		{
			Node node = new Node (0, "TestInsert", 2, 3);
			long id = this.nodeModel.InsertAndGetId (node);
			string sql = "select * from nodes where id = " + id.ToString ();
			SQLiteDataReader reader = this.database.ExecuteReader (sql);
			Assert.AreEqual (true, reader.Read ());
			Assert.AreEqual ("TestInsert", reader ["name"]);
		}

		[Test]
		public void UpdateTest ()
		{
			Node node = new Node (0, "TestInsert", 2, 3);
			node.id = this.nodeModel.InsertAndGetId (node);

			node.name = "TestUpdate";
			node.left = 20;
			node.top = 30;
			node.inputCapacity = 45;
			node.outputCapacity = 56;
			this.nodeModel.Update (node);
			string sql = "select * from nodes where id = " + node.id.ToString ();
			SQLiteDataReader reader = this.database.ExecuteReader (sql);
			Assert.AreEqual (true, reader.Read ());
			Assert.AreEqual ("TestUpdate", reader ["name"]);
			Assert.AreEqual (45, reader ["inputCapacity"]);
		}

		[Test]
		[ExpectedException]
		public void UpdateEmptyTest ()
		{
			Node node = new Node (0, "TestInsert", 2, 3);
			node.id = this.nodeModel.InsertAndGetId (node);

			node.id = 0;
			this.nodeModel.Update (node);
			string sql = "select * from nodes where id = " + node.id.ToString ();
			SQLiteDataReader reader = this.database.ExecuteReader (sql);
			Assert.AreEqual (false, reader.Read ());
		}

		[Test]
		public void DeleteTest ()
		{
			Node node = new Node (0, "TestInsert", 2, 3);
			node.id = this.nodeModel.InsertAndGetId (node);

			this.nodeModel.Delete (node.id);
		}

		[Test]
		[ExpectedException]
		public void DeleteEmptTest ()
		{
			Node node = new Node (0, "TestInsert", 2, 3);
			node.id = this.nodeModel.InsertAndGetId (node);
			this.nodeModel.Delete (-1);
		}

//		[Test]
//		public void SelectTest ()
//		{
//			for (int i = 0; i < 10; i++) {
//				Node node = new Node (i + 1, i.ToString (), i * 2, i * 4);
//				node.id = this.nodeModel.InsertAndGetId (node);
//			}
//
//			Node[] nodes = this.nodeModel.Select<Node> ();
//			for (int i = 0; i < nodes.Length; i++) {
//				Assert.AreEqual (i + 1, nodes [i].id, "Ids are not equal");
//				Assert.AreEqual (i.ToString (), nodes [i].name);
//				Assert.AreEqual (i * 2, nodes [i].left);
//				Assert.AreEqual (i * 4, nodes [i].top);
//			}
//		}

		public void SpeedTest ()
		{
			for (int i = 0; i < 100; i++) {
				Node node = new Node (0, i.ToString (), i, i);
				node.id = this.nodeModel.InsertAndGetId (node);
			}
		}
	}
}

