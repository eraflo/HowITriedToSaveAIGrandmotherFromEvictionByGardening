using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.NPC.Behaviour.Leaves
{
    public class ConditionNode : Node
    {
        public delegate bool Condition();
        private Condition condition;

        public ConditionNode(Condition condition)
        {
            this.condition = condition;
        }

        public NodeState Evaluate()
        {
            return condition() ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}
