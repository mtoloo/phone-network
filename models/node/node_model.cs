using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace Model
{
	public class NodeModel: Model, IModel
	{
		static string TABLE_NAME = "nodes";

		public NodeModel (Database database, bool createTableIfNotExists = false): 
			base(database, NodeModel.TABLE_NAME, createTableIfNotExists)
		{
		}

		public override void CreateTable ()
		{
			string sql = @"CREATE TABLE nodes(id INTEGER PRIMARY KEY AUTOINCREMENT,
name VARCHAR, left FLOAT, top FLOAT, inputCapacity INTEGER, outputCapacity INTEGER);";
			this.database.ExecuteNonQuery (sql);
		}

		public Node[] Select ()
		{
			int count;
			SQLiteDataReader reader = this.SelectReader(out count);
			Node[] result = new Node[count];
			int i = 0;
			while (reader.Read()) {
				result [i++] = new Node(reader);
			}
			return result;
		}

	}
}

