using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellBoxColliderHelper : MonoBehaviour
{   

    public List<GameObject> sellablePlants = new List<GameObject>();
    public SellBox sellBox;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

 
    private void OnTriggerEnter(Collider other)
    {
        var plantObj = other.gameObject;
        if (plantObj.CompareTag("Produce"))
        {
            sellablePlants.Add(plantObj);
            sellBox.UpdateText();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var plantObj = other.gameObject;
        if (plantObj.CompareTag("Produce"))
        {
            sellablePlants.Remove(plantObj);
            sellBox.UpdateText();
        }   
    }
    public void ResetPlants()
    {
        sellablePlants.Clear();
    }
}
