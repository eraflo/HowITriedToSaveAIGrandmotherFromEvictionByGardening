using Utils;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
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
