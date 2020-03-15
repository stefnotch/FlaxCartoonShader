using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FlaxEditor.Surface;

namespace NodeGraphs.Editor
{
    public class NodeFactoryGroup<TNode> where TNode : GraphNode
    {
        public GroupArchetype GroupArchetype;
        public readonly List<NodeFactory<TNode>> NodeFactories = new List<NodeFactory<TNode>>();

        public NodeFactory<TNode> Add(NodeFactory<TNode> nodeFactory)
        {
            NodeFactories.Add(nodeFactory);
            return nodeFactory;
        }

        public void Remove(NodeFactory<TNode> nodeFactory)
        {
            NodeFactories.Remove(nodeFactory);
        }

        public NodeFactory<TNode> GetNodeFactory(int typeId)
        {
            for (int i = 0; i < NodeFactories.Count; i++)
            {
                if (NodeFactories[i].Archetype.TypeID == typeId)
                {
                    return NodeFactories[i];
                }
            }

            return null;
        }

        public GroupArchetype GetGroupArchetype()
        {
            var nodeArchetypes = new NodeArchetype[NodeFactories.Count];
            for (int i = 0; i < NodeFactories.Count; i++)
            {
                nodeArchetypes[i] = NodeFactories[i].Archetype;
            }

            GroupArchetype.Archetypes = nodeArchetypes;
            return GroupArchetype;
        }
    }
}
