using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlaxEngine;
using FlaxEngine.Utilities;

namespace RenderingGraph.Nodes
{
    /// <summary>
    /// Renders a camera or the main camera
    /// </summary>
    /// <remarks>
    /// Make sure to keep this in sync with https://github.com/FlaxEngine/FlaxAPI/blob/master/FlaxEngine/Rendering/SceneRenderTask.cs
    /// </remarks>
    public class PixelsEffectNode : OutputNode
    {
        private ActorsSources _actorsSources = ActorsSources.ScenesAndCustomActors;
        private readonly List<Actor> _customActors = new List<Actor>();
        private RenderBuffers _buffers;
        private RenderView _view;
        private readonly HashSet<PostProcessEffect> _customPostFx = new HashSet<PostProcessEffect>();
        private bool _allowGlobalCustomPostFx = true;
        private readonly HashSet<PostProcessEffect> _postFx = new HashSet<PostProcessEffect>();

        private MaterialBase _material;
        private MaterialInstance _materialInstance;

        private Camera _orthographicCamera;
        private StaticModel _modelActor;
        private Model _model;

        public PixelsEffectNode(NodeDefinition definition) : base(definition)
        {
            
        }

        public override void OnEnable()
        {
            base.OnEnable();
            _view.Init();
            _orthographicCamera = CreateOrthographicCamera();
            SceneManager.SpawnActor(_orthographicCamera);

            _material = Content.Load<MaterialBase>(ParseGuid(Definition.Values[0]));
            if (!_material || !_material.IsSurface) return;
            _materialInstance = _material.CreateVirtualInstance();

            if (!_materialInstance) return;

            _model = Content.CreateVirtualAsset<Model>();
            _model.SetupLODs(1);
            _model.SetupMaterialSlots(1);
            _model.MinScreenSize = 0;
            // TODO: Optimize, use instanced rendering and whatnot
            // GenerateGridMesh(_model.LODs[0].Meshes[0], size);
            _model.LODs[0].Meshes[0].UpdateMesh(new Vector3[]
            {
                new Vector3(0,0,0), 
                new Vector3(100,0,0), 
                new Vector3(100,100,0), 
                new Vector3(0,100,0)
            }, new int[]
            {
                0,3,1,
                1,3,2
            }, uv: new Vector2[]
            {
                new Vector2(0,0), 
                new Vector2(1,0), 
                new Vector2(1,1), 
                new Vector2(0,1), 
            });

            _modelActor = FlaxEngine.Object.New<StaticModel>();
            _modelActor.DrawModes = DrawPass.Depth | DrawPass.GBuffer; // TODO: Optionally enable transparency?
            _modelActor.Model = _model;
            _modelActor.Entries[0].ReceiveDecals = false;
            _modelActor.Entries[0].ShadowsMode = ShadowsCastingMode.None;
            _modelActor.Entries[0].Material = _materialInstance;
            //_modelActor.LocalPosition = new Vector3(size * -0.5f, DistanceFromOrigin);
            SceneManager.SpawnActor(_modelActor);
            _customActors.Add(_modelActor);
        }

        public override void OnDisable()
        {
            _customActors.Clear();
            _customPostFx.Clear();
            FlaxEngine.Object.Destroy(ref _orthographicCamera);
            FlaxEngine.Object.Destroy(ref _materialInstance);
        //    FlaxEngine.Object.Destroy(ref _material);
            base.OnDisable();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (!_buffers) _buffers = RenderBuffers.New();
            _buffers.Size = Output.Size;
            var viewport = new Viewport(Vector2.Zero, _buffers.Size);
            if (_orthographicCamera)
            {
                _view.CopyFrom(_orthographicCamera, ref viewport);
            }

            var parameters = _materialInstance.Parameters;
            for (int i = 0, j = 0; i < parameters.Length; i++)
            {
                if (!parameters[i].IsPublic) continue;

                if (!HasInput(j + 1)) continue;
                parameters[i].Value = GetInput(j + 1);
                j++;
            }

            _postFx.Clear();
            _postFx.AddRange(_customPostFx);
            if (_allowGlobalCustomPostFx)
            {
                _postFx.AddRange(SceneRenderTask.GlobalCustomPostFx);
            }

            if (_orthographicCamera)
            {
                _postFx.AddRange( _orthographicCamera.GetScripts<PostProcessEffect>());
            }

            Context.GPUContext.DrawScene(Context.RenderTask, Output, _buffers, ref _view, _customActors, _actorsSources, _postFx);

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
    }
}
