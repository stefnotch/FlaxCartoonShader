using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace CartoonShader.Source.MeshGenerators
{
	public class QuadGenerator : IMeshGenerator
	{
		public Vector2 Size { get; }

		public QuadGenerator(Vector2 size)
		{
			Size = size;
		}

		public void Generate(Mesh mesh)
		{
			float width = Size.X;
			float height = Size.Y;

			Vector3[] vertices = new Vector3[6]
				{
					new Vector3(0,0,0),
					new Vector3(0,height,0),
					new Vector3(width,0,0),

					new Vector3(width,0,0),
					new Vector3(0,height,0),
					new Vector3(width,height,0)
				};
			Vector2[] uvs = new Vector2[6]
			{
					new Vector2(0,1),
					new Vector2(0,0),
					new Vector2(1,1),

					new Vector2(1,1),
					new Vector2(0,0),
					new Vector2(1,0)
			};
			int[] triangles = new int[6]
			{
					0,1,2,3,4,5
			};

			mesh.UpdateMesh(vertices, triangles, uv: uvs);
		}
	}
}