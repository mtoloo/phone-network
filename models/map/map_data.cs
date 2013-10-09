using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace Model
{
	public struct MapData: IData
	{
		public long id;
		public string name;
		public string file;
		public long left;
		public long top;
		public long width;
		public long height;

		public MapData (long id, string name, string file, long left, long top, long width, long height)
		{
			this.id = id;
			this.name = name;
			this.file = file;
			this.left = left;
			this.top = top;
			this.width = width;
			this.height = height;
		}

		public MapData (SQLiteDataReader reader)
		{
			this.id = (long)reader ["id"];
			this.name = reader ["name"].ToString ();
			this.left = (long)reader ["left"];
			this.file = reader ["file"].ToString();
			this.top = (long)reader ["top"];
			this.width = (long)reader ["width"];
			this.height = (long)reader ["height"];
		}

		public object[] SqlParameters ()
		{
			object[] result = new object[14] {
				"id",
				this.id,
				"name",
				this.name,
				"file",
				this.file,
				"left",
				this.left,
				"top",
				this.top,
				"width", this.width,
				"height", this.height
			};
			return result;
		}

		public override string ToString ()
		{
			return string.Format ("[Map: id={0}, name={1}, file={2}", id, name, file);
		}


	}

}

