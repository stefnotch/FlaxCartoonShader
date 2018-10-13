using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace CartoonShader.Source.MeshGenerators
{
	public class ScreenPixelQuadsGenerator : IMeshGenerator
	{
		public Int2 Size { get; }

		public ScreenPixelQuadsGenerator(Int2 size)
		{
			Size = size;
		}

		public void Generate(Mesh mesh)
		{
			int width = Size.X;
			int height = Size.Y;

			Vector3[] vertices = new Vector3[width * height * 6];
			Vector2[] uvs = new Vector2[width * height * 6];
			int[] triangles = new int[width * height * 6];
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					int index = (y + x * height) * 6;

					vertices[index] = new Vector3(x, y, 0);
					vertices[index + 1] = new Vector3(x, y + 1, 0);
					vertices[index + 2] = new Vector3(x + 1, y, 0);

					vertices[index + 3] = new Vector3(x + 1, y, 0);
					vertices[index + 4] = new Vector3(x, y + 1, 0);
					vertices[index + 5] = new Vector3(x + 1, y + 1, 0);

					Vector2 uv = new Vector2(
							(x + 0.5f) / (float)width,
							1f - (y + 0.5f) / (float)height
						);

					for (int i = 0; i < 6; i++)
					{
						uvs[index + i] = uv;
					}
				}
			}

			for (int i = 0; i < triangles.Length; i++)
			{
				triangles[i] = i;
			}

			mesh.UpdateMesh(vertices, triangles, uv: uvs);
		}
	}
}