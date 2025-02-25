using UnityEngine;

namespace Plant
{
    [CreateAssetMenu(fileName = "GlobalPlantOptions", menuName = "Plant/GlobalPlantOptions")]
    public class GlobalPlantOptions : ScriptableObject
    {
        public GameObject dirtPile;

        public GameObject dirtPileDigged;

        public GameObject dirtPileFilled;
    }
}