using System;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Drawing;
using Simban;
using System.Collections.Generic;
using Model;

namespace PhoneNetwork
{
	public enum MapMode : byte {
		None = 0,
		NewNode = 1,
		EditNode = 2,
		DeleteNode = 3,
		NewEdge = 4,
		EditEdge = 5,
		DeleteEdge = 6,

		EditMovingNode = 21,
		NewEdgeSecondNode = 41,
	};

	public class Map: System.Windows.Forms.PictureBox
	{
		public static string[] MapModeTitles = new String[7] {"None", "New Node", "Edit Node", "Delete Node", "New Edge", "Edit Edge", "Delete Edge"};

		public MapModel mapModel;
		public MapMode mapMode;
		Database database;
		public Nodes nodes;
		Edges edges;
		MapData mapData ;
		PointD mouseDelta;

		public Map (Database database, Nodes nodes, Edges edges)
		{
			this.database = database;
			this.mapModel = new MapModel(this.database, true);
			this.nodes = nodes;
			this.edges = edges;
			this.mapMode = MapMode.None;

			this.MouseClick += new MouseEventHandler(onClick);
			this.MouseDoubleClick += HandleMouseDoubleClick;
			this.MouseDown += new MouseEventHandler(onMouseDown);
			this.MouseMove += new MouseEventHandler(onMouseMove);
			this.MouseUp += new MouseEventHandler(onMouseUp);

		}

		void ShowNodeProperties (Node node)
		{
			NodeForm nodeForm = new NodeForm(node);
			if (nodeForm.ShowDialog() == DialogResult.OK)
				this.nodes.UpdateFirstSelected(nodeForm.node);
		}

		void ShowEdgeProperties (Edge edge)
		{
			EdgeForm edgeForm = new EdgeForm(edge);
			if (edgeForm.ShowDialog() == DialogResult.OK)
				this.edges.Update(edgeForm.edge);
		}
		void HandleMouseDoubleClick (object sender, MouseEventArgs e)
		{
			PointD location = this.MapToPoint (e.Location);
			if (this.mapMode == MapMode.EditNode && this.nodes.SelectAtPoisition(location, true))
				this.ShowNodeProperties(this.nodes.FirstSelected());
			if (this.mapMode == MapMode.EditEdge && this.edges.SelectAtPoisition(location, true))
				this.ShowEdgeProperties(this.edges.FirstSelected());
		}

		public void LoadMap (long id)
		{
			this.mapData = this.mapModel.LoadData(id);
		}

		PointD MapToPoint (Point point)
		{
			PointD result = new PointD((double)point.X * 100 / this.Image.Width,
			                         (double)point.Y * 100 / this.Image.Height);
			return result;
		}

		Point PointToMap (Point point)
		{
			Point result = new Point(point.X * this.Image.Width / 100,
			                         point.Y * this.Image.Height / 100);
			return result;
		}

		void onClick (object sender, MouseEventArgs e)
		{
			PointD location = this.MapToPoint (e.Location);
			switch (this.mapMode) {
			case MapMode.NewNode:
				this.NewNode (location);
				break;
			case MapMode.DeleteNode:
				this.DeleteNode (location);
				break;
			case MapMode.NewEdge:
				this.NewEdge(location);
				break;
			case MapMode.NewEdgeSecondNode:
				this.NewEdgeSecondNode(location);
				break;
			case MapMode.EditEdge:
				this.EditEdge(location);
				break;
			case MapMode.DeleteEdge:
				this.DeleteEdge(location);
				break;
			}
			Invalidate ();
		}

		public void NewNode (PointD location)
		{
				string nodeName = "New node";
				if (Dialog.InputBox("New Node", "Enter name of new node", ref nodeName) == DialogResult.OK) {
					this.nodes.CreateNew (nodeName, location.X, location.Y);
				}
		}		

		void DeleteNode (PointD location)
		{
			if (this.nodes.SelectAtPoisition(location, true))
			{
				Invalidate();
				DialogResult confirm = MessageBox.Show ("Delete the selected node " + this.nodes.FirstSelected().name, "Confrim delete", MessageBoxButtons.YesNo);
				if (confirm == DialogResult.Yes){
					this.nodes.DeleteSelected();
					this.nodes.LoadAll();
					this.edges.LoadAll();
					Invalidate();
				}
			}
		}

		public bool MoveNodeStart (PointD location)
		{
			if (!this.nodes.SelectAtPoisition(location, true))
				return false;
			this.mouseDelta = new PointD(this.nodes.FirstSelected().left - location.X,
			                            this.nodes.FirstSelected().top - location.Y);
			this.mapMode = MapMode.EditMovingNode;
			return true;
		}

		public Point NodePointOnMap(double left, double top)
		{
			Point result = new Point(Convert.ToInt32(left * this.Image.Width / 100 + this.mapData.left),
			                         Convert.ToInt32(top * this.Image.Height / 100 + this.mapData.top));
			return result;
		}

		void DrawNode (PaintEventArgs pe, Node node)
		{
			Color[] colors = new Color[2] {
				Color.FromArgb (50, 50, 255), 
				Color.FromArgb (150, 155, 255)};
			if (this.nodes.IsSelected(node))
				colors = new Color[3] {
					Color.FromArgb (255, 200, 200), 
					Color.FromArgb (200, 150, 150),
					Color.FromArgb (150, 50, 50)
			};
			System.Drawing.Pen pen;
			var point = this.NodePointOnMap (node.left, node.top);
			Rectangle rect = new Rectangle (point, new Size (2, 2));
			foreach (Color color in colors) {
				pen = new System.Drawing.Pen (color);
				pe.Graphics.DrawEllipse (pen, rect);
				rect = new Rectangle(rect.Left - 1, rect.Top - 1, rect.Width + 2, rect.Height + 2);
			}
		}

		void NewEdge (PointD location)
		{
			if (this.nodes.SelectAtPoisition (location, true))
				this.mapMode = MapMode.NewEdgeSecondNode;
		}

		void NewEdgeSecondNode (PointD location)
		{
			if (this.nodes.SelectAtPoisition (location, false)) {
				this.edges.CreateNew (this.nodes.FirstSelected (), this.nodes.SecondSelected ());
				this.nodes.ClearSelection ();
				this.mapMode = MapMode.NewEdge;
			}
		}

		void EditEdge (PointD location)
		{
			this.edges.SelectAtPoisition(location, true);
		}

		void DeleteEdge (PointD location)
		{
			if (this.edges.SelectAtPoisition(location, true))
			{
				Invalidate();
				Edge edge = this.edges.FirstSelected();
				DialogResult confirm = MessageBox.Show ("Delete the edge between " + edge.node1.name + " and " + edge.node2.name + "?", "Confrim delete", MessageBoxButtons.YesNo);
				if (confirm == DialogResult.Yes){
					this.edges.DeleteSelected();
					this.edges.LoadAll();
					Invalidate();
				}
			}
		}

		void DrawEdge (Edge edge, ref PaintEventArgs pe)
		{
			System.Drawing.Pen pen = new System.Drawing.Pen (Color.Blue);
			if (this.edges.IsSelected(edge))
			    pen.Color = Color.Red;
			pe.Graphics.DrawLine (pen, this.NodePointOnMap (edge.node1.left, edge.node1.top), this.NodePointOnMap (edge.node2.left, edge.node2.top));
		}

		protected override void OnPaint (PaintEventArgs pe)
		{
			base.OnPaint (pe);
			if (this.Image == null)
				return;
			foreach (Node node in nodes.nodes.Values) {
				this.DrawNode(pe, node);
			}
			foreach (Edge edge in edges.edges.Values) {
				DrawEdge (edge, ref pe);
			}
		}

		void onMouseDown (object sedner, MouseEventArgs e)
		{
			PointD location = this.MapToPoint (e.Location);
			if (this.mapMode == MapMode.EditNode) {
				this.MoveNodeStart(location);
			}
		}

		public void MoveNodeContinue (PointD location)
		{
			this.nodes.MoveFirstSelectedNode (location.X + this.mouseDelta.X, 
			                                  location.Y + this.mouseDelta.Y);
		}

		void onMouseMove (object sender, MouseEventArgs e)
		{
			PointD location = this.MapToPoint (e.Location);
			if (this.mapMode == MapMode.EditMovingNode && e.Button == System.Windows.Forms.MouseButtons.Left) {
				MoveNodeContinue (location);
				this.Invalidate();
			}
		}

		public void MoveNodeFinish ()
		{
			this.mapMode = MapMode.EditNode;
			this.nodes.UpdateFirstSelected ();
		}

		void onMouseUp (object sender, MouseEventArgs e)
		{
			if (this.mapMode == MapMode.EditMovingNode && e.Button == System.Windows.Forms.MouseButtons.Left) {
//				if (this.nodes.FirstSelected().left != this.
				MoveNodeFinish ();
			}
		}
	}
}

