using UnityEngine;

namespace Plant
{
    [CreateAssetMenu(fileName = "PlantObject", menuName = "Plant/PlantObject")]
    public class PlantObject : ScriptableObject
    {
        public string plantName;
        
        public GameObject[] sproutStages;
        
        public GameObject grownPlant;
        
        public int growthTime;
    }
}
