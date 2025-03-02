using UnityEngine;

public class SkipDay : MonoBehaviour
{
    public void SkipTheDay()
    {
        TimeOfDayManager.Instance.SkipDay();
    }
}
