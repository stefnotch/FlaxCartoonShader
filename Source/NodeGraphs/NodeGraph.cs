using System;
using System.Linq;
using FlaxEngine;
using FlaxEngine.Utilities;

namespace NodeGraphs
{
    public class NodeGraph<TContext> where TContext : GraphContext, new()
    {
        private bool _enabled;

        [NoSerialize]
        public TContext Context;

        [Serialize]
        protected GraphNode<TContext>[] Nodes;

        /// <summary>
        /// Serialized Visject surface
        /// </summary>
        public byte[] VisjectSurface { get; set; }

        [Serialize]
        public GraphParameter<TContext>[] Parameters { get; set; }

        [NoSerialize]
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    if (_enabled)
                        OnEnable();
                    else
                        OnDisable();
                }
            }
        }

        public void SetNodes(GraphNode<TContext>[] nodes)
        {
            bool enabled = Enabled;
            Enabled = false;
            Nodes = nodes;
            Enabled = enabled;
        }


        protected virtual void OnEnable()
        {
            if (Nodes != null)
            {
                int maxVariableIndex = Math.Max(
                    Parameters.Select(p => p.OutputIndex).DefaultIfEmpty(0).Max(),
                    Nodes.Max(node => node.Definition.OutputIndices.DefaultIfEmpty(0).Max())
                );
                Context = new TContext
                {
                    Variables = new object[maxVariableIndex + 1]
                };
                Nodes.ForEach(n => n.Context = Context);
                Nodes.ForEach(n => n.OnEnable());
            }
            else
            {
                Context = new TContext
                {
                    Variables = new object[0]
                };
            }
        }

        protected virtual void OnDisable()
        {
            Nodes?.ForEach(n => n.OnDisable());
            Nodes?.ForEach(n => n.Context = null);
            Context = null;
        }

        public virtual void Update(float deltaTime)
        {
            if (Nodes == null || Nodes.Length <= 0) return;

            // Update the parameters
            // Each parameter will write its Value to the context
            for (int i = 0; i < Parameters.Length; i++) Parameters[i].Update(Context);
            // Update the nodes
            // Each node will get its inputs from the context
            //    Then, each node will execute its associated action
            //    Lastly, it will write the outputs to the context
            for (int i = 0; i < Nodes.Length; i++) Nodes[i].OnUpdate();
        }

        public void OnDestroy()
        {
            Enabled = false;
        }
    }
}