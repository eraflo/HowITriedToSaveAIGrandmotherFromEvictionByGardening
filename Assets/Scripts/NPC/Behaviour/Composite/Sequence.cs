using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.NPC.Behaviour.Composite
{
    public class Sequence : CompositeNode
    {
        public override NodeState Evaluate()
        {
            foreach (var child in children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.FAILURE:
                        return NodeState.FAILURE;
                    case NodeState.RUNNING:
                        return NodeState.RUNNING;
                    case NodeState.SUCCESS:
                        continue;
                }
            }
            return NodeState.SUCCESS;
        }
    }
}
