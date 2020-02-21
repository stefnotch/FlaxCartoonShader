using System;
using NodeGraphs;

namespace RenderingGraph
{
    public class RenderingNode : GraphNode<RenderingGraphContext>
    {
        public RenderingNode(NodeDefinition definition) : base(definition)
        {
        }

        // TODO: Properly serialize guids instead of relying on this hack!
        public static Guid ParseGuid(object guid)
        {
            if (guid is Guid guidStruct) return guidStruct;
            if (guid == null) return Guid.Empty;
            if (guid is string guidString)
            {
                return guidString == "" ? Guid.Empty : Guid.Parse(guidString);
            }

            return Guid.Empty;
        }
    }
}