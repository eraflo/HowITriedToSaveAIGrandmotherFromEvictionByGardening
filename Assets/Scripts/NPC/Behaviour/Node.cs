using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.NPC.Behaviour
{
    public interface Node
    {
        public NodeState Evaluate();
    }

    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }
}
