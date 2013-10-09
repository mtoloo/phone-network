using System;
using System.Collections.Generic;

namespace Model
{
	public class Edges
	{
		EdgeModel model;
		List<Edge> selectedEdges;
		public List<Edge> edges {
			get;
			set;
		}

		public Edges (Database database, bool createTableIfNotExists, Nodes nodes)
		{
			Edge.nodes = nodes;
			this.model = new EdgeModel(database, createTableIfNotExists);
			this.edges = new List<Edge>();
			this.selectedEdges = new List<Edge>();
		}

		public void LoadAll(Nodes nodes)
		{
			this.edges.Clear();
			this.edges.AddRange(this.model.Select(nodes));
		}

		public Edge CreateNew(Node node1, Node node2)
		{
			if (node1.id == 0)
				throw new Exception("Node " + node1.name + " has not been saved");
			if (node2.id == 0)
				throw new Exception("Node " + node2.name + " has not been saved");
			Edge edge = new Edge(0, node1, node2, node2 - node1);
			edge.id = this.model.InsertAndGetId(edge);
			this.edges.Add(edge);
			return edge;
		}

		public void DeleteEdge (Edge edge)
		{
			this.model.Delete(edge.id);
			this.edges.Remove(edge);
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


//		public bool SelectAtPoisition (double left, double top, double minimumDistance)
//		{
//			return false;
//
//		}
	}
}

