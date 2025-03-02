using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.NPC.Behaviour
{
    public abstract class CompositeNode : Node
    {
        protected List<Node> children = new List<Node>();

        public CompositeNode()
        {
        }

        public CompositeNode(List<Node> children)
        {
            this.children = children;
        }

        public abstract NodeState Evaluate();

        public void AddChild(Node child)
        {
            children.Add(child);
        }

        public void RemoveChild(Node child)
        {
            children.Remove(child);
        }

        public void ClearChildren()
        {
            children.Clear();
        }

        public List<Node> GetChildren()
        {
            return children;
        }
    }
}
