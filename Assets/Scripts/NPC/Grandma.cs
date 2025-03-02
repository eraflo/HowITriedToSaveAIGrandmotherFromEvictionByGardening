using Assets.Scripts.GeminiAI.Enums;
using Assets.Scripts.GeminiAI.FunctionsCall;
using Assets.Scripts.NPC.Behaviour;
using Assets.Scripts.NPC.Behaviour.Composite;
using Assets.Scripts.NPC.Behaviour.Leaves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.NPC
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Grandma : NPC
    {
        [Header("Grandma Knowledge")]
        [SerializeField] private Transform _toolCabinet;

        private bool fetchTool = false;

        protected sealed override void DeclareFunctions()
        {
            _geminiClient.DeclareNewFunction(GetFunctionDeclaration("FetchTool"));
        }

        private void Start()
        {
            Selector root = new Selector();

            Sequence idleSequence = new Sequence();
            ActionNode idleAction = new ActionNode(Idle);


            Sequence fetchToolSequence = new Sequence();
            ConditionNode fetchToolCondition = new ConditionNode(() => fetchTool);
            ActionNode fetchToolAction = new ActionNode(FetchToolAction);

            idleSequence.AddChild(idleAction);

            fetchToolSequence.AddChild(fetchToolCondition);
            fetchToolSequence.AddChild(fetchToolAction);

            root.AddChild(fetchToolSequence);
            root.AddChild(idleSequence);

            _behaviourTree.Initialize(root);
        }

        [Function("fetch_tool", "go fetch the tool asked by the player")]
        private void FetchTool(
            [Parameter(SchemaType.STRING, "the tool to fetch")] string tool
        )
        {
            Debug.Log($"Fetching tool : {tool}");
            fetchTool = true;
        }

        private NodeState Idle()
        {
            Debug.Log("Idle");
            return NodeState.SUCCESS;
        }

        private NodeState FetchToolAction()
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.SetDestination(_toolCabinet.position);


                if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                {
                    fetchTool = false;
                    return NodeState.SUCCESS;
                }
                else
                {
                    return NodeState.RUNNING;
                }
            }

            return NodeState.FAILURE;
        }
    }
}
