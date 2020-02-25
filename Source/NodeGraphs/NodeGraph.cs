using System;
using System.Linq;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.Utilities;

namespace NodeGraphs
{
    public abstract class NodeGraph<TContext> where TContext : GraphContext
    {
        private bool _enabled;
        private GraphNode<TContext>[] _nodes;

        [NoSerialize]
        protected TContext Context;

        /// <summary>
        /// Serialized Visject surface
        /// </summary>
        [Serialize]
        public byte[] VisjectSurface { get; set; }

        /// <summary>
        /// Parameters
        /// </summary>
        [Serialize]
        public GraphParameter<TContext>[] Parameters { get; set; }

        public GraphNode<TContext>[] Nodes
        {
            get => _nodes;
            set
            {
                if (Enabled) throw new Exception("Cannot set nodes while being enabled");
                _nodes = value;
            }
        }

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
                        Scripting.InvokeOnUpdate(async () =>
                        {
                            Task.Delay(1000).ContinueWith(a => { Scripting.InvokeOnUpdate(Enable); });
                        });
                    else
                        OnDisable();
                }
            }
        }

        protected abstract TContext CreateContext(object[] variables);

        private void Enable()
        {
            if (!_enabled) return;
            if (_nodes == null) return;
            OnEnable();
        }

        protected virtual void OnEnable()
        {
            int maxVariableIndex = Math.Max(
                Parameters.Select(p => p.OutputIndex).DefaultIfEmpty(0).Max(),
                _nodes.Max(node => node.Definition.OutputIndices.DefaultIfEmpty(0).Max())
            );
            Context = CreateContext(new object[maxVariableIndex + 1]);

            _nodes.ForEach(n => n.Context = Context);
            _nodes.ForEach(n => n.OnEnable());
        }

        protected virtual void OnDisable()
        {
            _nodes?.ForEach(n => n.OnDisable());
            _nodes?.ForEach(n => n.Context = null);
            Context = null;
        }

        public virtual void Update(float deltaTime)
        {
            if (_nodes == null || _nodes.Length <= 0) return;

            // Update the parameters
            // Each parameter will write its Value to the context
            for (int i = 0; i < Parameters.Length; i++) Parameters[i].Update(Context);
            // Update the nodes
            // Each node will get its inputs from the context
            //    Then, each node will execute its associated action
            //    Lastly, it will write the outputs to the context
            for (int i = 0; i < _nodes.Length; i++) _nodes[i].OnUpdate();
        }

        public void OnDestroy()
        {
            Enabled = false;
        }
    }
}