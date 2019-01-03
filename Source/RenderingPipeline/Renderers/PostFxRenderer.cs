using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace CartoonShader.Source.RenderingPipeline.Renderers
{
	public class PostFxRenderer : RendererWithTask
	{
		private Camera _orthographicCamera;
		private StaticModel _modelActor;
		private Model _model;
		private const float ZPos = 100f;

		[Serialize]
		protected MaterialBase _material;

		[NoSerialize]
		protected MaterialInstance _materialInstance;

		public PostFxRenderer()
		{
			MaterialChanged(_material);
		}

		/// <summary>
		/// The Material that will be used by this <see cref="PostFxRenderer"/> to render something
		/// </summary>
		[NoSerialize]
		public MaterialBase Material
		{
			get => _material;
			set
			{
				if (_material != value)
				{
					_material = value;
					MaterialChanged(_material);
				}
			}
		}

		/// <summary>
		/// Should a quad be generated for every pixel
		/// </summary>
		public bool QuadForEachPixel = false;

		protected override void EnableChanged(bool enabled)
		{
			if (enabled)
			{
				if (!_model)
				{
					// TODO: Can I call this in the constructor?!
					_model = Content.CreateVirtualAsset<Model>();
					_model.SetupLODs(1);
				}
				if (!_modelActor)
				{
					_modelActor = FlaxEngine.Object.New<StaticModel>();
					_modelActor.Model = _model;
					_task.CustomActors.Add(_modelActor);
				}
			}

			base.EnableChanged(enabled);
			if (enabled)
			{
				if (!_orthographicCamera)
				{
					_orthographicCamera = CreateOrthographicCamera();
					_task.Camera = _orthographicCamera;
					_task.CustomActors.Add(_orthographicCamera);
				}

				_task.AllowGlobalCustomPostFx = false;
				_task.ActorsSource = ActorsSources.CustomActors;
				_task.Mode = ViewMode.Emissive;
			}
		}

		private void UpdateMesh(Mesh mesh)
		{
			if (mesh == null) return;

			int width = (int)_size.X;
			int height = (int)_size.Y;

			if (QuadForEachPixel)
			{
				new MeshGenerators.ScreenPixelQuadsGenerator(new Int2(width, height)).Generate(mesh);
			}
			else
			{
				new MeshGenerators.QuadGenerator(_size).Generate(mesh);
			}
		}

		protected override void EnableRenderTask(bool enabled)
		{
			base.EnableRenderTask(enabled);
			if (enabled)
			{
				_modelActor.Entries[0].Material = _materialInstance;
			}
		}

		private Camera CreateOrthographicCamera()
		{
			_orthographicCamera = FlaxEngine.Object.New<Camera>();
			_orthographicCamera.UsePerspective = false;
			_orthographicCamera.NearPlane = 2;
			_orthographicCamera.FarPlane = 1000;
			_orthographicCamera.OrthographicScale = 1;
			_orthographicCamera.LocalPosition = Vector3.Zero;
			return _orthographicCamera;
		}

		protected override void SizeChanged(Vector2 size)
		{
			base.SizeChanged(size);
			if (_modelActor)
			{
				_modelActor.LocalPosition = new Vector3(_size * -0.5f, ZPos);
			}
			if (_model)
			{
				UpdateMesh(_model.LODs[0].Meshes[0]);
			}
		}

		protected virtual void MaterialChanged(MaterialBase material)
		{
			if (material == null) return;

			UpdateMaterialInputs(material);
		}

		private void UpdateMaterialInputs(MaterialBase material)
		{
			material.WaitForLoaded();
			IEnumerable<string> previousRendererInputNames = null;
			if (_materialInstance)
			{
				previousRendererInputNames = GetRendererInputNames(_materialInstance);
				FlaxEngine.Object.Destroy(ref _materialInstance);
			}
			_materialInstance = material.CreateVirtualInstance();
			_inputs.UpdateInputs(GetRendererInputNames(_materialInstance), previousRendererInputNames);

			if (_modelActor) _modelActor.Entries[0].Material = _materialInstance;
		}

		private void UpdateMaterialInput(string name, RenderTarget renderTarget)
		{
			ActionRunner.Instance.AfterFirstUpdate(() =>
			{
				if (name == null || !renderTarget) return;

				if (_materialInstance == null)
				{
					Debug.LogError("Missing material instance");
				}
				var materialParam = _materialInstance.GetParam(name);
				if (materialParam != null)
				{
					// Warning: If you ever change the code to RenderOutput, the compiler won't warn you here...
					materialParam.Value = renderTarget;
				}
			});
		}

		private IEnumerable<string> GetRendererInputNames(MaterialInstance materialInstance)
		{
			if (materialInstance == null) return Enumerable.Empty<string>();

			var inputParameters = materialInstance.Parameters
				.Where(param => param.Type == MaterialParameterType.RenderTarget)
				.Select(param => param.Name);

			return inputParameters;
		}

		protected override void RendererInputChanged(string name, RenderOutput newRendererOutput)
		{
			UpdateMaterialInput(name, newRendererOutput?.RenderTarget);
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected override void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					FlaxEngine.Object.Destroy(ref _materialInstance);
				}
				disposedValue = true;
			}
			base.Dispose(disposing);
		}

		#endregion IDisposable Support
	}
}