﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartoonShader.Source.RenderingPipeline.Renderers;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline
{
	public class RenderPipeline : IDisposable
	{
		private HashSet<IRenderer> _renderers = new HashSet<IRenderer>();
		private HashSet<IRendererDisplayer> _renderDisplayers = new HashSet<IRendererDisplayer>();
		private bool _enabled;

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
					foreach (var renderer in _renderers.OfType<RendererWithTask>())
					{
						renderer.Order = -1000000;
					}
					// Inefficient, but who cares
					foreach (var renderer in _renderers.OfType<RendererWithTask>())
					{
						SetupRendererOrder(renderer, renderer.Order);
					}
				}

				foreach (var renderer in _renderers)
				{
					renderer.Enabled = _enabled;
				}
				foreach (var rendererDisplayer in _renderDisplayers)
				{
					rendererDisplayer.Enabled = _enabled;
				}
			}
		}

		private void SetupRendererOrder(IRenderer renderer, int maxOrder)
		{
			if (renderer is RendererWithTask rendererWithTask && rendererWithTask.Order > maxOrder)
			{
				rendererWithTask.Order = maxOrder;
			}
			foreach (var input in renderer.Inputs.Values)
			{
				SetupRendererOrder(input.Renderer, maxOrder - 1);
			}
		}

		// Ugh, C# still doesn't have a IReadOnly interface for Sets
		public HashSet<IRenderer> Renderers => _renderers;

		public HashSet<IRendererDisplayer> RenderDisplayers => _renderDisplayers;

		public T AddRendererDisplayer<T>(T rendererDisplayer) where T : IRendererDisplayer
		{
			//e.g. RenderToMaterial
			_renderDisplayers.Add(rendererDisplayer);
			return rendererDisplayer;
		}

		public T AddRenderer<T>(T renderer) where T : IRenderer
		{
			// TODO: Only change the renderer size if the renderer size has not been modified yet
			renderer.Size = DefaultSize;

			_renderers.Add(renderer);
			return renderer;
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
					foreach (var rendererDisplayer in _renderDisplayers)
					{
						rendererDisplayer.Dispose();
					}
					_renderDisplayers = null;
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