using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEditor;
using FlaxEditor.Surface;
using FlaxEditor.Surface.Elements;
using FlaxEngine;
using NodeGraphs;
using RenderingGraph.Nodes;

namespace RenderingGraph.Editor
{
    public class RenderingGraphSurface : VisjectSurface
    {
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
            // TODO: PostFx node
            // TODO: PixelsRenderer node
            // TODO: 
            new NodeArchetype
            {
                TypeID = 1,
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

                    // TODO: Material parameter inputs
                    //NodeElementArchetype.Factory.Input(2, "", true, ConnectionType.All, 1),
                   // NodeElementArchetype.Factory.TextBox(20, 40, 80, 20, -1, false),

                    NodeElementArchetype.Factory.Output(0, "", ConnectionType.Object, 2),

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

            bool success = FlaxEditor.Editor.SaveJsonAsset(asset.Path, assetInstance);
            asset.Reload();
            return success;
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

            graph.SetNodes(FindNode(MainNodeGroupId, MainNodeTypeId)
                .DepthFirstTraversal()
                .Select<SurfaceNode, GraphNode<RenderingGraphContext>>(surfaceNode =>
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

                    var nodeDefinition = new RenderingNode.NodeDefinition()
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
                            return new MainNode(nodeDefinition);
                        }
                        if (typeId == 2)
                        {
                            return new CameraNode(nodeDefinition);
                        }

                        if (typeId == 4)
                        {
                            return new TextureNode(nodeDefinition);
                        }
                    } else if (groupId == EffectNodeGroupId)
                    {
                        if (typeId == 1)
                        {
                            return new PostEffectNode(nodeDefinition);
                        }
                    }
                    else if (groupId == ParameterNodeGroupId)
                    {
                        nodeDefinition.InputIndices = new int[1] { graphParams[(Guid) surfaceNode.Values[0]].OutputIndex };
                        return new ParameterNode(nodeDefinition);
                    }

                    throw new NotSupportedException("Not supported node type");
                })
                .ToArray());
        }
    }
}
