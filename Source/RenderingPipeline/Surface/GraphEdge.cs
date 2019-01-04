using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonShader.Source.RenderingPipeline.Surface
{
	public class GraphEdge
	{
		public GraphNode FromNode;
		public GraphNode ToNode;

		public GraphEdge(GraphNode from, GraphNode to)
		{
			FromNode = from;
			ToNode = to;
		}

		public static GraphEdge CreateBetween(GraphNode from, GraphNode to)
		{
			var graphEdge = new GraphEdge(from, to);
			from.OutputEdges.Add(graphEdge);
			to.InputEdges.Add(graphEdge);
			return graphEdge;
		}
	}
}