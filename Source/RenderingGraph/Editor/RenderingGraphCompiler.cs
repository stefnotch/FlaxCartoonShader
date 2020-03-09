using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeGraphs;
using RenderingGraph.Nodes;

namespace RenderingGraph.Editor
{
    public class RenderingGraphCompiler
    {
        public struct NodeCreator
        {
            public int GroupId;
            public int TypeId;
            public Func<GraphNodeDefinition, GraphNode<RenderingGraphContext>> Create;
        }

        public static NodeCreator[] NodeCreators = new[]
        {
            new NodeCreator
            {
                GroupId = RenderingGraphSurface.MainNodeGroupId,
                TypeId = 1,
                Create = (definition) => new MainNode(definition)
            },
            new NodeCreator
            {
                GroupId = 1,
                TypeId = 2,
                Create = (definition) => new CameraNode(definition)
            },
            new NodeCreator
            {
                GroupId = 1,
                TypeId = 4,
                Create = (definition) => new TextureNode(definition)
            },
            new NodeCreator
            {
                GroupId = RenderingGraphSurface.EffectNodeGroupId,
                TypeId = 1,
                Create = (definition) => new PostEffectNode(definition)
            },
            new NodeCreator
            {
                GroupId = RenderingGraphSurface.EffectNodeGroupId,
                TypeId = 2,
                Create = (definition) => new PixelsEffectNode(definition)
            },
            new NodeCreator
            {
                GroupId = RenderingGraphSurface.ParameterNodeGroupId,
                TypeId = 1,
                Create = (definition) => new ParameterNode(definition)
            },
        };

        public void Compile(RenderingGraph graph, GraphParameter<RenderingGraphContext>[] parameters, GraphNodeDefinition[] nodeDefinitions)
        {
            var nodes = new GraphNode<RenderingGraphContext>[nodeDefinitions.Length];

            for (int i = 0; i < nodes.Length; i++)
            {
                var nodeCreator = GetNodeCreator(nodeDefinitions[i]);
                nodes[i] = nodeCreator.Create(nodeDefinitions[i]);
            }

            graph.Parameters = parameters;
            graph.Nodes = nodes;
        }

        private static NodeCreator GetNodeCreator(GraphNodeDefinition nodeDefinition)
        {
            for (int j = 0; j < NodeCreators.Length; j++)
            {
                if (NodeCreators[j].GroupId == nodeDefinition.GroupId &&
                    NodeCreators[j].TypeId == nodeDefinition.TypeId)
                {
                    return NodeCreators[j];
                }
            }

            throw new NotSupportedException("Not supported node type");
        }
    }
}
