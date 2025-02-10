using UnityEngine;

public class TimeOfDayManager : MonoBehaviour
{
    [SerializeField] private float timeScale = 1;
    
    public int Day { get; private set; } = 1;
    
    public int Hour { get; private set; }
    
    public int Minute { get; private set; }
    
    private float _time;
    
    private void Update()
    {
        _time += Time.deltaTime * timeScale;
        if (!(_time >= 1))
        {
            return;
        }
        
        Minute++;
        _time = 0;
        
        if (Minute >= 60)
        {
            Hour++;
            Minute = 0;
        }
        
        if (Hour >= 24)
        {
            Day++;
            Hour = 0;
        }
    }
}
