using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.GUI;

namespace CartoonShader.Source.RenderingPipeline.Surface
{
	public class RenderPipelineSurface : ContainerControl
	{
		private RenderPipeline _renderPipeline;

		public RenderPipeline RenderPipeline
		{
			get
			{
				return _renderPipeline;
			}
			set
			{
				_renderPipeline = value;
				RenderPipelineChanged(_renderPipeline);
			}
		}

		private TextBox textBox;

		public RenderPipelineSurface()
		{
			textBox = new TextBox()
			{
				Parent = this
			};
			textBox.DockStyle = DockStyle.Fill;
			textBox.IsMultiline = true;
			textBox.PerformLayout();
		}

		private void RenderPipelineChanged(RenderPipeline newRenderPipeline)
		{
			var graphNodes = CreateGraph(newRenderPipeline);
			if (graphNodes.Count == 0)
			{
				textBox.Clear();
				return;
			}
			AssignX(graphNodes);
			var highestRank = graphNodes.Max(n => n.Position.X);
			List<GraphNode>[] graphNodesInLevel = new List<GraphNode>[highestRank + 1];

			for (int i = 0; i < graphNodesInLevel.Length; i++)
			{
				graphNodesInLevel[i] = graphNodes.FindAll(n => n.Position.X == i);
			}
			DrawNodes(graphNodesInLevel);
		}

		private void DrawNodes(List<GraphNode>[] graphNodesInLevel)
		{
			var maxSize = new Int2(100);

			for (int i = 0; i < graphNodesInLevel.Length; i++)
			{
				for (int j = 0; j < graphNodesInLevel[i].Count; j++)
				{
					var graphNode = graphNodesInLevel[i][j];
					graphNode.Position.Y = j;

					//graphNode.Position *= maxSize;
					// Create a child and attach it to this....
				}
			}
		}

		private void AssignX(List<GraphNode> graphNodes)
		{
			GraphNode startingNode = graphNodes.First();

			foreach (var node in graphNodes)
			{
				CalculateRanks(node, graphNodes);
			}
		}

		private void CalculateRanks(GraphNode node, List<GraphNode> graphNodes)
		{
			SetRankRecursive(node, 0);

			foreach (var graphNode in graphNodes)
			{
				graphNode.Visited = false;
			}
		}

		private void SetRankRecursive(GraphNode node, int newRank)
		{
			if (node.Visited) return;
			node.Visited = true;
			if (newRank > node.Position.X)
			{
				node.Position.X = newRank;
			}
			foreach (var outputEdge in node.OutputEdges)
			{
				var nextNode = outputEdge.ToNode;
				SetRankRecursive(nextNode, node.Position.X + 1);
			}
		}

		private List<GraphNode> CreateGraph(RenderPipeline newRenderPipeline)
		{
			// List of GraphNodes
			var graphNodes = new List<GraphNode>();
			foreach (var renderer in newRenderPipeline.Renderers)
			{
				graphNodes.Add(new GraphNode(renderer));
			}
			foreach (var rendererDisplayer in newRenderPipeline.RenderDisplayers)
			{
				graphNodes.Add(new GraphNode(rendererDisplayer));
			}
			// Graph Edges
			foreach (var graphNode in graphNodes)
			{
				if (graphNode.Value is IRenderer renderer)
				{
					foreach (var input in renderer.Inputs.Values)
					{
						var inputGraphNode = graphNodes.First(n => n.Value == input.Renderer);
						GraphEdge.CreateBetween(inputGraphNode, graphNode);
					}
				}
				else if (graphNode.Value is IRendererDisplayer rendererDisplayer)
				{
					foreach (var input in rendererDisplayer.Inputs.Values)
					{
						var inputGraphNode = graphNodes.First(n => n.Value == input.Renderer);
						GraphEdge.CreateBetween(inputGraphNode, graphNode);
					}
				}
			}

			return graphNodes;
		}
	}
}