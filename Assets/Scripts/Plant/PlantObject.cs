using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Plant
{
    [CreateAssetMenu(fileName = "PlantObject", menuName = "Plant/PlantObject")]
    public class PlantObject : ScriptableObject
    {
        public string plantName;
        
        public GameObject[] sproutStages;
        
        public XRGrabInteractable grownPlant;
        
        public int growthTime;
    }
}
