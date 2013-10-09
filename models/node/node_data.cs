using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace Model
{
	public struct Node: IData
	{
		public long id;
		public string name;
		public double left;
		public double top;
		public bool invalid;
		public int inputCapacity;
		public int outputCapacity;

		public Node (bool invalid = true)
		{
			this.invalid = invalid;
			this.id = 0;
			this.name = "";
			this.left = 0;
			this.top = 0;
			this.inputCapacity = 0;
			this.outputCapacity = 0;
		}

		public Node (long id, string name, double left, double top)
		{
			this.id = id;
			this.name = name;
			this.left = left;
			this.top = top;
			this.invalid = false;
			this.inputCapacity = 0;
			this.outputCapacity = 0;
		}

		public Node (SQLiteDataReader reader)
		{
			this.id = (long)reader ["id"];
			this.name = reader ["name"].ToString ();
			this.left = (double)reader ["left"];
			this.top = (double)reader ["top"];
			this.invalid = false;
			this.inputCapacity = (int)(long)reader ["InputCapacity"];
			this.outputCapacity = (int)(long)reader ["OutputCapacity"];
		}

		public object[] SqlParameters ()
		{
			object[] result = new object[12] {
				"id",
				this.id,
				"name",
				this.name,
				"left",
				this.left,
				"top",
				this.top,
				"InputCapacity", this.inputCapacity,
				"OutputCapacity", this.outputCapacity
			};
			return result;
		}

		public void Move(double left, double top)
		{
			this.left = left;
			this.top = top;
		}

		public static double operator - (Node n1, Point p2) 
		{
			double result = Math.Pow(p2.X - n1.left, 2)+ Math.Pow(p2.Y - n1.top, 2);
			return Math.Sqrt(result);
		}


		public static double operator - (Node n1, Node n2) 
		{
			double result = Math.Pow(n2.left - n1.left, 2)+ Math.Pow(n2.top - n1.top, 2);
			return Math.Sqrt(result);
		}

		public override string ToString ()
		{
			return string.Format ("[Node: id={0}, name={1}, left={2}, top={3}]", id, name, left, top);
		}


	}

}

