using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace Model
{
	public struct Edge: IData
	{
		public long id;
		public static Nodes nodes;
		public long node1_id;
		public Node node1 {get {return nodes.nodes[node1_id];}}
		public long node2_id;
		public Node node2 {get {return nodes.nodes[node2_id];}}
		public double distance;

		public Edge (long id, Node node1, Node node2, double distance)
		{
			this.id = id;
			this.node1_id = node1.id;
			this.node2_id = node2.id;
			this.distance = distance;
		}

		public Edge (SQLiteDataReader reader)
		{
			this.id = (long)reader ["id"];
			this.node1_id = (long)reader ["node1_id"];
			this.node2_id = (long)reader ["node2_id"];
			this.distance = (double)reader ["distance"];
		}

		public object[] SqlParameters ()
		{
			object[] result = new object[8] {
				"id",
				this.id,
				"node1_id",
				this.node1_id,
				"node2_id",
				this.node2_id,
				"distance",
				this.distance
			};
			return result;
		}

//		public static double operator - (Edge edge, Point point) {
//			return 0;
//		}

	}

}

