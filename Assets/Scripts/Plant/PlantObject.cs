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

        public GameObject weed;
        
        public GrowType growType;
        
        public int sellValue;

        [Header("Use this one if Grow Type is set to Time")]
        public int growthTimeSeconds;

        [Header("Use this one if Grow Type is set to Day")]
        public int growthTimeDays;
    }
}
