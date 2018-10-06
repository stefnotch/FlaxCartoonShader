using System;
using System.Collections.Generic;
using FlaxEngine;

namespace CartoonShader
{
	[Tooltip("Expects an orthographic camera")]
	public class FullscreenQuad : Script
	{
		/// <summary>
		/// Material to use for this fullscreen quad
		/// </summary>
		public MaterialBase Material;

		private Model _tempModel;
		private ModelActor _childModel;
		private const float ZPos = 100f;
		private const float Scale = 0.5f;

		private Vector2 _screenSize;

		private void Start()
		{
			// Create dynamic model with a single LOD with one mesh
			_tempModel = Content.CreateVirtualAsset<Model>();
			_tempModel.SetupLODs(1);

			_childModel = Actor.TemporaryChild<ModelActor>();
			_childModel.Model = _tempModel;

			_childModel.Entries[0].Material = Material;

			_childModel.LocalPosition = new Vector3(0, 0, ZPos);
		}

		private void Update()
		{
			if (_screenSize != Screen.Size)
			{
				_screenSize = Screen.Size;
				UpdateSquare(_tempModel.LODs[0].Meshes[0]);
				_childModel.LocalPosition = new Vector3(Screen.Size * -0.5f, ZPos);
			}
		}

		private void OnDestroy()
		{
			Destroy(ref _childModel);
			Destroy(ref _tempModel);
		}

		private void UpdateSquare(Mesh mesh)
		{
			if (mesh == null) return;

			int width = (int)_screenSize.X;
			int height = (int)_screenSize.Y;

			// Vertices and UVs
			int vertexCountX = width + 1;
			int vertexCountY = height + 1;

			Vector3[] vertices = new Vector3[vertexCountX * vertexCountY];
			Vector2[] uvs = new Vector2[vertexCountX * vertexCountY];

			for (int x = 0; x < vertexCountX; x++)
			{
				for (int y = 0; y < vertexCountY; y++)
				{
					int index = y + x * vertexCountY;

					vertices[index] = new Vector3(x, y, 0);
					uvs[index] = new Vector2(x / (float)vertexCountX, 1f - y / (float)vertexCountY);
				}
			}

			// Indices
			int[] triangles = new int[width * height * 6];
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					int vertexIndex = (y + x * vertexCountY);
					int index = (y + x * height) * 6;

					triangles[index] = vertexIndex;
					triangles[index + 1] = vertexIndex + 1;
					triangles[index + 2] = vertexIndex + vertexCountY;

					triangles[index + 3] = vertexIndex + 1;
					triangles[index + 4] = vertexIndex + vertexCountY + 1;
					triangles[index + 5] = vertexIndex + vertexCountY;
				}
			}

			mesh.UpdateMesh(vertices, triangles, uv: uvs);
		}
	}
}