using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader
{
	public class ToFullscreenQuadRenderer : Script
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
		private RenderTarget _output;
		private SceneRenderTask _task;
		private bool _setMaterial;
		private FullscreenQuad _fullscreenQuad;

		private void OnEnable()
		{
			_resolution = Screen.Size;
			// Create backbuffer
			if (_output == null)
				_output = RenderTarget.New();
			_output.Init(PixelFormat.R8G8B8A8_UNorm, _resolution);

			// Create rendering task
			if (_task == null)
				_task = RenderTask.Create<SceneRenderTask>();
			_task.Order = -100000;
			_task.Camera = SourceCamera;
			_task.Output = _output;
			_task.Enabled = false;

			//task.ActorsSource = ActorsSources.CustomActors;
			//task.AllowGlobalCustomPostFx = false;
			//task.CustomActors =
			//task.CustomPostFx

			_setMaterial = true;

			/*
			 OnEnable is called always when object is created/loaded, Start is called after when the script is active (and the parent actor).
			 */
			_fullscreenQuad = New<FullscreenQuad>();
			_fullscreenQuad.Material = OutputMaterial;

			Actor.AddScript(_fullscreenQuad);
		}

		private void OnDisable()
		{
			// Cleanup
			Destroy(ref _task);
			Destroy(ref _output);
		}

		private void Update()
		{
			if (_resolution != Screen.Size)
			{
				_resolution = Screen.Size;
				if (_output)
				{
					_output.Init(PixelFormat.R8G8B8A8_UNorm, _resolution);
				}
			}

			_task.Enabled = true;

			if (_setMaterial)
			{
				_setMaterial = false;

				if (_fullscreenQuad.Material)
				{
					_fullscreenQuad.Material.GetParam(RenderTargetMaterialParam).Value = _output;
				}
			}
		}
	}
}