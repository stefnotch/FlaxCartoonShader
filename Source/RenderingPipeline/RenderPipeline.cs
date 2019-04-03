using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartoonShader.Source.RenderingPipeline.RenderingOutput;
using CartoonShader.Source.RenderingPipeline.RenderingTask;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline
{
	public class RenderPipeline : IDisposable
	{
		private HashSet<IRenderer> _renderers = new HashSet<IRenderer>();
		private bool _enabled;
		private List<MaterialInstance> _materialInstances = new List<MaterialInstance>();

		// TODO: Set size to screen size
		public RenderPipeline(Vector2 defaultSize)
		{
			DefaultSize = defaultSize;
		}

		public Vector2 DefaultSize { get; internal set; }

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				_enabled = value;
				if (_enabled)
				{
					// Order setup
					foreach (var renderer in _renderers)
					{
						renderer.Task.Order = -1000000;
					}
					// Inefficient, but who cares
					foreach (var renderer in _renderers)
					{
						SetupRendererOrder(renderer, renderer.Task.Order);
					}

					Scripting.InvokeOnUpdate(() =>
					{
						foreach (var renderer in _renderers)
						{
							renderer.Task.Enabled = _enabled;
						}
					});
				}
			}
		}

		private void SetupRendererOrder(IRenderer renderer, int maxOrder)
		{
			if (renderer.Task.Order > maxOrder)
			{
				renderer.Task.Order = maxOrder;
			}
			foreach (var input in renderer.Inputs.Values.Where(r => r != null))
			{
				SetupRendererOrder(input.Renderer, maxOrder - 1);
			}
		}

		// Ugh, C# still doesn't have a IReadOnly interface for Sets
		public HashSet<IRenderer> Renderers => _renderers;

		// TODO: Properly document the fact that the materials create internal MaterialInstances and dispose of them...
		public RenderOutputToMaterial ShowRenderOutput(RenderOutput renderOutput, Actor actor, MaterialBase material, int affectedEntry = -1)
		{
			material.WaitForLoaded();

			var materialInstance = material;
			if (!material.IsVirtual)
			{
				var instance = material.CreateVirtualInstance();
				_materialInstances.Add(instance);
				materialInstance = instance;
			}
			var renderOutputToMaterial = FlaxEngine.Object.New<RenderOutputToMaterial>();
			renderOutputToMaterial.Material = materialInstance;
			renderOutputToMaterial.RenderOutput = renderOutput;
			renderOutputToMaterial.AffectedEntry = -1;
			renderOutputToMaterial.Actor = actor;
			return renderOutputToMaterial;
		}

		public T AddRenderer<T>(T renderer) where T : IRenderer
		{
			_renderers.Add(renderer);
			return renderer;
		}

		public CameraRenderer AddCameraRenderer(Camera camera, string name = "")
		{
			return AddRenderer(new CameraRenderer(camera, this.DefaultSize, name));
		}

		public PixelsRenderer AddPixelsRenderer(MaterialBase material, string name = "")
		{
			material.WaitForLoaded();

			var materialInstance = material;
			if (!material.IsVirtual)
			{
				var instance = material.CreateVirtualInstance();
				_materialInstances.Add(instance);
				materialInstance = instance;
			}
			return AddRenderer(new PixelsRenderer(materialInstance, this.DefaultSize, name));
		}

		public PostEffectRenderer AddPostEffectRenderer(MaterialBase material, string name = "")
		{
			material.WaitForLoaded();

			var materialInstance = material;
			if (!material.IsVirtual)
			{
				var instance = material.CreateVirtualInstance();
				_materialInstances.Add(instance);
				materialInstance = instance;
			}
			return AddRenderer(new PostEffectRenderer(materialInstance, this.DefaultSize, name));
		}

		#region IDisposable Support

		private bool _disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					Enabled = false;
					// TODO: dispose managed state (managed objects).
					foreach (var renderer in _renderers)
					{
						renderer.Dispose();
					}
					_renderers = null;

					foreach (var instance in _materialInstances)
					{
						FlaxEngine.Object.Destroy(instance);
					}
					_materialInstances.Clear();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

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