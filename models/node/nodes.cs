using System;
using System.Collections.Generic;
using System.Drawing;

namespace Model
{
	public class Nodes
	{
//		Database database;
		NodeModel model;
		public Dictionary<long, Node> nodes;
		List<long> selectedIds;

		public static double minimumDistance = 5;

		public Nodes (Database database, bool createTableIfNotExists)
		{
//			this.database = database;
			this.model = new NodeModel (database, createTableIfNotExists);
			this.nodes = new Dictionary<long, Node>();
			this.selectedIds = new List<long> ();
		}

		public void LoadAll ()
		{
			Node[] nodesArr = this.model.Select();
			this.nodes.Clear();
			foreach(Node node in nodesArr)
				this.nodes.Add(node.id, node);
		}

		public Node CreateNew (string name, double left, double top)
		{
			Node node = new Node (0, name, left, top);
			node.id = this.model.InsertAndGetId (node);
			this.nodes.Add (node.id, node);
			return node;
		}

		public void ClearSelection ()
		{
			this.selectedIds.Clear ();
		}
		public bool SelectAtPoisition (double left, double top, double minimumDistance)
		{
			double minDistance = 100000, distance;
			Node nearestNode = new Node (true);
			Node locationNode = new Node (0, "dummy", left, top);
			foreach (Node node in nodes.Values) {
				distance = node - locationNode;
				if (distance < minDistance && distance < minimumDistance) {
					minDistance = distance;
					nearestNode = node;
				}
			}
			if (!nearestNode.invalid) {
				this.selectedIds.Add (nearestNode.id);
				return true;
			}
			return false;
		}

		public bool SelectAtPoisition (PointD location, bool clearPreviouseSelection = true)
		{
			if (clearPreviouseSelection)
				this.ClearSelection();
			return this.SelectAtPoisition(location.X, location.Y, Nodes.minimumDistance);
		}

		public bool Select (Node aNode)
		{
			this.selectedIds.Clear ();
			foreach (Node node in nodes.Values) {
				if (aNode.id == node.id) {
					this.selectedIds.Add (node.id);
					return true;
				}
			}
			return false;
		}

		public bool IsSelected (Node node)
		{
			return this.selectedIds.IndexOf(node.id)>= 0;
		}

		Node FindById (long id)
		{
			foreach (Node node in nodes.Values) {
				if (id == node.id) {
					return node;
				}
			}
			throw new Exception ("Could not find node with id " + id.ToString ());
		}

		public int SelectedCount()
		{
			return this.selectedIds.Count;
		}
		void CheckSelected ()
		{
			if (this.selectedIds.Count == 0)
				throw new Exception ("No node is currently selected");
		}

		long FirstSelectedId ()
		{
			return this.selectedIds[0];
		}

		public Node FirstSelected ()
		{
			CheckSelected ();
			return this.nodes [this.FirstSelectedId()];
		}

		long SecondSelectedId ()
		{
			if (this.selectedIds.Count < 2)
				throw new Exception ("Second node is not currently selected");
			return this.selectedIds[1];
		}

		public Node SecondSelected ()
		{
			CheckSelected ();
			return this.nodes [this.SecondSelectedId()];
		}

		public void MoveFirstSelectedNode (double left, double top)
		{
			CheckSelected ();
			long id = FirstSelectedId();
			Node node = this.nodes [id];
			node.Move(left, top);
			this.nodes [id] = node;
		}

		public void UpdateFirstSelected (Node node)
		{
			this.model.Update(node);
			long id = FirstSelectedId();
			this.nodes[id] = node;
		}

		public void UpdateFirstSelected ()
		{
			CheckSelected ();
			long id = FirstSelectedId();
			Node node = this.nodes [id];
			this.UpdateFirstSelected(node);
		}

		public void DeleteSelected ()
		{
			long id = FirstSelectedId ();
			this.model.Delete(id);
			this.nodes.Remove (id);
			this.ClearSelection();
		}

		public bool DeleteAtPoisition (PointD location)
		{
			if (this.SelectAtPoisition (location, true)) {
				this.DeleteSelected ();
				return true;
			} else
				return false;
		}

		public override string ToString ()
		{
			string output = "";
			foreach(Node node in nodes.Values)
				output += "\n" + node.ToString();
			return output;
		}

	}
}

