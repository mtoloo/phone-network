using System;
using NUnit.Framework;
using System.Data.SQLite;
using System.Windows.Forms;
using Model;

namespace PhoneNetwork
{
	[TestFixture]
	public class MapTest
	{
		Database database;
		Map map;

		public MapTest ()
		{
			this.database = new Database (":memory:");
		}

		[SetUp]
		public void Init ()
		{
			this.database.CreateOrOpen ();
			this.database.DropTableIfExists ("maps");
			this.database.DropTableIfExists ("nodes");
			this.database.DropTableIfExists ("edges");
			Nodes nodes = new Nodes (this.database, true);
			Edges edges = new Edges (this.database, true, nodes);
			this.map = new Map (this.database, nodes, edges);
			MapData mapData = new MapData (0, "Test", "map_test.jpg", 0, 0, 100, 100);
			long mapId = this.map.mapModel.InsertAndGetId (mapData);
			this.map.LoadMap(mapId);
		}

		[TearDown]
		public void Dest ()
		{
			this.database.Close ();
		}

		[Test]
		public void TestModeEditNode()
		{
			this.map.mapMode = MapMode.EditNode;
			this.map.nodes.CreateNew("Node1", 10, 20);
			PointD location = new PointD(100, 200);
			this.map.MoveNodeStart(location);
			Assert.AreNotEqual(MapMode.EditMovingNode, this.map.mapMode);

			location = new PointD(11, 21);
			this.map.MoveNodeStart(location);
			Assert.AreEqual(MapMode.EditMovingNode, this.map.mapMode);
			location = new PointD(11 + 4, 21 - 1);
			this.map.MoveNodeContinue(location);
			Assert.AreEqual(10 + 4, this.map.nodes.nodes[1].left);
			Assert.AreEqual(20 - 1, this.map.nodes.nodes[1].top);

			this.map.MoveNodeFinish();
			this.map.nodes.LoadAll();
			Assert.AreEqual(10 + 4, this.map.nodes.nodes[1].left);
			Assert.AreEqual(MapMode.EditNode, this.map.mapMode);
		}
	}
}
