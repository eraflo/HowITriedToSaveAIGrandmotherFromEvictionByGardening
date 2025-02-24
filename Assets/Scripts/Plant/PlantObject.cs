using UnityEngine;

[CreateAssetMenu(fileName = "PlantObject", menuName = "Plant/PlantObject", order = 1)]
public class PlantObject : ScriptableObject
{
    public GameObject[] sproutStages;
    
    public GameObject grownPlant;
    
    public int growthTime;
}
