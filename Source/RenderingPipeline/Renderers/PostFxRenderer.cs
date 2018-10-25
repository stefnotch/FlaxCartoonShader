using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace CartoonShader.Source.RenderingPipeline.Renderers
{
	public class PostFxRenderer : RendererWithTask
	{
		[Serialize]
		protected MaterialBase _material;

		[NoSerialize]
		protected MaterialInstance _materialInstance;

		public override IRendererOutput DefaultOutput => base.DefaultOutput;

		/// <summary>
		/// The Material that will be used by this <see cref="PostFxRenderer"/> to render something
		/// </summary>
		[NoSerialize]
		public MaterialBase Material
		{
			get => _material;
			set
			{
				if (_material != value)
				{
					_material = value;
					MaterialChangedInternal(_material);
				}
			}
		}

		protected override void Enable(bool enabled)
		{
			base.Enable(enabled);
		}

		protected override void OrderChanged(int order)
		{
			base.OrderChanged(order);
		}

		protected override void SizeChanged(Vector2 size)
		{
			base.SizeChanged(size);
		}

		private void MaterialChangedInternal(MaterialBase material)
		{
			if (Enabled) MaterialChanged(material);
		}

		protected virtual void MaterialChanged(MaterialBase material)
		{
			if (material == null) return;

			material.WaitForLoaded();
			if (_materialInstance)
			{
				FlaxEngine.Object.Destroy(ref _materialInstance);
			}
			_materialInstance = material.CreateVirtualInstance();

			//this.Inputs.SetInputs()
			/*
			Dictionary<string, RendererInput> newInputs = new Dictionary<string, RendererInput>();
			AddInputsFromMaterial(newInputs, _materialInstance.Parameters);
			AddInputs(newInputs);
			UpdateInputs(newInputs);
			newInputs.Clear();

			UpdateMaterialInputs();*/
		}

		protected override void RendererInputChanged(string name, IRendererOutput newRendererOutput)
		{
		}

		/*
		 *
		 		/// <summary>
		/// Updates the _materialInstance-RenderTarget-parameter values
		/// </summary>
		private void UpdateMaterialInputs()
		{
			ActionRunner.Instance.AfterFirstUpdate(() =>
			{
				foreach (var input in Inputs.Values)
				{
					UpdateMaterialInput(input);
				}
			});
		}
		*/

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected override void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					//FlaxEngine.Object.Destroy(ref _task);
					//FlaxEngine.Object.Destroy(ref _defaultOutput);
				}
				disposedValue = true;
			}
			base.Dispose(disposing);
		}

		#endregion IDisposable Support
	}
}