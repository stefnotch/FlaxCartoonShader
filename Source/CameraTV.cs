using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source
{
	public class CameraTV : Script
	{
		public Camera Camera;
		public MaterialBase OutputMaterial;
		public string MaterialParamName { get; private set; } = "Image";

		private Vector2 _resolution;
		private RenderTarget output;
		private SceneRenderTask task;
		private MaterialInstance _materialInstance;
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
			task.Order = 100000;
			task.Camera = Camera;
			task.Output = output;
			task.Enabled = false;

			// Use dynamic material instance
			if (OutputMaterial && _materialInstance == null)
				_materialInstance = OutputMaterial.CreateVirtualInstance();
			_setMaterial = true;
		}

		private void OnDisable()
		{
			// Cleanup
			Destroy(ref task);
			Destroy(ref output);
			Destroy(ref _materialInstance);
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

				if (_materialInstance)
				{
					_materialInstance.GetParam(MaterialParamName).Value = output;
				}

				if (Actor is ModelActor modelActor)
				{
					if (modelActor.HasContentLoaded)
					{
						modelActor.Entries[0].Material = _materialInstance;
					}
				}
			}
		}
	}
}