using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderPipeline
{
	public class RenderTargetToMaterial : IDisposable
	{
		private MaterialInstance _materialInstance;

		/// <summary>
		/// string: Name of the RenderTarget's Material Parameter
		/// </summary>
		public readonly Dictionary<string, RenderTarget> Inputs = new Dictionary<string, RenderTarget>();

		/// <summary>
		/// The material that should be used
		/// </summary>
		public MaterialBase RenderMaterial;

		/// <summary>
		/// If it should create a material instance
		/// </summary>
		public bool ShouldCreateMaterialInstance = true;

		/// <summary>
		/// Will be a material instance if <see cref="ShouldCreateMaterialInstance"/> is true
		/// </summary>
		public MaterialInstance MaterialInstance => _materialInstance;

		/// <summary>
		/// Model Actor
		/// </summary>
		public ModelActor ModelActor;

		public RenderTargetToMaterial()
		{
		}

		public RenderTargetToMaterial(Dictionary<string, RenderTarget> input)
		{
			Inputs = input;
		}

		public void Initialize()
		{
			if (RenderMaterial)
			{
				if (ShouldCreateMaterialInstance)
				{
					_materialInstance = RenderMaterial.CreateVirtualInstance();
					ModelActor.Entries[0].Material = _materialInstance;
				}
				else
				{
					ModelActor.Entries[0].Material = RenderMaterial;
				}
			}
		}

		public void StartRenderTask()
		{
			foreach (var nameAndRenderTarget in Inputs)
			{
				MaterialParameter parameter;
				if (ShouldCreateMaterialInstance)
				{
					parameter = _materialInstance.GetParam(nameAndRenderTarget.Key);
				}
				else
				{
					parameter = RenderMaterial.GetParam(nameAndRenderTarget.Key);
				}
				if (parameter != null)
				{
					parameter.Value = nameAndRenderTarget.Value;
				}
			}
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if (_materialInstance)
					{
						FlaxEngine.Object.Destroy(ref _materialInstance);
					}
				}

				disposedValue = true;
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