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

		[Test]
		public void MigrationTest()
		{
			string directory = "migrations_test";
			Directory.Delete(directory, true);
			Directory.CreateDirectory(directory);
			string filePath = System.IO.Path.Combine (directory, "script1.sql");
			StreamWriter file = File.CreateText (filePath);
			file.Write(@"create table table1 (id int); 
				create table table2 (name string)");
			file.Close();

			this.database.migrate(directory);
			Assert.AreEqual(filePath, this.database.ExecuteScalarQuery("select version from versions"));

			this.database.ExecuteNonQuery("insert into table1 (id) values (5)");
			Assert.AreEqual(5, this.database.ExecuteScalarQuery("select id from table1"));

			this.database.ExecuteNonQuery("insert into table2 (name) values ('migration')");
			Assert.AreEqual("migration", this.database.ExecuteScalarQuery("select name from table2"));

			//database migration should not run migration twice
			this.database.migrate(directory);
		}

		[Test]
		public void migrateOrderTest()
		{
			//scripts must run in alphabetic order
			string directory = "migrations_test";
			Directory.Delete(directory, true);
			Directory.CreateDirectory(directory);

			string file1Path = System.IO.Path.Combine (directory, "02- script.sql");
			StreamWriter file1 = File.CreateText (file1Path);
			file1.Write(@"alter table table1 add name string");
			file1.Close();

			string file2Path = System.IO.Path.Combine (directory, "01- script.sql");
			StreamWriter file2 = File.CreateText (file2Path);
			file2.Write(@"create table table1 (id int)");
			file2.Close();

			this.database.migrate(directory);
		}

	}
}

