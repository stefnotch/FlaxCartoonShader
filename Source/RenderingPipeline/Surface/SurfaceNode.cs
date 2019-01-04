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

			var header = new TextBox()
			{
				IsReadOnly = true,
				AnchorStyle = AnchorStyle.Upper,
				DockStyle = DockStyle.Top,
				Text = graphNode.Value.GetType().Name,
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

		private async Task InitializeLater(Image image)
		{
			ActionRunner.Instance.AfterFirstUpdate(() =>
			{
				if (GraphNode?.Value == null) return;

				if (GraphNode.Value is IRenderer renderer)
				{
					if (renderer.DefaultOutput?.RenderTarget == null) return;

					image.Brush = new RenderTargetBrush(renderer.DefaultOutput.RenderTarget);
				}
				else if (GraphNode.Value is IRendererDisplayer rendererDisplayer)
				{
					var firstInput = rendererDisplayer.Inputs.First().Value;
					if (firstInput?.RenderTarget == null) return;

					image.Brush = new RenderTargetBrush(firstInput.RenderTarget);
				}
			});
		}

		public RenderPipelineSurface Surface { get; }
		public GraphNode GraphNode { get; }
	}
}