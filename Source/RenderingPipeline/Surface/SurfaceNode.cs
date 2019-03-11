using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.GUI;

namespace CartoonShader.Source.RenderingPipeline.Surface
{
	public class SurfaceNode : ContainerControl
	{
		public SurfaceNode(RenderPipelineSurface surface, GraphNode graphNode, Vector2 location, Vector2 size) : base(location, size)
		{
			Surface = surface;
			BackgroundColor = FlaxEngine.Color.Gray;
			GraphNode = graphNode;

			string name = (graphNode.Value as IRenderer)?.Name ?? graphNode.Value.GetType().Name;

			var header = new TextBox()
			{
				IsReadOnly = true,
				AnchorStyle = AnchorStyle.Upper,
				DockStyle = DockStyle.Top,
				Text = name,
				Parent = this
			};

			var inputConnections = new VerticalPanel()
			{
				AnchorStyle = AnchorStyle.Left,
				DockStyle = DockStyle.Left,
				Width = Box.SideLength,
				Parent = this
			};
			foreach (var inputEdge in graphNode.InputEdges)
			{
				var box = new Box()
				{
					Parent = inputConnections
				};
				inputEdge.ToBox = box;
			}

			var outputConnections = new VerticalPanel()
			{
				AnchorStyle = AnchorStyle.Right,
				DockStyle = DockStyle.Right,
				Width = Box.SideLength,
				Parent = this
			};
			foreach (var outputEdge in graphNode.OutputEdges)
			{
				var box = new Box()
				{
					Parent = outputConnections
				};
				outputEdge.FromBox = box;
			}

			var image = new Image()
			{
				AnchorStyle = AnchorStyle.BottomCenter,
				DockStyle = DockStyle.Bottom,
				Parent = this
			};
			InitializeLater(image);
		}

		private void InitializeLater(Image image)
		{
			if (GraphNode?.Value == null) return;

			if (GraphNode.Value is IRenderer renderer)
			{
				renderer.DefaultOutput.Subscribe((renderOutput) =>
				{
					if (renderOutput.RenderTarget != null)
					{
						image.Brush = new RenderTargetBrush(renderOutput.RenderTarget);
					}
				},
				() => image.Brush = null);
			}
			else if (GraphNode.Value is IRendererDisplayer rendererDisplayer)
			{
				var firstInput = rendererDisplayer
					.Inputs
					.Select(i => i.Value)
					.Where(i => i != null)
					.DefaultIfEmpty(null)
					.FirstOrDefault();

				firstInput?.Subscribe((renderOutput) =>
				{
					if (renderOutput.RenderTarget != null)
					{
						image.Brush = new RenderTargetBrush(renderOutput.RenderTarget);
					}
				},
				() => image.Brush = null);
			}
		}

		public RenderPipelineSurface Surface { get; }
		public GraphNode GraphNode { get; }
	}
}