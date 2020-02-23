using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace RenderingGraph.Nodes
{
    public class PixelsEffectNodeBad : OutputNode
    {
        private const float DistanceFromOrigin = 100f;
        private Vector2 _cachedSize;

        private MaterialParameter[] _inputParameters;
        private MaterialBase _material;
        private MaterialInstance _materialInstance;
        private RenderBuffers _buffers;
        private RenderView _view;
        private readonly List<Actor> _customActors = new List<Actor>();
        private Camera _orthographicCamera;
        private StaticModel _modelActor;
        private Model _model;

        public PixelsEffectNodeBad(NodeDefinition definition) : base(definition)
        {
        }

        public override void OnEnable()
        {
            base.OnEnable();
            _material = Content.Load<MaterialBase>(ParseGuid(Definition.Values[0]));
            if (!_material || !_material.IsSurface) return;
            _materialInstance = _material.CreateVirtualInstance();

            if (!_materialInstance) return;

            var instanceParameters = _materialInstance.Parameters;

            int parameterCount = 0;
            for (int i = 0; i < instanceParameters.Length; i++)
            {
                if (!instanceParameters[i].IsPublic) continue;
                parameterCount++;
            }

            _inputParameters = new MaterialParameter[parameterCount];
            for (int i = 0, j = 0; i < instanceParameters.Length; i++)
            {
                if (!instanceParameters[i].IsPublic) continue;
                _inputParameters[j] = instanceParameters[i];
                j++;
            }

            var size = Size;
            _orthographicCamera = CreateOrthographicCamera();
            _model = Content.CreateVirtualAsset<Model>();
            _model.SetupLODs(1);
            _model.SetupMaterialSlots(1);
            _model.MinScreenSize = 0;
            // TODO: Optimize, use instanced rendering and whatnot
            GenerateGridMesh(_model.LODs[0].Meshes[0], size);
            _cachedSize = size;
            _modelActor = FlaxEngine.Object.New<StaticModel>();
            SceneManager.SpawnActor(_orthographicCamera);
            _modelActor.Model = _model;
            _modelActor.DrawModes = DrawPass.Depth | DrawPass.GBuffer; // TODO: Optionally enable transparency?
            _modelActor.Entries[0].ReceiveDecals = false;
            _modelActor.Entries[0].ShadowsMode = ShadowsCastingMode.None;
            _modelActor.Entries[0].Material = _materialInstance;
            _modelActor.LocalPosition = new Vector3(size * -0.5f, DistanceFromOrigin);

            _customActors.Add(_modelActor);
            _view.Init();
            _view.Mode = ViewMode.Emissive;
            _view.Flags = ViewFlags.None;
            _view.MaxShadowsQuality = Quality.Low;
            _view.TemporalAAJitter = Vector4.Zero;
        }

        public override void OnDisable()
        {
            _inputParameters = null;
            FlaxEngine.Object.Destroy(ref _orthographicCamera);
            FlaxEngine.Object.Destroy(ref _modelActor);
            FlaxEngine.Object.Destroy(ref _model);
            FlaxEngine.Object.Destroy(ref _materialInstance);
            FlaxEngine.Object.Destroy(ref _material);
            base.OnDisable();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (_materialInstance && _material && _materialInstance.BaseMaterial)
            {
                for (int i = 0; i < _inputParameters.Length; i++)
                {
                    if (!HasInput(i + 1)) continue;
                    _inputParameters[i].Value = GetInput(i + 1);
                }

                var size = Size;
                _modelActor.LocalPosition = new Vector3(size.X * -0.5f, size.Y * -0.5f, DistanceFromOrigin);
                if (_cachedSize != size)
                {
                    // TODO: Execute this asynchronously
                    GenerateGridMesh(_model.LODs[0].Meshes[0], size);
                    _cachedSize = size;
                }

                if (!_buffers) _buffers = RenderBuffers.New();
                _buffers.Size = size;
                var viewport = new Viewport(Vector2.Zero, _buffers.Size);
                _view.CopyFrom(_orthographicCamera, ref viewport);

                Context.GPUContext.DrawScene(Context.RenderTask, Output, _buffers, ref _view, _customActors, ActorsSources.ScenesAndCustomActors);
            }
            else
            {
                Context.GPUContext.Clear(Output, Color.Zero);
            }

            Return(0, Output);
        }

        private static Camera CreateOrthographicCamera()
        {
            var orthographicCamera = FlaxEngine.Object.New<Camera>();
            orthographicCamera.NearPlane = 2;
            orthographicCamera.FarPlane = 1000;
            orthographicCamera.OrthographicScale = 1;
            orthographicCamera.LocalPosition = Vector3.Zero;
            orthographicCamera.UsePerspective = false;
            return orthographicCamera;
        }

        private static void GenerateGridMesh(Mesh mesh, Vector2 size)
        {
            if (mesh == null) return;

            int width = Math.Max((int)size.X, 1);
            int height = Math.Max((int)size.Y, 1);

            var vertices = new Vector3[width * height * 4];
            var uvs = new Vector2[width * height * 4];
            var triangles = new int[width * height * 6];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int index = (y + x * height) * 4;

                    vertices[index] = new Vector3(x, y, 0);
                    vertices[index + 1] = new Vector3(x + 1, y, 0);
                    vertices[index + 2] = new Vector3(x + 1, y + 1, 0);
                    vertices[index + 3] = new Vector3(x, y + 1, 0);

                    // Half pixel offset so that we always sample the center of the pixels
                    Vector2 uv = new Vector2(
                            (x + 0.5f) / (float)width,
                            1f - (y + 0.5f) / (float)height
                        );
                   
                    for (int i = 0; i < 4; i++)
                    {
                        uvs[index + i] = uv;
                    }

                    int triangleIndex = (y + x * height) * 6;
                    triangles[triangleIndex] = index;
                    triangles[triangleIndex + 1] = index + 3;
                    triangles[triangleIndex + 2] = index + 1;

                    triangles[triangleIndex + 3] = index + 1;
                    triangles[triangleIndex + 4] = index + 3;
                    triangles[triangleIndex + 5] = index + 2;
                }
            }

            mesh.UpdateMesh(vertices, triangles, uv: uvs);
        }
    }
}
