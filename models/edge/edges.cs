using System;
using System.Collections.Generic;

namespace Model
{
	public class Edges
	{
		static int MinimumDistance = 5;

		EdgeModel model;
		List<long> selectedIds;
		public Dictionary<long, Edge> edges;

		public Edges (Database database, bool createTableIfNotExists, Nodes nodes)
		{
			Edge.nodes = nodes;
			this.model = new EdgeModel(database, createTableIfNotExists);
			this.edges = new Dictionary<long, Edge>();
			this.selectedIds = new List<long>();
		}

		public void LoadAll()
		{
			Edge[] edgesArr = this.model.Select();
			this.edges.Clear();
			foreach(Edge edge in edgesArr)
				this.edges.Add(edge.id, edge);
		}

		public Edge CreateNew(Node node1, Node node2)
		{
			if (node1.id == 0)
				throw new Exception("Node " + node1.name + " has not been saved");
			if (node2.id == 0)
				throw new Exception("Node " + node2.name + " has not been saved");
			Edge edge = new Edge(0, node1.id, node2.id, node2 - node1);
			edge.id = this.model.InsertAndGetId(edge);
			this.edges.Add(edge.id, edge);
			return edge;
		}

		public void DeleteEdge (Edge edge)
		{
			this.model.Delete(edge.id);
			this.edges.Remove(edge.id);
			this.ClearSelection();
		}

		public void DeleteSelected ()
		{
			this.DeleteEdge(this.FirstSelected());
		}
		//Delete all edges related to given node
		public void DeleteNodeEdges(Node node)
		{
			for(int i = 0 ; i < this.edges.Count; ) {
				Edge edge = this.edges[i];
				if (edge.node1.id == node.id || edge.node2.id == node.id)
					DeleteEdge(edge);
				else i++;
			}
		}

		public bool IsSelected (Edge edge)
		{
			return this.selectedIds.IndexOf(edge.id)>= 0;
		}
		public bool SelectAtPoisition (PointD point, double minimumDistance)
		{
			double minimum = 1000;
			Edge result = new Edge();
			foreach (Edge edge in edges.Values) {
				double distance = edge - point;
				if (distance < minimum) {
					minimum = distance;
					result = edge;
				}
			}
			if (minimum < minimumDistance) {
				this.selectedIds.Add(result.id);
				return true;
			}
			return false;
		}

		void ClearSelection ()
		{
			this.selectedIds.Clear();
		}
		public bool SelectAtPoisition (PointD point, bool clearPreviouseSelection)
		{
			if (clearPreviouseSelection)
				this.ClearSelection();
			return this.SelectAtPoisition(point, Edges.MinimumDistance);
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

		public Edge FirstSelected ()
		{
			this.CheckSelected ();
			return this.edges [this.FirstSelectedId()];
		}


	}
}

