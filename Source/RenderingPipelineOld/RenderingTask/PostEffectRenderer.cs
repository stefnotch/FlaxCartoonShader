using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartoonShader.Source.RenderingPipeline.RenderingOutput;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline.RenderingTask
{
	public class PostEffectRenderer : IRenderer
	{
		public PostEffectRenderer(MaterialBase material, Vector2 size, string name = "")
		{
			if (!material.IsPostFx) throw new ArgumentException("PostFx Material expected", nameof(material));
			Name = name;
			Inputs = new MaterialRenderInputs(material);

			Output = new RenderOutput(size, this);

			if (!Task) Task = RenderTask.Create<CustomRenderTask>();
			Task.Enabled = false;

			// TODO: Enable the task!

			Task.OnRender = OnRender;
		}

		public string Name { get; set; }

		public MaterialRenderInputs Inputs { get; }

		public CustomRenderTask Task { get; }

		RenderTask IRenderer.Task => Task;

		public RenderOutput Output { get; }

		public MaterialBase Material => Inputs.Material;

		public PostEffectRenderer SetInput(string name, RenderOutput renderOutput)
		{
			Inputs[name] = renderOutput;
			return this;
		}

		private void OnRender(GPUContext context)
		{
			// TODO: Should I pass in the "default" one? Also, can't you cache that!?
			RenderTarget input = null;
			if (Inputs.TryGetValue(MaterialRenderInputs.DefaultInputName, out var renderOutput))
			{
				input = renderOutput?.RenderTarget;
			}

			context.DrawPostFxMaterial(Inputs.Material, Output.RenderTarget, input);
		}

		#region IDisposable Support

		private bool _disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					if (Task)
					{
						Task.Enabled = false;
					}
					Task?.Dispose();
					FlaxEngine.Object.Destroy(Task);
					FlaxEngine.Object.Destroy(Output.RenderTarget);
					Inputs.Material = null;
				}
				_disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}

		#endregion IDisposable Support
	}
}