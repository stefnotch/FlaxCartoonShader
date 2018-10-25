﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace CartoonShader.Source.RenderingPipeline
{
	public interface IRenderer : IDisposable
	{
		RendererOutputs Outputs { get; }

		RendererInputs Inputs { get; }

		Vector2 Size { get; set; }

		bool Enabled { get; set; }
	}
}