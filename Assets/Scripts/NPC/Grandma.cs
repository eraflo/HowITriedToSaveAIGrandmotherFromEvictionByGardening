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
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Scripts.NPC
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Grandma : NPC
    {
        [Header("Grandma Knowledge")]
        [SerializeField] private Transform _toolCabinet;
        [SerializeField] private Transform _player;
        [SerializeField] private Transform _playerRightHand;

        [Header("XR")]
        [SerializeField] private Transform _grabPoint;

        private bool fetchTool = false;
        private bool gotTool = false;
        private string toolToFetch;
        private bool lookAtTarget = false;

        private IXRSelectInteractable _grabbedTool;
        private XRInteractionManager _interactionManager;

        protected sealed override void DeclareFunctions()
        {
            _geminiClient.DeclareNewFunction(GetFunctionDeclaration("FetchTool"));
        }

        private void Start()
        {

            _interactionManager = FindObjectOfType<XRInteractionManager>();

            Selector root = new Selector();

            Sequence idleSequence = new Sequence();
            ActionNode idleAction = new ActionNode(Idle);


            Sequence fetchToolSequence = new Sequence();
            ConditionNode fetchToolCondition = new ConditionNode(() => fetchTool);
            ActionNode fetchToolAction = new ActionNode(FetchToolAction);
            ActionNode grabToolAction = new ActionNode(GrabTool);

            Sequence giveToolToPlayer = new Sequence();
            ConditionNode toolGrabbedCondition = new ConditionNode(() => gotTool);
            ActionNode goToPlayerAction = new ActionNode(GoToPlayer);
            ActionNode giveToolToPlayerAction = new ActionNode(GiveToolToPlayer);

            idleSequence.AddChild(idleAction);

            fetchToolSequence.AddChild(fetchToolCondition);
            fetchToolSequence.AddChild(fetchToolAction);
            fetchToolSequence.AddChild(grabToolAction);

            giveToolToPlayer.AddChild(toolGrabbedCondition);
            giveToolToPlayer.AddChild(goToPlayerAction);
            giveToolToPlayer.AddChild(giveToolToPlayerAction);

            root.AddChild(fetchToolSequence);
            root.AddChild(giveToolToPlayer);
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
            toolToFetch = tool;
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

                if (_toolArea != null)
                {
                    lookAtTarget = false;
                    return NodeState.SUCCESS;
                }

                if (!lookAtTarget)
                {
                    transform.LookAt(_toolCabinet.position);
                    lookAtTarget = true;
                }

                return NodeState.RUNNING;
            }

            return NodeState.FAILURE;
        }

        private NodeState GrabTool()
        {
            _grabbedTool = _toolArea.GrabTool(toolToFetch);
            if (_grabbedTool != null)
            {

                var _grabbedToolObject = _grabbedTool as MonoBehaviour;
                if (_grabbedToolObject != null)
                {
                    _grabbedToolObject.transform.SetParent(_grabPoint);
                    _grabbedToolObject.transform.localPosition = Vector3.zero;
                    _grabbedToolObject.transform.localRotation = Quaternion.identity;
                    _grabbedToolObject.GetComponent<Rigidbody>().useGravity = false;
                }

                gotTool = true;
                fetchTool = false;

                return NodeState.SUCCESS;
            }

            return NodeState.RUNNING;
        }

        private NodeState GoToPlayer()
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.SetDestination(_player.position);


                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    lookAtTarget = false;
                    return NodeState.SUCCESS;
                }


                if (!lookAtTarget)
                {
                    transform.LookAt(_player.position);
                    lookAtTarget = true;
                }

                return NodeState.RUNNING;
            }

            Debug.Log("No agent");

            return NodeState.FAILURE;
        }

        private NodeState GiveToolToPlayer()
        {
            IXRSelectInteractor ray = _playerRightHand.GetComponent<XRRayInteractor>();
            if (ray != null)
            {
                var _grabbedToolObject = _grabbedTool as MonoBehaviour;
                if (_grabbedToolObject != null)
                {
                    _grabbedToolObject.transform.SetParent(null);
                    _grabbedToolObject.GetComponent<Rigidbody>().useGravity = true;
                }

                _interactionManager.SelectEnter(ray, _grabbedTool);

                gotTool = false;

                return NodeState.SUCCESS;
            }

            return NodeState.FAILURE;
        }
    }
}
