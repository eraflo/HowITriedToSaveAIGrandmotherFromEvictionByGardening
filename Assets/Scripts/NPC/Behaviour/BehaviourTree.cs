using Assets.Scripts.NPC.Behaviour.Composite;
using Assets.Scripts.NPC.Behaviour.Leaves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.NPC.Behaviour
{
    public class BehaviourTree : MonoBehaviour
    {
        private Node root;
        private bool isInitialized = false;

        public void Initialize(Node root)
        {
            if (isInitialized)
            {
                throw new InvalidOperationException("Behaviour tree already initialized");
            }

            this.root = root;
            isInitialized = true;
        }

        public void ResetTree()
        {
            if (root != null)
            {
                root = null;
                isInitialized = false;
            }
        }

        public Node GetTree()
        {
            return root;
        }

        void Update()
        {
            if (root != null)
            {
                root.Evaluate();
            }
        }
    }
}
