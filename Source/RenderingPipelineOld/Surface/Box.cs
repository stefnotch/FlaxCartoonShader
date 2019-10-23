using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.GUI;

namespace CartoonShader.Source.RenderingPipeline.Surface
{
	public class Box : Control
	{
		public const int SideLength = 15;

		public Box()
		{
			this.Size = new Vector2(SideLength);
		}

		public override void Draw()
		{
			base.Draw();
			int padding = 3;
			int size = SideLength - padding * 2;
			Rectangle rectangle = new Rectangle(padding, padding, size, size);
			Render2D.DrawRectangle(rectangle, Color.DarkBlue);
		}
	}
}