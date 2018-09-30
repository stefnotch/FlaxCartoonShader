using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader
{
	public class MotionVectorRenderer : Script
	{
		public Camera Cam;
		public MaterialBase Material;

		[Limit(1, 100)]
		public float ResolutionDivisor
		{
			get { return resolutionDivisor; }
			set
			{
				value = Mathf.Clamp(value, 1, 100);
				if (resolutionDivisor != value)
				{
					resolutionDivisor = value;
					//resolution = Cam.Viewport.Size;// / resolutionDivisor;
					if (output)
					{
						output.Init(PixelFormat.R8G8B8A8_UNorm, resolution);
					}
				}
			}
		}

		private Vector2 resolution = new Vector2(640, 374);
		private RenderTarget output;
		private SceneRenderTask task;
		private MaterialInstance material;
		private bool setMaterial;
		private float resolutionDivisor;

		private void OnEnable()
		{
			// Create backbuffer
			if (output == null)
				output = RenderTarget.New();
			output.Init(PixelFormat.R8G8B8A8_UNorm, resolution);

			// Create rendering task
			if (task == null)
				task = RenderTask.Create<SceneRenderTask>();
			task.Order = -100;
			task.Camera = Cam;
			task.Output = output;
			task.Enabled = false;

			// Use dynamic material instance
			if (Material && material == null)
				material = Material.CreateVirtualInstance();
			setMaterial = true;
		}

		private void OnDisable()
		{
			// Cleanup
			Destroy(ref task);
			Destroy(ref output);
			Destroy(ref material);
		}

		private void Update()
		{
			task.Enabled = true;

			if (setMaterial)
			{
				setMaterial = false;

				if (material)
				{
					material.GetParam("Image").Value = output;
				}

				if (Actor is ModelActor modelActor)
				{
					if (modelActor.HasContentLoaded)
					{
						modelActor.Entries[0].Material = material;
					}
				}
			}
		}
	}
}