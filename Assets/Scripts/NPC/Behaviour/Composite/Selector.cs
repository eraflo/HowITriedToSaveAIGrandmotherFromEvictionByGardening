using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.NPC.Behaviour.Composite
{
    public class Selector : CompositeNode
    {
        public override NodeState Evaluate()
        {
            foreach (var child in children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.RUNNING:
                        return NodeState.RUNNING;
                    case NodeState.SUCCESS:
                        return NodeState.SUCCESS;
                }
            }
            return NodeState.FAILURE;
        }
    }
}
