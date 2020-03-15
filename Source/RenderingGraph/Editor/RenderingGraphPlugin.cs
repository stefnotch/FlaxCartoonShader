using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEditor;
using FlaxEditor.Surface;
using FlaxEngine;
using NodeGraphs.Editor;
using RenderingGraph.Nodes;

namespace RenderingGraph.Editor
{
    public class RenderingGraphPlugin : EditorPlugin
    {
        private RenderingGraphProxy _renderingGraphProxy;
        private List<NodeFactory<RenderingNode>> _nodeFactories;

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            _nodeFactories = new List<NodeFactory<RenderingNode>>();
            AddRenderingGraphNodes();

            _renderingGraphProxy = new RenderingGraphProxy();
            Editor.ContentDatabase.Proxy.Insert(22, _renderingGraphProxy);
        }

        private void AddRenderingGraphNodes()
        {
            var sourceNodeFactories = RenderingGraphSurface.SourceNodeFactories;

            sourceNodeFactories.Add(new NodeFactory<RenderingNode>()
            {
                Archetype = new NodeArchetype
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
                Create = (definition) => new MainNode()
                {
                    Index = definition.Index,
                    InputIndices = definition.InputIndices,
                    OutputIndices = definition.OutputIndices
                }
            });

            sourceNodeFactories.Add(new NodeFactory<RenderingNode>()
            {
                Archetype = new NodeArchetype
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
                Create = (definition) => new CameraNode()
                {
                    Index = definition.Index,
                    InputIndices = definition.InputIndices,
                    OutputIndices = definition.OutputIndices
                }
            });

            // TODO: Camera node

            sourceNodeFactories.Add(new NodeFactory<RenderingNode>()
            {
                Archetype = new NodeArchetype
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
                },
                Create = (definition) => new TextureNode()
                {
                    Index = definition.Index,
                    InputIndices = definition.InputIndices,
                    OutputIndices = definition.OutputIndices,
                    // TODO: is this correct?
                    Texture = FlaxEngine.Content.Load<Texture>((Guid)definition.Values[0])
                }
            });


            // And now, the effect nodes
            var effectNodeFactories = RenderingGraphSurface.EffectNodeFactories;
            effectNodeFactories.Add(new NodeFactory<RenderingNode>()
            {
                Archetype = new NodeArchetype
                {
                    TypeID = 1,
                    Create = (id, context, arch, groupArch) => new RenderingGraphSurface.MaterialNodeParamsSet(id, context, arch, groupArch),
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
                Create = (definition) => new PostEffectNode()
                {
                    Index = definition.Index,
                    InputIndices = definition.InputIndices,
                    OutputIndices = definition.OutputIndices,
                    // TODO: is this correct?
                    Material = FlaxEngine.Content.Load<MaterialBase>((Guid)definition.Values[0])
                }
            });

            effectNodeFactories.Add(new NodeFactory<RenderingNode>()
            {
                Archetype = new NodeArchetype
                {
                    TypeID = 2,
                    Create = (id, context, arch, groupArch) => new RenderingGraphSurface.MaterialNodeParamsSet(id, context, arch, groupArch),
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
                },
                Create = (definition) => new PixelsEffectNode()
                {
                    Index = definition.Index,
                    InputIndices = definition.InputIndices,
                    OutputIndices = definition.OutputIndices,
                    // TODO: is this correct?
                    Material = FlaxEngine.Content.Load<MaterialBase>((Guid)definition.Values[0])
                }
            });

            var parameterNodeFactories = RenderingGraphSurface.GetNodeFactoryGroup(6);
            parameterNodeFactories.Add(new NodeFactory<RenderingNode>()
            {
                Archetype = FlaxEditor.Surface.Archetypes.Parameters.Nodes[0],
                Create = (definition) => new ParameterNode()
                {
                    Index = definition.Index,
                    InputIndices = definition.InputIndices,
                    OutputIndices = definition.OutputIndices,
                }
            });
        }

        /// <inheritdoc />
        public override void Deinitialize()
        {
            // TODO: Only clear my node factories
            RenderingGraphSurface.NodeFactoryGroups.ForEach(g => g.NodeFactories.Clear());

            Editor.ContentDatabase.Proxy.Remove(_renderingGraphProxy);
            base.Deinitialize();
        }
    }
}
