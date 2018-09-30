﻿using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader
{
	public class MotionVectorRenderer : Script
	{
		[Limit(1, 100)]
		public float ResolutionDivisor
		{
			get { return _resolutionDivisor; }
			set
			{
				value = Mathf.Clamp(value, 1, 1000);
				if (_resolutionDivisor != value)
				{
					_resolutionDivisor = value;
					if (Camera && output)
					{
						_resolution = Camera.Viewport.Size / _resolutionDivisor;
						output.Init(PixelFormat.R8G8B8A8_UNorm, _resolution);
					}
				}
			}
		}

		public Camera Camera;
		public MaterialBase OutputMaterial;
		public string MaterialParamName { get; private set; } = "Image";

		private Vector2 _resolution;
		private RenderTarget output;
		private SceneRenderTask task;
		private bool _setMaterial;
		private float _resolutionDivisor;

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
			task.Order = -10000;
			task.Camera = Camera;
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

			if (_setMaterial && task?.Buffers)
			{
				_setMaterial = false;

				if (OutputMaterial)
				{
					OutputMaterial.GetParam(MaterialParamName).Value = task.Buffers.MotionVectors;
				}
			}
		}
	}
}