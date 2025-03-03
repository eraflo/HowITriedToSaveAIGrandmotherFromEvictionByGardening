using TMPro;
using UnityEngine;

public class SeedBox : MonoBehaviour
{

    public string seedType;

    public Canvas canvas;

    public TextMeshProUGUI buyTitle;

    public TextMeshProUGUI buyText;

    private GameManager gm;

    public int seedCost;

    public Transform camTransform;

    public TextMeshProUGUI yourMoneyText;

    public GameObject seedPrefab;

   
    
    // Start is called before the first frame update
    void Start()
    {   
        canvas.enabled = false;
        
        gm = GameManager.Instance;
      
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
        
    }

    public void BuyThis()
    {   
        if (gm.Money >= seedCost)
        {
            gm.TakeMoney(seedCost);
            Instantiate(seedPrefab, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);
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
