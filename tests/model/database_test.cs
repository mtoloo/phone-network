using System;
using NUnit.Framework;
using System.IO;
using System.Collections.Generic;

namespace Model
{
	[TestFixture]
	public class DatabaseTest
	{
		Database database;

		public DatabaseTest ()
		{
			this.database = new Database(":memory:");
		}

		[SetUp]
		public void Init ()
		{
			this.database.CreateOrOpen();
		}

		[TearDown]
		public void Dest ()
		{
			this.database.Close();
		}

		[Test]
		public void ExecuteTest()
		{
			var result = this.database.ExecuteScalarQuery("select 5");
			Assert.AreEqual(5, result);

			KeyValuePair<string, object>[] parameters;
			parameters = new KeyValuePair<string, object>[1];
			parameters[0] = new KeyValuePair<string, object>("v", 6);
			var actual = this.database.ExecuteScalarQuery ("select @v", new object[2] {"v", 6});
			Assert.AreEqual(6, actual);
		}

		[Test]
		public void TableExists()
		{
			this.database.DropTableIfExists("test");
			Assert.AreEqual(false, this.database.tableExists("test"));
			this.database.ExecuteNonQuery("create table test(a int)");
			Assert.AreEqual(true, this.database.tableExists("test"));
			this.database.ExecuteNonQuery("drop table test");
			Assert.AreEqual(false, this.database.tableExists("test"));
		}

		[Test]
		public void VersionTest ()
		{
			this.database.DropTableIfExists("versions");
			Assert.AreEqual("", this.database.Version());
			this.database.VersionAdd("1.0");
			Assert.AreEqual("1.0", this.database.Version());
			this.database.VersionAdd("0.5");
			Assert.AreEqual("0.5", this.database.Version());
		}

		[Test]
		public void ExecuteScriptTest ()
		{
			this.database.DropTableIfExists("script_test");

			string file = "script.sql";
			string contents = @"create table script_test (id int);
				insert into script_test (id) values (1);
				insert into script_test (id) values (2);";
			System.IO.File.WriteAllText(file, contents);
			this.database.ExecuteScript(file);

			// Script which contains error, must rollback
			var result = this.database.ExecuteScalarQuery ("select count(*) from script_test");
			Assert.AreEqual(2, result);
			string contentsError = @"insert into script_test (id) values (3);
insert into script_test2 (id) values (-1);";
			System.IO.File.WriteAllText(file, contentsError);
			try {
				this.database.ExecuteScript (file);
			} catch (Exception) {
			}
			result = this.database.ExecuteScalarQuery ("select count(*) from script_test");
			Assert.AreEqual(2, result, "Including error in script, all the sqls shoud rollback");
		}

		[Test]
		[ExpectedException(typeof(System.Data.SQLite.SQLiteException))]
		public void RelationTest() 
		{
			this.database.DropTableIfExists("artist");
			this.database.DropTableIfExists("track");
			string sql = @"CREATE TABLE artist(
  artistid    INTEGER PRIMARY KEY, 
  artistname  TEXT
);
CREATE TABLE track(
  trackid     INTEGER, 
  trackname   TEXT, 
  trackartist INTEGER,
  FOREIGN KEY(trackartist) REFERENCES artist(artistid)
);";
			this.database.ExecuteScriptContent(sql);
			this.database.ExecuteNonQuery("INSERT INTO track VALUES(14, 'Mr. Bojangles', 3)");
		}
	}
}

