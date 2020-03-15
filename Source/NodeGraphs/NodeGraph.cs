using System;
using System.Linq;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.Utilities;

namespace NodeGraphs
{
    public abstract class NodeGraph<TNode> where TNode : GraphNode
    {
        private bool _enabled;
        private TNode[] _nodes;
        protected object[] Variables;

        /// <summary>
        /// Serialized Visject surface
        /// </summary>
        [Serialize]
        public byte[] VisjectSurface { get; set; }

        /// <summary>
        /// Parameters
        /// </summary>
        [Serialize]
        public GraphParameter[] Parameters { get; set; }

        [Serialize]
        public TNode[] Nodes
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
                        Scripting.InvokeOnUpdate(() =>
                        {
                            Task.Delay(1000).ContinueWith(a =>
                             {
                                 Scripting.InvokeOnUpdate(() =>
                                 {
                                     if (!_enabled) return;
                                     if (_nodes == null) return;
                                     OnEnable();
                                 });
                             });
                        });
                    else
                        OnDisable();
                }
            }
        }

        protected virtual void OnEnable()
        {
            int maxVariableIndex = Math.Max(
                Parameters.Select(p => p.OutputIndex).DefaultIfEmpty(0).Max(),
                _nodes.Max(node => node.OutputIndices.DefaultIfEmpty(0).Max())
            );
            Variables = new object[maxVariableIndex + 1];

            _nodes.ForEach(n => n.Variables = Variables);
            _nodes.ForEach(n => n.OnEnable());
        }

        protected virtual void OnDisable()
        {
            _nodes?.ForEach(n => n.OnDisable());
            _nodes?.ForEach(n => n.Variables = null);
            Variables = null;
        }

        public virtual void Update(float deltaTime)
        {
            if (_nodes == null || _nodes.Length <= 0) return;

            // Update the parameters
            // Each parameter will write its Value to the context
            for (int i = 0; i < Parameters.Length; i++) Parameters[i].Update(Variables);
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