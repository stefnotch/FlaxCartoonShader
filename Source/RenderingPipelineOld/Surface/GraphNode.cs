using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace CartoonShader.Source.RenderingPipeline.Surface
{
	public class GraphNode
	{
		public object Value;
		public readonly List<GraphEdge> InputEdges = new List<GraphEdge>();
		public readonly List<GraphEdge> OutputEdges = new List<GraphEdge>();
		public Int2 Position = Int2.Zero;
		public bool Visited;

		public GraphNode(object value)
		{
			Value = value;
		}
	}
}