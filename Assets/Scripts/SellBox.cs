using Plant;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SellBox : MonoBehaviour
{
    public SellBoxColliderHelper colliderBox;
    private GameManager gm;
    public TextMeshProUGUI sellValueText;
    public TextMeshProUGUI yourMoneyText;
    public Canvas canvas;
    public Transform camTransform;
    // Start is called before the first frame update
    void Start()
    {
        canvas.enabled = false;
        gm = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckCameraDistanceFromBox() <= 3f)
        {
            ToggleCanvasVisible(true);
        }
        else
        {
            ToggleCanvasVisible(false);
        }
    }

    private int CalculateSellValue()
    {
        int currentSellValue = 0;
        var currentPlants = colliderBox.sellablePlants;
        foreach (var plant in currentPlants)
        {
            int addValue = plant.GetComponent<FullyGrown>().plant.sellValue;
            currentSellValue += addValue;
        }
        return currentSellValue;
    }

    public void UpdateText()
    {
        sellValueText.text = "Sell for (+" + CalculateSellValue().ToString() + "€)";
    }

    public void SellPlants()
    {
        var currentPlantsToSell = colliderBox.sellablePlants;
        int moneyGained = CalculateSellValue();
        foreach(GameObject plant in currentPlantsToSell)
        {  
            Destroy(plant);
        }
        colliderBox.ResetPlants();
        gm.GiveMoney(moneyGained);
        yourMoneyText.text = "Bank account: " + gm.Money.ToString() + "€";
        UpdateText();
    }

    private float CheckCameraDistanceFromBox()
    {
        float distance = Vector3.Distance(this.transform.position, camTransform.position);
        return distance;
    }

    private void ToggleCanvasVisible(bool flag)
    {
        if (flag)
        {
            canvas.enabled = true;
        }
        else
        {
            canvas.enabled = false;
        }
    }
}
