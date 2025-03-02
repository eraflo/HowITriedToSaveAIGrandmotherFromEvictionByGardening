using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.NPC.Behaviour.Leaves
{
    public class ActionNode : Node
    {
        public delegate NodeState Action();
        private Action action;

        public ActionNode(Action action)
        {
            this.action = action;
        }

        public NodeState Evaluate()
        {
            return action();
        }
    }
}
