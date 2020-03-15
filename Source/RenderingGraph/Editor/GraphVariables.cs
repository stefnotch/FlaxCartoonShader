using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEditor.Surface.Elements;
using FlaxEngine;

namespace RenderingGraph.Editor
{
    /// <summary>
    /// Used to map output boxes to an index
    /// </summary>
    public class GraphVariables
    {
        private class Variable
        {
            /// <summary>
            /// Index of the variable/output box
            /// </summary>
            public int Index;

            /// <summary>
            /// How often it can be used
            /// If we're done using an output box, we can re-use its index
            /// </summary>
            public int UsagesLeft;
        }

        private readonly Dictionary<Int2, Variable> _connectionIds = new Dictionary<Int2, Variable>();
        private readonly HashSet<int> _takenConnectionIds = new HashSet<int>();

        /// <summary>
        /// Register a output box
        /// </summary>
        /// <param name="box">An output box</param>
        /// <returns>The id of the output box or -1</returns>
        public int RegisterVariable(OutputBox box)
        {
            if (!box.IsOutput)
            {
                throw new ArgumentException("Not an output box", nameof(box));
            }

            if (box.Connections.Count <= 0) return -1;

            // If this box already has been registered
            if (_connectionIds.TryGetValue(GetBoxId(box), out var variable))
            {
                return variable.Index;
            }

            // Otherwise, register it
            for (int i = 0; ; i++)
            {
                if (!_takenConnectionIds.Contains(i))
                {
                    _connectionIds.Add(GetBoxId(box), new Variable() { Index = i, UsagesLeft = box.Connections.Count });
                    _takenConnectionIds.Add(i);

                    return i;
                }
            }
        }

        /// <summary>
        /// Registers a new variable 
        /// </summary>
        /// <remarks>This variable cannot be unregistered anymore</remarks>
        /// <returns>Index of the variable</returns>
        public int RegisterVariable()
        {
            for (int i = 0; ; i++)
            {
                if (!_takenConnectionIds.Contains(i))
                {
                    //_connectionIds.Add(GetBoxId(box), new Variable() { Index = i, UsageCount = box.Connections.Count });
                    _takenConnectionIds.Add(i);

                    return i;
                }
            }
        }

        /// <summary>
        /// Use an input box
        /// </summary>
        /// <param name="box">An input box</param>
        /// <returns>The id of the associated output box or -1</returns>
        public int UseVariable(InputBox box)
        {
            if (!box.HasAnyConnection) return -1;

            var outputBox = box.Connections[0];

            if (_connectionIds.TryGetValue(GetBoxId(outputBox), out var variable))
            {
                variable.UsagesLeft--;
                if (variable.UsagesLeft <= 0)
                {
                    RemoveVariable(outputBox);
                }
                return variable.Index;
            }
            else
            {
                throw new Exception("Parent box hasn't been registered");
            }
        }

        private void RemoveVariable(Box box)
        {
            var id = GetBoxId(box);
            if (_connectionIds.TryGetValue(id, out var variable))
            {
                _connectionIds.Remove(id);
                _takenConnectionIds.Remove(variable.Index);
            }
        }

        private Int2 GetBoxId(Box box)
        {
            return new Int2((int)box.ParentNode.ID, box.ID);
        }
    }
}
