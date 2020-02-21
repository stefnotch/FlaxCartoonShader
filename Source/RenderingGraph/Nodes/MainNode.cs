using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingGraph.Nodes
{
    /// <summary>
    /// The main output node of the graph
    /// </summary>
    public class MainNode : RenderingNode
    {
        public MainNode(NodeDefinition definition) : base(definition)
        {
        }
    }
}
