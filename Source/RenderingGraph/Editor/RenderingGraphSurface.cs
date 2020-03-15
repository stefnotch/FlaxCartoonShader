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
using NodeGraphs.Editor;
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

        public RenderingGraphSurface(IVisjectSurfaceOwner owner, Action onSave, FlaxEditor.Undo undo = null, SurfaceStyle style = null)
            : base(owner, onSave, undo, style, GetGroupArchetypes())
        {
        }

        public static readonly NodeFactoryGroup<RenderingNode> SourceNodeFactories =
            new NodeFactoryGroup<RenderingNode>()
            {
                GroupArchetype = new GroupArchetype
                {
                    GroupID = MainNodeGroupId,
                    Name = "Rendering Graph Source",
                    Color = new Color(231, 231, 60)
                }
            };

        public static readonly NodeFactoryGroup<RenderingNode> EffectNodeFactories =
            new NodeFactoryGroup<RenderingNode>()
            {
                GroupArchetype = new GroupArchetype
                {
                    GroupID = 2,
                    Name = "Rendering Graph Effects",
                    Color = new Color(231, 231, 60)
                }
            };

        public static readonly List<NodeFactoryGroup<RenderingNode>> NodeFactoryGroups = new List<NodeFactoryGroup<RenderingNode>>()
        {
            // Our own nodes, including the main node
            SourceNodeFactories,
            EffectNodeFactories,
            // Just a single parameter node
            new NodeFactoryGroup<RenderingNode>()
            {
                GroupArchetype = new GroupArchetype
                {
                    GroupID = 6,
                    Name = "Parameters",
                    Color = new Color(52, 73, 94),
                }
            }
        };

        public static List<GroupArchetype> GetGroupArchetypes()
        {
            return new List<GroupArchetype>(NodeFactoryGroups.Select(t => t.GetGroupArchetype()));
        }

        public static NodeFactoryGroup<RenderingNode> GetNodeFactoryGroup(int groupId)
        {
            for (int i = 0; i < NodeFactoryGroups.Count; i++)
            {
                if (NodeFactoryGroups[i].GroupArchetype.GroupID == groupId)
                {
                    return NodeFactoryGroups[i];
                }
            }

            return null;
        }

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
                var node = NodeFactory.CreateNode(GetGroupArchetypes(), 1, surfaceContext, MainNodeGroupId, MainNodeTypeId);

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

            var graphParams = new Dictionary<Guid, GraphParameter>();
            var parameters = new GraphParameter[Parameters.Count];
            for (int i = 0; i < Parameters.Count; i++)
            {
                var param = Parameters[i];
                var graphParameter = new GraphParameter(param.Name, param.Value, variableIndexGetter.RegisterVariable());
                graphParams.Add(param.ID, graphParameter);
                parameters[i] = graphParameter;
            }

            var nodes = FindNode(MainNodeGroupId, MainNodeTypeId)
                .DepthFirstTraversal()
                .Select<SurfaceNode, RenderingNode>((surfaceNode, index) =>
                {
                    int[] inputIndices = surfaceNode.Elements
                        .OfType<InputBox>()
                        .Select(inputBox => inputBox.HasAnyConnection ? variableIndexGetter.UseVariable(inputBox) : -1)
                        .ToArray();
                    int[] outputIndices = surfaceNode.Elements
                        .OfType<OutputBox>()
                        .Select(outputBox => variableIndexGetter.RegisterVariable(outputBox))
                        .ToArray();

                    int groupId = surfaceNode.GroupArchetype.GroupID;
                    int typeId = surfaceNode.Archetype.TypeID;

                    if (groupId == paramNodeGroupId)
                    {
                        var graphParam = graphParams[(Guid)surfaceNode.Values[0]];
                        inputIndices = new int[1] { graphParam.OutputIndex };
                    }

                    var nodeDefinition = new NodeDefinition()
                    {
                        GroupId = groupId,
                        TypeId = typeId,
                        Index = index,
                        Values = surfaceNode.Values,
                        InputIndices = inputIndices,
                        OutputIndices = outputIndices
                    };

                    var nodeFactory = GetNodeFactoryGroup(groupId).GetNodeFactory(typeId);
                    return nodeFactory.Create(nodeDefinition);
                })
                .ToArray();

            graph.Parameters = parameters;
            graph.Nodes = nodes;
        }
    }
}
