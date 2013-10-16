using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace Model
{
	public class EdgeModel: Model, IModel
	{
		static string TABLE_NAME = "edges";

		public EdgeModel (Database database, bool createTableIfNotExists = false): 
			base(database, EdgeModel.TABLE_NAME, createTableIfNotExists)
		{
		}

		public override void CreateTable ()
		{
			string sql = @"CREATE TABLE edges(id INTEGER PRIMARY KEY AUTOINCREMENT, node1_id INTEGER,
node2_id INTEGER, distance FLOAT, capacity INTEGER,
 FOREIGN KEY(node1_id) REFERENCES nodes(id) ON DELETE CASCADE,
 FOREIGN KEY(node2_id) REFERENCES nodes(id) ON DELETE CASCADE);";
			this.database.ExecuteNonQuery (sql);
		}

		public Edge[] Select ()
		{
			int count;
			SQLiteDataReader reader = this.SelectReader(out count);
			Edge[] result = new Edge[count];
			int i = 0;
			while (reader.Read()) {
				result [i++] = new Edge(reader);
			}
			return result;
		}

	}
}

