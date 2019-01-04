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
		[NoSerialize]
		private RenderPipeline _renderPipeline;

		public RenderPipelineSurface()
		{
			BackgroundColor = Color.GhostWhite;
		}

		[NoSerialize]
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

		private List<GraphEdge> _graphEdges;

		private void RenderPipelineChanged(RenderPipeline newRenderPipeline)
		{
			var graph = CreateGraph(newRenderPipeline);

			var graphNodes = graph.Item1;
			_graphEdges = graph.Item2;

			if (graphNodes.Count == 0)
			{
				DisposeChildren();
				return;
			}
			AssignX(graphNodes);
			var highestRank = graphNodes.Max(n => n.Position.X);
			List<GraphNode>[] graphNodesInLevel = new List<GraphNode>[highestRank + 1];

			for (int i = 0; i < graphNodesInLevel.Length; i++)
			{
				graphNodesInLevel[i] = graphNodes.FindAll(n => n.Position.X == i);
			}
			SpawnNodes(graphNodesInLevel);
		}

		private void SpawnNodes(List<GraphNode>[] graphNodesInLevel)
		{
			var maxSize = new Vector2(100);
			var padding = new Vector2(20);

			for (int i = 0; i < graphNodesInLevel.Length; i++)
			{
				for (int j = 0; j < graphNodesInLevel[i].Count; j++)
				{
					var graphNode = graphNodesInLevel[i][j];
					graphNode.Position.Y = j;

					// Create the SurfaceNodes..
					Vector2 pos = new Vector2(graphNode.Position.X, graphNode.Position.Y);
					var surfaceNode = new SurfaceNode(this, graphNode, pos * (maxSize + padding), maxSize)
					{
						Parent = this
					};
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

		private Tuple<List<GraphNode>, List<GraphEdge>> CreateGraph(RenderPipeline newRenderPipeline)
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
			var graphEdges = new List<GraphEdge>();
			foreach (var graphNode in graphNodes)
			{
				if (graphNode.Value is IRenderer renderer)
				{
					foreach (var input in renderer.Inputs.Values.Where(input => input != null))
					{
						var inputGraphNode = graphNodes.First(n => n.Value == input.Renderer);
						graphEdges.Add(GraphEdge.CreateBetween(inputGraphNode, graphNode));
					}
				}
				else if (graphNode.Value is IRendererDisplayer rendererDisplayer)
				{
					foreach (var input in rendererDisplayer.Inputs.Values.Where(input => input != null))
					{
						var inputGraphNode = graphNodes.First(n => n.Value == input.Renderer);
						graphEdges.Add(GraphEdge.CreateBetween(inputGraphNode, graphNode));
					}
				}
			}

			return Tuple.Create(graphNodes, graphEdges);
		}

		public override void Draw()
		{
			base.Draw();
			if (_graphEdges == null) return;

			Color color = Color.DarkBlue;
			var boxCenterPos = new Vector2(Box.SideLength / 2f);
			for (int i = 0; i < _graphEdges.Count; i++)
			{
				var edge = _graphEdges[i];

				Vector2 start = edge.FromBox.PointToParent(this, boxCenterPos);
				Vector2 end = edge.ToBox.PointToParent(this, boxCenterPos);

				DrawConnection(ref start, ref end, ref color);
			}
		}

		#region FlaxAPI Copy Pasta

		// Copyright (c) 2012-2018 Wojciech Figat. All rights reserved.
		private void DrawConnection(ref Vector2 start, ref Vector2 end, ref Color color)
		{
			// Calculate control points
			var dst = (end - start) * new Vector2(0.5f, 0.05f);
			Vector2 control1 = new Vector2(start.X + dst.X, start.Y + dst.Y);
			Vector2 control2 = new Vector2(end.X - dst.X, end.Y + dst.Y);

			// Draw line
			Render2D.DrawBezier(start, control1, control2, end, color, 2.2f);

			/*
			// Debug drawing control points
			Vector2 bSize = new Vector2(4, 4);
			Render2D.FillRectangle(new Rectangle(control1 - bSize * 0.5f, bSize), Color.Blue);
			Render2D.FillRectangle(new Rectangle(control2 - bSize * 0.5f, bSize), Color.Gold);
			*/
		}

		// End of Copyright (c) 2012-2018 Wojciech Figat. All rights reserved.

		#endregion FlaxAPI Copy Pasta

		public override void OnDestroy()
		{
			base.OnDestroy();
			_renderPipeline = null;
			_graphEdges = null;
		}
	}
}