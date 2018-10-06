using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source
{
	public class SceneRenderer : Script
	{
		/// <summary>
		/// Camera source
		/// </summary>
		public Camera SourceCamera;

		/// <summary>
		/// Normal Material that will be applied to the fullscreen quad
		/// </summary>
		public MaterialBase OutputMaterial;

		/// <summary>
		/// The name of the RenderTarget
		/// </summary>
		[Tooltip("The name of the RenderTarget")]
		public string RenderTargetMaterialParam { get; set; } = "Image";

		private Vector2 _resolution;
		private RenderTarget output;
		private SceneRenderTask task;
		private bool _setMaterial;

		private void OnEnable()
		{
			_resolution = Screen.Size;
			// Create backbuffer
			if (output == null)
				output = RenderTarget.New();
			output.Init(PixelFormat.R8G8B8A8_UNorm, _resolution);

			// Create rendering task
			if (task == null)
				task = RenderTask.Create<SceneRenderTask>();
			task.Order = -100000;
			task.Camera = SourceCamera;
			task.Output = output;
			task.Enabled = false;

			_setMaterial = true;
		}

		private void OnDisable()
		{
			// Cleanup
			Destroy(ref task);
			Destroy(ref output);
		}

		private void Update()
		{
			if (_resolution != Screen.Size)
			{
				_resolution = Screen.Size;
				if (output)
				{
					output.Init(PixelFormat.R8G8B8A8_UNorm, _resolution);
				}
			}

			task.Enabled = true;

			if (_setMaterial)
			{
				_setMaterial = false;

				if (OutputMaterial)
				{
					OutputMaterial.GetParam(RenderTargetMaterialParam).Value = output;
				}
			}
		}
	}
}