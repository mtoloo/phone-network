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
		public long capacity;
		private Node fnode1;
		private Node fnode2;

		public Node node1 {
			get { 
				if (!fnode1.invalid)
					return fnode1; 
				return nodes.nodes [node1_id]; 
			}
		}

		public long node2_id;

		public Node node2 {
			get { 
				if (!fnode2.invalid)
					return fnode2;
				return nodes.nodes [node2_id]; 
			}
		}

		public double distance;

		public Edge (long id, long node1_id, long node2_id, double distance)
		{
			this.id = id;
			this.node1_id = node1_id;
			this.node2_id = node2_id;
			this.fnode1 = new Node (true);
			this.fnode2 = new Node (true);
			this.distance = distance;
			this.capacity = 1;
		}

		public Edge (long id, Node node1, Node node2, double distance)
		{
			this.id = id;
			this.fnode1 = node1;
			this.fnode2 = node2;
			this.node1_id = node1.id;
			this.node2_id = node2.id;
			this.distance = distance;
			this.capacity = 1;
		}

		public Edge (SQLiteDataReader reader)
		{
			this.id = (long)reader ["id"];
			this.node1_id = (long)reader ["node1_id"];
			this.node2_id = (long)reader ["node2_id"];
			this.fnode1 = new Node (true);
			this.fnode2 = new Node (true);
			this.distance = (double)reader ["distance"];
			this.capacity = 1;
		}

		public object[] SqlParameters ()
		{
			object[] result = new object[10] {
				"id",
				this.id,
				"node1_id",
				this.node1_id,
				"node2_id",
				this.node2_id,
				"distance",
				this.distance,
				"capacity",
				this.capacity
			};
			return result;
		}

		//Compute the distance from AB to C
		//Thanks for http://www.topcoder.com/tc?d1=tutorials&d2=geometry1&module=Static
		static double LineToPointDistance2D (PointD A, PointD B, PointD C, bool isSegment)
		{
			double dist = ((B - A) ^ (C - A)) / Math.Sqrt ((B - A) * (B - A));
			if (isSegment) {
				double dot1 = (C - B) * (B - A);
				if (dot1 > 0)
					return Math.Sqrt ((B - C) * (B - C));
				double dot2 = (C - A) * (A - B);
				if (dot2 > 0)
					return Math.Sqrt ((A - C) * (A - C));
			}
			return Math.Abs (dist);
		}

		public static double operator - (Edge edge, PointD point)
		{
			double result = LineToPointDistance2D (edge.node1.point, edge.node2.point, point, true);
			return result;
		}
	}

}

