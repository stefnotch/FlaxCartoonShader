using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderPipelineOld
{
	public abstract class Renderer : Script
	{
		[Serialize]
		private Vector2 _size;

		public string Name { get; set; }
		public abstract RenderTarget RenderOutput { get; }

		[NoSerialize]
		public Vector2 Size
		{
			get => _size;
			set
			{
				if (_size != value)
				{
					_size = value;
					SizeChanged(_size);
				}
			}
		}

		public abstract void Initialize(Vector2 size, int order = -100000);

		protected abstract void SizeChanged(Vector2 newSize);
	}
}