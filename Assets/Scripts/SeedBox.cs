using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Utils;

public class SeedBox : MonoBehaviour
{

    public string seedType;

    public Canvas canvas;

    public UnityEngine.UI.Button buyButton;

    public TextMeshProUGUI buyTitle;

    public TextMeshProUGUI buyText;

    private GameManager gm;

    public int seedCost;

    public Transform camTransform;

    public bool hasBeenBought;

    private Vector3 startingPos;

    private BoxCollider col;

    private Rigidbody rb;

    public TextMeshProUGUI yourMoneyText;

   
    
    // Start is called before the first frame update
    void Start()
    {   
        startingPos = transform.position;
        canvas.enabled = false;
        
        gm = GameManager.Instance;
        col = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();

        col.enabled = false;
        rb.isKinematic = true;
      
        if (seedType != null)
        {
            buyTitle.text = "Seeds for sale (" + seedType + ")";
        }
        else
        {
            buyTitle.text = "Seeds for sale";
        }
        buyText.text = "Buy for " + seedCost + "€";
        
    }

    // Update is called once per frame
    void Update()
    {

        yourMoneyText.text = "Bank account: " + gm.Money.ToString() + "€";

        if (CheckCameraDistanceFromBox() <= 3f)
        {
            ToggleCanvasVisible(true);
        }
        else
        {
            ToggleCanvasVisible(false);
        }

        if (hasBeenBought)
        {
            canvas.enabled = false;
            col.enabled = true;
            rb.isKinematic = false;
        }
        else
        {
           transform.position = startingPos;
        }
        
    }

    public void BuyThis()
    {   
        if (gm.Money >= seedCost)
        {
            gm.TakeMoney(seedCost);
            hasBeenBought = true;
        }
        
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
