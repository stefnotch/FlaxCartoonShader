using System;
using FlaxEditor.Surface;
using FlaxEngine;
using NodeGraphs;

namespace NodeGraphs.Editor
{
    public class NodeFactory<TNode> where TNode : GraphNode
    {
        public delegate TNode CreateNodeDelegate(NodeDefinition definition);

        public NodeArchetype Archetype;
        public CreateNodeDelegate Create;
    }
}
