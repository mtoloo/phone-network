using System;
using System.Data.SQLite;
using System.Drawing;

namespace Model
{
	public class MapModel: Model
	{
		static string TABLE_NAME = "maps";

		public MapModel (Database database, bool createTableIfNotExists = false): 
			base(database, MapModel.TABLE_NAME, createTableIfNotExists)
		{
		}

		public override void CreateTable ()
		{
			string sql = @"CREATE TABLE maps(id INTEGER PRIMARY KEY AUTOINCREMENT,
name VARCHAR, file VARCHAR, left INTEGER, top INTEGER, width INTEGER, height INTEGER);";
			this.database.ExecuteNonQuery (sql);
		}

		public MapData[] Select ()
		{
			int count;
			SQLiteDataReader reader = this.SelectReader(out count);
			MapData[] result = new MapData[count];
			int i = 0;
			while (reader.Read()) {
				result [i++] = new MapData(reader);
			}
			return result;
		}

		public MapData LoadData(long id)
		{
			string sql = "select * from " + this.table + " where id = @id";
			SQLiteDataReader reader = this.database.ExecuteReader (sql, new object[2] {"id", id});
			MapData result = new MapData(reader);
			return result;
		}


	}
}

