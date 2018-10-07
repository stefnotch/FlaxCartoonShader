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

			Vector3[] vertices = new Vector3[width * height * 6];
			Vector2[] uvs = new Vector2[width * height * 6];
			int[] triangles = new int[width * height * 6];
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					int index = (y + x * height) * 6;

					//TODO: x+1, y+1...
					vertices[index] = new Vector3(x, y, 0);
					vertices[index + 1] = new Vector3(x + 1, y, 0);
					vertices[index + 2] = new Vector3(x, y + 1, 0);

					vertices[index + 3] = new Vector3(x + 1, y, 0);
					vertices[index + 4] = new Vector3(x + 1, y + 1, 0);
					vertices[index + 5] = new Vector3(x, y + 1, 0);

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