using UnityEngine;
using Utils;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform canvasSpawnPoint;
    [SerializeField] private Transform houseSpawnPoint;
    [SerializeField] private Transform marketSpawnPoint;
    
    public int Money { get; private set; }

    private void Update()
    {
        if (TimeOfDayManager.Instance.Hour < 19)
        {
            return;
        }

        if (TimeOfDayManager.Instance.Paused)
        {
            return;
        }

        TimeOfDayManager.Instance.NextDay();
    }

    public void SetPlayerPosition(PlayerPosition pos)
    {
        player.position = pos switch
        {
            PlayerPosition.Canvas => canvasSpawnPoint.position,
            PlayerPosition.House => houseSpawnPoint.position,
            PlayerPosition.Market => marketSpawnPoint.position,
            _ => player.position
        };
    }

    public void SwitchArea()
    {
        var distanceToHouse = Vector3.Distance(player.position, houseSpawnPoint.position);
        var distanceToMarket = Vector3.Distance(player.position, marketSpawnPoint.position);

        SetPlayerPosition(distanceToHouse < distanceToMarket ? PlayerPosition.Market : PlayerPosition.House);
    }
    
    public void GiveMoney(int amount)
    {
        Money += amount;
    }
    
    public void TakeMoney(int amount, bool landlord = false)
    {
        Money -= amount;
        if (Money > 0 && landlord)
        {
            return;
        }
        
        // TODO: game over logic
    }
}

public enum PlayerPosition
{
    Canvas,
    House,
    Market
}
