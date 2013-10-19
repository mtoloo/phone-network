using System;
using System.Data.SQLite;
using System.IO;
using System.Collections.Generic;

namespace Model
{
	public class Database
	{
		public SQLiteConnection connection;

		private string fileName;

		public Database (string fileName)
		{
			this.fileName = fileName;
			InitializeConnection (ref fileName);
		}


		void InitializeConnection (ref string fileName)
		{
			string connectionString = "Data Source=" + fileName + ";Version=3;";
			this.connection = new SQLiteConnection (connectionString);
		}

		public void CreateOrOpen()
		{
			if (fileName != ":memory:" && !File.Exists(this.fileName))
			    this.Create();
			this.Open();
		}

		public void migrate (string path)
		{
			string[] files = Directory.GetFiles (path, "*.sql", SearchOption.AllDirectories);
			Array.Sort(files);
			string[] versions = new string[0];
			if (this.tableExists("versions"))
				versions = this.ExecuteArray("select version from versions");

			foreach (string file in files) {
				if (Array.IndexOf(versions, file) >= 0)
					continue;
				this.ExecuteScript(file);
				this.VersionAdd(file);
			}
		}

		public void Create ()
		{
			SQLiteConnection.CreateFile(this.fileName);
		}

		public void Open ()
		{
			if ((int)this.connection.State == 1)
				Console.WriteLine("Connection is already open");
			else
				this.connection.Open();
			this.ExecuteNonQuery("PRAGMA foreign_keys = ON;");
		}

		public void Close ()
		{
			this.connection.Close();
		}

		public void ExecuteNonQuery (string sql)
		{
			SQLiteCommand command = this.connection.CreateCommand();
			command.CommandText = sql;
			command.ExecuteNonQuery();
		}

		public int ExecuteNonQuery (string sql, object[] paramValues)
		{
			SQLiteCommand command = this.connection.CreateCommand();
			command.CommandText = sql;
			UpdateCommandParametersFromStringParameters (ref command, paramValues);
			int recordsAffected =  command.ExecuteNonQuery();
			if (recordsAffected == 0)
				throw new Exception("No records was affected by executing query:" + sql);
			return recordsAffected;
		}

		static void UpdateCommandParametersFromStringParameters (ref SQLiteCommand command, object[] paramValues)
		{
			for (int i = 0; i < paramValues.Length; i++) {
				string key = paramValues [i++].ToString ();
				object value = paramValues [i];
				command.Parameters.AddWithValue (key, value);
			}
		}
		public object ExecuteScalarQuery (string sql, object[] paramValues)
		{
			SQLiteCommand command = this.connection.CreateCommand ();
			command.CommandText = sql;
			UpdateCommandParametersFromStringParameters (ref command, paramValues);
			return command.ExecuteScalar();
		}

		public SQLiteDataReader ExecuteReader (string sql, object[] paramValues)
		{
			SQLiteCommand command = this.connection.CreateCommand ();
			command.CommandText = sql;
			UpdateCommandParametersFromStringParameters (ref command, paramValues);
			return command.ExecuteReader();
		}

		public SQLiteDataReader ExecuteReader (string sql)
		{
			SQLiteCommand command = this.connection.CreateCommand ();
			command.CommandText = sql;
			return command.ExecuteReader();
		}

		public object ExecuteScalarQuery (string sql)
		{
			SQLiteCommand command = this.connection.CreateCommand();
			command.CommandText = sql;
			return command.ExecuteScalar();
		}

		string[] ExecuteArray (string sql)
		{
			SQLiteDataReader dataReader = this.ExecuteReader (sql);
			List<string> result = new List<string>();
			while (dataReader.Read())
				result.Add(dataReader.GetString(0));
			return result.ToArray();
		}

		public void ExecuteScriptContent (string contents)
		{
			string[] sqls = contents.Split (';');
			SQLiteTransaction trans = this.connection.BeginTransaction ();
			try {
				foreach (string sql in sqls) {
					this.ExecuteNonQuery (sql);
				}
				trans.Commit ();
			}
			catch (Exception ex) {
				trans.Rollback ();
				throw ex;
			}
		}
		public void ExecuteScript (string file)
		{
			string contents = System.IO.File.ReadAllText (file);
			ExecuteScriptContent (contents);
		}
		public bool tableExists (string tableName)
		{
			string sql = "SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName;";
			var result = this.ExecuteScalarQuery(sql, new object[2] {"tableName", tableName});
			return (result != null);
		}

		void DropTable (string table)
		{
			string sql = "drop table " + table;
			this.ExecuteNonQuery(sql);
		}

		public void DropTableIfExists (string table)
		{
			if (this.tableExists(table))
				this.DropTable(table);
		}

		public string Version ()
		{
			if (this.tableExists ("versions")) {
				string sql = "select version from versions order by date desc, id desc limit 1";
				var result = this.ExecuteScalarQuery (sql);
				if (result == null)
					return "0";
				return result.ToString();
			}
			return "";
		}

		void VersionCreateTable ()
		{
			String sql = "create table versions (id INTEGER PRIMARY KEY AUTOINCREMENT, date DATE, version VARCHAR)";
			this.ExecuteNonQuery(sql);
		}

		public void VersionAdd(string version) 
		{
			if (this.Version().Equals(""))
				this.VersionCreateTable();
			string sql = "insert into versions (date, version) values (@date, @version)";
			string now = DateTime.Now.ToString();
			this.ExecuteNonQuery(sql, new object[4] {"date", now, "version", version});
		}

	}
}

