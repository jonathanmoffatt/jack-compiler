using System.Collections.Generic;

namespace JackCompiler
{
    public class Node : NodeBase
    {
        private List<NodeBase> children = new List<NodeBase>();

        public IEnumerable<NodeBase> Children => children;

        public Node(NodeType type)
        {
            Type = type;
        }

        public T AddChild<T>(T node) where T : NodeBase
        {
            if (node != null)
                children.Add(node);
            return node;
        }

        public void AddChildren<T>(params T[] nodes) where T : NodeBase
        {
            foreach (var node in nodes)
                AddChild(node);
        }

    }
}
