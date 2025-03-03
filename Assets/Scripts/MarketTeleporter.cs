using UnityEngine;

public class MarketTeleporter : MonoBehaviour
{
    public void Travel()
    {
        GameManager.Instance.SwitchArea();
    }
}
