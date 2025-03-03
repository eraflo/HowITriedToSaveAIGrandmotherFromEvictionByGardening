using Assets.Scripts.NPC;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Collider))]
    public class ToolArea : MonoBehaviour
    {
        [SerializeField]
        private List<XRGrabInteractable> tools = new List<XRGrabInteractable>();

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<NPC.NPC>())
            {
                NPC.NPC npc = other.gameObject.GetComponent<NPC.NPC>();
                npc.EnterToolArea(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<NPC.NPC>())
            {
                NPC.NPC npc = other.gameObject.GetComponent<NPC.NPC>();
                npc.ExitToolArea();
            }
        }

        public XRGrabInteractable GrabTool()
        {
            if (tools.Count == 0)
            {
                return null;
            }

            int index = Random.Range(0, tools.Count);
            XRGrabInteractable tool = tools[index];
            tools.RemoveAt(index);
            return tool;
        }

        public XRGrabInteractable GrabTool(int index)
        {
            if (index < 0 || index >= tools.Count)
            {
                return null;
            }

            XRGrabInteractable tool = tools[index];
            tools.RemoveAt(index);
            return tool;
        }

        public XRGrabInteractable GrabTool(string toolName)
        {
            XRGrabInteractable tool = tools.Find(t => t.name.ToLower() == toolName.ToLower());
            if (tool != null)
            {
                tools.Remove(tool);
            }
            return tool;
        }

        public void ReturnTool(XRGrabInteractable tool)
        {
            tools.Add(tool);
        }
    }
}
