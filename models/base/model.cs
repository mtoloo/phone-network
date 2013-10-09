using System;
using System.Data.SQLite;

namespace Model
{
	interface IModel
	{
		bool TableExists ();
		void CheckTableExists ();
		void CreateTable ();
		int Count ();
		long LastInsertId ();
		long InsertAndGetId (IData data);
		void Update (IData data);
	}

	public abstract class Model: IModel
	{
		protected Database database;
		protected string table;

		public Model (Database database, string table, bool createTableIfNotExists = false)
		{
			this.database = database;
			this.table = table;
			if (!this.TableExists ())
				if (createTableIfNotExists)
					this.CreateTable ();
				else
					throw new Exception("Table " + table + " does not exists");
		}

		public abstract void CreateTable ();

		public bool TableExists ()
		{
			return this.database.tableExists (this.table);
		}

		public void CheckTableExists ()
		{
			if (!this.TableExists ())
				throw new Exception (string.Format ("Table {0} does not exists", this.table));
		}

		public int Count ()
		{
			string sql = string.Format ("select count(*) from {0}", this.table);
			var resultStr = this.database.ExecuteScalarQuery (sql).ToString ();
			int resultInt = int.Parse (resultStr);
			return resultInt;
		}

		public void Delete (long id)
		{
			string sql = string.Format ("delete from {0} where id = @id", this.table);
			this.database.ExecuteNonQuery (sql, new object[2] {"id", id});
		}

		public long LastInsertId ()
		{
			string lastIdSql = "select last_insert_rowid()";
			return (long)this.database.ExecuteScalarQuery (lastIdSql);
		}

		static void CommaSeperatedFieldAndValueParameters (ref object[] sqlParams, out string fields, out string valueParameters)
		{
			int size = sqlParams.Length / 2;
			string[] fieldNames = new string[size];
			for (int i = 0; i < sqlParams.Length; i += 2) {
				fieldNames [i / 2] = sqlParams [i].ToString ();
			}
			fields = String.Join (",", fieldNames);
			valueParameters = '@' + String.Join (",@", fieldNames);
		}

		static string FieldAndValueParametersForUpdateSql (ref object[] sqlParams)
		{
			int size = sqlParams.Length / 2;
			string[] fieldNames = new string[size];
			for (int i = 0; i < sqlParams.Length; i += 2) {
				fieldNames [i / 2] = string.Format ("{0} = @{0}", sqlParams [i].ToString ());
			}
			return String.Join (",", fieldNames);
		}

		public long InsertAndGetId (IData data)
		{
			string sql = "insert into {0} ({1}) values ({2})";
			object[] sqlParams = data.SqlParameters ();
			Array.Copy (sqlParams, 2, sqlParams, 0, sqlParams.Length - 2);
			string fieldNames, valueParameters;
			CommaSeperatedFieldAndValueParameters (ref sqlParams, out fieldNames, out valueParameters);
			sql = string.Format (sql, this.table, fieldNames, valueParameters);
			this.database.ExecuteNonQuery (sql, sqlParams);
			return (long)this.LastInsertId ();
		}

		public void Update (IData data)
		{
			string sql = "update {0} set {1} where id = @id";
			object[] sqlParams = data.SqlParameters ();
			string fieldAndValueParameters = FieldAndValueParametersForUpdateSql(ref sqlParams);
			sql = string.Format(sql, this.table, fieldAndValueParameters);
			this.database.ExecuteNonQuery(sql, sqlParams);
		}

		public SQLiteDataReader SelectReader (out int count)
		{
			count = this.Count ();
			string sql = "select * from " + this.table;
			SQLiteDataReader reader = this.database.ExecuteReader (sql, new object[0]);
			return reader;
		}

	}
}

