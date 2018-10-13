using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source
{
	public class RenderTargetMaterialOutput
	{
		private MaterialInstance _materialInstance;

		/// <summary>
		/// string: Name of the RenderTarget's Material Parameter
		/// </summary>
		public readonly Dictionary<string, RenderTarget> Input = new Dictionary<string, RenderTarget>();

		/// <summary>
		/// The material that should be used
		/// </summary>
		public MaterialBase RenderMaterial;

		/// <summary>
		/// If it should create a material instance
		/// </summary>
		public bool ShouldCreateMaterialInstance = false;

		/// <summary>
		/// Will be a material instance if <see cref="ShouldCreateMaterialInstance"/> is true
		/// </summary>
		public MaterialInstance MaterialInstance => _materialInstance;

		/// <summary>
		/// Model Actor
		/// </summary>
		public ModelActor ModelActor;

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

			Scripting.Update += Scripting_Update;
		}

		private void Scripting_Update()
		{
			foreach (var nameAndRenderTarget in Input)
			{
				if (ShouldCreateMaterialInstance)
				{
					_materialInstance.GetParam(nameAndRenderTarget.Key).Value = nameAndRenderTarget.Value;
				}
				else
				{
					RenderMaterial.GetParam(nameAndRenderTarget.Key).Value = nameAndRenderTarget.Value;
				}
			}

			Scripting.Update -= Scripting_Update;
		}
	}
}