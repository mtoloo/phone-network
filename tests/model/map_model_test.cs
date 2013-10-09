using System;
using NUnit.Framework;
using System.Data.SQLite;

namespace Model
{
	[TestFixture]
	public class MapModelTest
	{
		Database database;
		MapModel mapModel;

		public MapModelTest ()
		{
			this.database = new Database (":memory:");
		}

		[SetUp]
		public void Init ()
		{
			this.database.CreateOrOpen ();
			this.database.DropTableIfExists ("maps");
			this.mapModel = new MapModel (this.database, true);
		}

		[TearDown]
		public void Dest ()
		{
			this.database.Close ();
		}

		[Test]
		public void InsertTest()
		{
			MapData map = new MapData(0, "InsertedMap", "file", 0, 0, 100, 100);
			map.id = this.mapModel.InsertAndGetId(map);

			Assert.AreEqual(this.mapModel.LastInsertId(), map.id);
			string sql = "select * from maps where id = " + map.id.ToString ();
			SQLiteDataReader reader = this.database.ExecuteReader (sql);
			Assert.AreEqual (true, reader.Read ());
			Assert.AreEqual ("InsertedMap", reader ["name"]);
		}

		[Test]
		public void CountTest()
		{
			Assert.AreEqual(0, this.mapModel.Count());
			MapData map = new MapData(0, "MapCount", "file", 0, 0, 100, 100);
			map.id = this.mapModel.InsertAndGetId(map);
			Assert.AreEqual(1, this.mapModel.Count());
		}

		[Test]
		public void LoadAllMaps()
		{
			MapData map1 = new MapData(0, "MapForAll1", "file1", 0, 0, 10, 10);
			map1.id = this.mapModel.InsertAndGetId(map1);
			MapData map2 = new MapData(0, "MapForAll2", "file2", 10, 20, 100, 200);
			map2.id = this.mapModel.InsertAndGetId(map2);

			MapData[] maps = new MapData[2];
			maps = this.mapModel.Select();
			Assert.AreEqual(2, maps.Length);
			Assert.AreEqual("MapForAll1", maps[0].name);
			Assert.AreEqual("MapForAll2", maps[1].name);
			Assert.AreEqual("file2", maps[1].file);
			Assert.AreEqual(10, maps[1].left);
			Assert.AreEqual(20, maps[1].top);
			Assert.AreEqual(100, maps[1].width);
			Assert.AreEqual(200, maps[1].height);
		}

	}
}

