using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEditor;
using FlaxEditor.GUI;
using FlaxEditor.Surface;
using FlaxEditor.Surface.Elements;
using FlaxEngine;
using NodeGraphs;
using RenderingGraph.Nodes;

namespace RenderingGraph.Editor
{
    public class RenderingGraphSurface : VisjectSurface
    {
        public class MaterialNodeParamsSet : SurfaceNode
        {
            private readonly List<ISurfaceNodeElement> _dynamicChildren = new List<ISurfaceNodeElement>();
            private AssetPicker _assetPicker;
            private MaterialBase _material;
            private readonly Vector2 _baseSize;
            private readonly int _inputCount;
            private readonly int _highestBoxId;

            public MaterialNodeParamsSet(uint id, VisjectSurfaceContext context, NodeArchetype nodeArch,
                GroupArchetype groupArch)
                : base(id, context, nodeArch, groupArch)
            {
                _baseSize = Archetype.Size;
                _inputCount = Archetype.Elements.Count(e => e.Type == NodeElementType.Input);
                _highestBoxId = Archetype.Elements.Max(e => e.BoxID);
            }

            private void UpdateAssetPicker()
            {
                if (_assetPicker == null)
                {
                    _assetPicker = GetChild<AssetPicker>();
                    _assetPicker.SelectedItemChanged += SelectedItemChanged;
                }
            }

            private void UpdateLayout()
            {
                Guid materialGuid = (Guid)Values[0];
                if (_material && materialGuid == _material.ID) return;

                _material = Content.Load<MaterialBase>(materialGuid);
                ClearDynamicElements();
                float height = _baseSize.Y;
                if (_material)
                {
                    int yLevel = _inputCount;
                    int boxId = _highestBoxId + 1;
                    foreach (var parameter in _material.Parameters)
                    {
                        if (!parameter.IsPublic) continue;

                        var connectionType = GetConnectionType(parameter.Type);
                        var archetype = NodeElementArchetype.Factory.Input(yLevel, parameter.Name, true, connectionType, boxId);
                        var element = AddElement(archetype);
                        _dynamicChildren.Add(element);
                        yLevel++;
                        boxId++;
                    }

                    height += Mathf.Max(0, _dynamicChildren.Count - 2) * 20f;
                }
                Resize(_baseSize.X, height);
            }

            private static ConnectionType GetConnectionType(MaterialParameterType parameter)
            {
                if (parameter == MaterialParameterType.Invalid)
                {
                    return ConnectionType.Invalid;
                }

                if (parameter == MaterialParameterType.Bool)
                {
                    return ConnectionType.Bool;
                }

                if (parameter == MaterialParameterType.Integer)
                {
                    return ConnectionType.Integer;
                }

                if (parameter == MaterialParameterType.Float)
                {
                    return ConnectionType.Float;
                }

                if (parameter == MaterialParameterType.Vector2)
                {
                    return ConnectionType.Vector2;
                }

                if (parameter == MaterialParameterType.Vector3)
                {
                    return ConnectionType.Vector3;
                }

                if (parameter == MaterialParameterType.Vector4)
                {
                    return ConnectionType.Vector4;
                }

                if (parameter == MaterialParameterType.Color)
                {
                    return ConnectionType.Vector4;
                }

                // TODO: Automatically cast between GPUTexture and Texture
                if (parameter == MaterialParameterType.Texture || parameter == MaterialParameterType.CubeTexture ||
                    parameter == MaterialParameterType.NormalMap || parameter == MaterialParameterType.SceneTexture ||
                    parameter == MaterialParameterType.GPUTexture ||
                    parameter == MaterialParameterType.GPUTextureArray ||
                    parameter == MaterialParameterType.GPUTextureVolume ||
                    parameter == MaterialParameterType.GPUTextureCube)
                {
                    return ConnectionType.Object;
                }

                if (parameter == MaterialParameterType.Matrix)
                {
                    return ConnectionType.Object;
                }

                if (parameter == MaterialParameterType.ChannelMask)
                {
                    return ConnectionType.Object;
                }

                return ConnectionType.Invalid;
            }

            private void ClearDynamicElements()
            {
                for (int i = 0; i < _dynamicChildren.Count; i++)
                {
                    RemoveElement(_dynamicChildren[i]);
                }

                _dynamicChildren.Clear();
            }

            private void SelectedItemChanged()
            {
                UpdateLayout();
            }

            /// <inheritdoc />
            public override void OnLoaded()
            {
                base.OnLoaded();

                UpdateAssetPicker();
                UpdateLayout();
            }

            /// <inheritdoc />
            public override void OnValuesChanged()
            {
                base.OnValuesChanged();

                UpdateAssetPicker();
                UpdateLayout();
            }
        }

        public const int MainNodeGroupId = 1;
        public const int MainNodeTypeId = 1;
        public const int EffectNodeGroupId = 2;
        public const int ParameterNodeGroupId = 6;

        // Surface will be expanded later
        public RenderingGraphSurface(IVisjectSurfaceOwner owner, Action onSave, FlaxEditor.Undo undo = null, SurfaceStyle style = null)
            : base(owner, onSave, undo, style, RenderingGraphGroups)
        {
        }

        public static readonly NodeArchetype[] RenderingGraphSourceNodes =
        {
            // Main node
            new NodeArchetype
            {
                TypeID = 1,
                Title = "RenderingGraph",
                Description = "Main expression graph node",
                Flags = NodeFlags.AllGraphs | NodeFlags.NoRemove | NodeFlags.NoSpawnViaGUI | NodeFlags.NoCloseButton,
                Size = new Vector2(150, 300),
                Elements = new[]
                {
                    NodeElementArchetype.Factory.Input(0, "Output", true, ConnectionType.Object, 0) // Last optional param: Value Index
                }
            },
            new NodeArchetype
            {
                TypeID = 2,
                Title = "Main Camera",
                Description = "Main camera output",
                Flags = NodeFlags.AllGraphs,
                Size = new Vector2(200, 90),
                Elements = new[]
                {
                    // TODO: Motion vectors at full res or half res dropdown
                    NodeElementArchetype.Factory.Input(0, "Size", true, ConnectionType.Vector2, 0),
                    NodeElementArchetype.Factory.Input(1, "Camera", true, ConnectionType.Object, 1),
                    NodeElementArchetype.Factory.Output(0, "Scene Color", ConnectionType.Object, 2),
                    NodeElementArchetype.Factory.Output(1, "Depth Buffer", ConnectionType.Object, 3),
                    NodeElementArchetype.Factory.Output(2, "Motion Vectors", ConnectionType.Object, 4),
                }
            },
            // TODO: Camera node
            new NodeArchetype
            {
                TypeID = 4,
                Title = "Texture",
                Description = "Texture",
                Flags = NodeFlags.AllGraphs,
                Size = new Vector2(150, 90),
                DefaultValues = new object[]
                {
                    Guid.Empty
                },
                Elements = new[]
                {
                    NodeElementArchetype.Factory.Input(0, "Size", true, ConnectionType.Vector2, 0),
                    NodeElementArchetype.Factory.Output(0, "Color", ConnectionType.Object, 2),
                    NodeElementArchetype.Factory.Asset(0, 20, 0, ContentDomain.Texture)
                }
            }
        };

        public static readonly NodeArchetype[] RenderingGraphEffectNodes =
        {
            // TODO: PixelsRenderer node
            new NodeArchetype
            {
                TypeID = 1,
                Create = (id, context, arch, groupArch) => new MaterialNodeParamsSet(id, context, arch, groupArch),
                Title = "PostFx",
                Description = "Post Processing Effect",
                Flags = NodeFlags.AllGraphs,
                Size = new Vector2(200, 90),
                DefaultValues = new object[]
                {
                    Guid.Empty
                },
                Elements = new[]
                {
                    NodeElementArchetype.Factory.Input(0, "Size", true, ConnectionType.Vector2, 0),
                    NodeElementArchetype.Factory.Input(1, "Input", true, ConnectionType.Object, 1),
                    NodeElementArchetype.Factory.Output(0, "", ConnectionType.Object, 2),
                    // TODO: Only allow PostFx materials
                    NodeElementArchetype.Factory.Asset(100, FlaxEditor.Surface.Constants.LayoutOffsetY, 0, ContentDomain.Material)
                }
            },
            new NodeArchetype
            {
                TypeID = 2,
                Create = (id, context, arch, groupArch) => new MaterialNodeParamsSet(id, context, arch, groupArch),
                Title = "Pixels Effect",
                Description = "Effect with control over pixel positions",
                Flags = NodeFlags.AllGraphs,
                Size = new Vector2(200, 90),
                DefaultValues = new object[]
                {
                    Guid.Empty
                },
                Elements = new[]
                {
                    NodeElementArchetype.Factory.Input(0, "Size", true, ConnectionType.Vector2, 0),
                    // TODO: Actual Input Texture from where the size is taken
                    NodeElementArchetype.Factory.Output(0, "", ConnectionType.Object, 1),
                    // TODO: Only allow Surface materials
                    NodeElementArchetype.Factory.Asset(100, FlaxEditor.Surface.Constants.LayoutOffsetY, 0, ContentDomain.Material)
                }
            }
        };

        // Our group archetypes
        public static readonly List<GroupArchetype> RenderingGraphGroups = new List<GroupArchetype>()
        {
            // Our own nodes, including the main node
            new GroupArchetype
            {
                GroupID = MainNodeGroupId,
                Name = "Rendering Graph Source",
                Color = new Color(231, 231, 60),
                Archetypes = RenderingGraphSourceNodes
            },
            new GroupArchetype
            {
                GroupID = EffectNodeGroupId,
                Name = "Rendering Graph Effects",
                Color = new Color(231, 231, 60),
                Archetypes = RenderingGraphEffectNodes
            },
            // Just a single parameter node
            new GroupArchetype
            {
                GroupID = ParameterNodeGroupId,
                Name = "Parameters",
                Color = new Color(52, 73, 94),
                Archetypes = new []{ FlaxEditor.Surface.Archetypes.Parameters.Nodes[0] }
            }
        };

        /// <summary>
        /// For saving and loading surfaces
        /// </summary>
        private class FakeSurfaceContext : ISurfaceContext
        {
            public string SurfaceName => throw new NotImplementedException();

            public byte[] SurfaceData { get; set; }

            public void OnContextCreated(VisjectSurfaceContext context)
            {

            }
        }

        /// <summary>
        /// Tries to load surface graph from the asset.
        /// </summary>
        /// <param name="createDefaultIfMissing">True if create default surface if missing, otherwise won't load anything.</param>
        /// <returns>Loaded surface bytes or null if cannot load it or it's missing.</returns>
        public static byte[] LoadSurface(JsonAsset asset, RenderingGraph assetInstance, bool createDefaultIfMissing)
        {
            if (!asset) throw new ArgumentNullException(nameof(asset));
            if (assetInstance == null) throw new ArgumentNullException(nameof(assetInstance));

            // Return its data
            if (assetInstance.VisjectSurface?.Length > 0)
            {
                return assetInstance.VisjectSurface;
            }

            // Create it if it's missing
            if (createDefaultIfMissing)
            {
                // A bit of a hack
                // Create a Visject Graph with a main node and serialize it!
                var surfaceContext = new VisjectSurfaceContext(null, null, new FakeSurfaceContext());

                // Add the main node
                // TODO: Change NodeFactory.DefaultGroups to your list of group archetypes
                var node = NodeFactory.CreateNode(RenderingGraphGroups, 1, surfaceContext, MainNodeGroupId, MainNodeTypeId);

                if (node == null)
                {
                    Debug.LogWarning("Failed to create main node.");
                    return null;
                }
                surfaceContext.Nodes.Add(node);
                node.Location = Vector2.Zero;
                surfaceContext.Save();
                return surfaceContext.Context.SurfaceData;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Updates the surface graph asset (save new one, discard cached data, reload asset).
        /// </summary>
        /// <param name="data">Surface data.</param>
        /// <returns>True if cannot save it, otherwise false.</returns>
        public static bool SaveSurface(JsonAsset asset, RenderingGraph assetInstance, byte[] surfaceData)
        {
            if (!asset) throw new ArgumentNullException(nameof(asset));

            assetInstance.VisjectSurface = surfaceData;
            return FlaxEditor.Editor.SaveJsonAsset(asset.Path, assetInstance);
        }

        public void CompileSurface(RenderingGraph graph)
        {
            // We're mapping every output box to an index
            // So we can store the node outputs in an array
            var variableIndexGetter = new GraphVariables();

            // Get the parameters
            GetParameterGetterNodeArchetype(out ushort paramNodeGroupId);

            var graphParams = new Dictionary<Guid, GraphParameter<RenderingGraphContext>>();
            var parameters = new GraphParameter<RenderingGraphContext>[Parameters.Count];
            for (int i = 0; i < Parameters.Count; i++)
            {
                var param = Parameters[i];
                var graphParameter = new GraphParameter<RenderingGraphContext>(param.Name, param.Value, variableIndexGetter.RegisterNewVariable());
                graphParams.Add(param.ID, graphParameter);
                parameters[i] = graphParameter;
            }
            // Set the parameters
            graph.Parameters = parameters;
            graph.Nodes = FindNode(MainNodeGroupId, MainNodeTypeId)
                .DepthFirstTraversal()
                .Select<SurfaceNode, GraphNode<RenderingGraphContext>>((surfaceNode, index) =>
                {
                    int[] inputIndices = surfaceNode.Elements
                        .OfType<InputBox>()
                        .Select(inputBox => inputBox.HasAnyConnection ? variableIndexGetter.UseInputBox(inputBox) : -1)
                        .ToArray();
                    int[] outputIndices = surfaceNode.Elements
                        .OfType<OutputBox>()
                        .Select(outputBox => variableIndexGetter.RegisterOutputBox(outputBox))
                        .ToArray();

                    int groupId = surfaceNode.GroupArchetype.GroupID;
                    int typeId = surfaceNode.Archetype.TypeID;

                    var nodeDefinition = new GraphNodeDefinition()
                    {
                        GroupId = groupId,
                        TypeId = typeId,
                        Values = surfaceNode.Values,
                        InputIndices = inputIndices,
                        OutputIndices = outputIndices
                    };

                    // Create the runtime nodes
                    if (groupId == MainNodeGroupId)
                    {
                        if (typeId == MainNodeTypeId)
                        {
                            return new MainNode(nodeDefinition) { NodeIndex = index };
                        }

                        if (typeId == 2)
                        {
                            return new CameraNode(nodeDefinition) { NodeIndex = index };
                        }

                        if (typeId == 4)
                        {
                            return new TextureNode(nodeDefinition) { NodeIndex = index };
                        }
                    }
                    else if (groupId == EffectNodeGroupId)
                    {
                        if (typeId == 1)
                        {
                            return new PostEffectNode(nodeDefinition) { NodeIndex = index };
                        }

                        if (typeId == 2)
                        {
                            return new PixelsEffectNode(nodeDefinition) { NodeIndex = index };
                        }
                    }
                    else if (groupId == ParameterNodeGroupId)
                    {
                        var graphParam = graphParams[(Guid)surfaceNode.Values[0]];
                        nodeDefinition.InputIndices = new int[1] { graphParam.OutputIndex };
                        return new ParameterNode(nodeDefinition) { NodeIndex = index };
                    }

                    throw new NotSupportedException("Not supported node type");
                })
                .ToArray();
        }
    }
}
