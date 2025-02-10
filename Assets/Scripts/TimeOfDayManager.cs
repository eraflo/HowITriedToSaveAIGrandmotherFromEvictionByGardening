using UnityEngine;

public class TimeOfDayManager : MonoBehaviour
{
    [SerializeField] private Light sun;
    [SerializeField] private float timeScale = 1;
    
    public int Day { get; private set; } = 1;

    public int Hour { get; private set; } = 7;
    
    public int Minute { get; private set; }
    
    private float _time;
    
    private void Update()
    {
        _time += Time.deltaTime * timeScale;
        if (_time >= 1)
        {
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

        UpdateSunRotation();
    }

    private void UpdateSunRotation()
    {
        var timeInDay = Hour + Minute / 60f;
        var sunAngle = timeInDay / 24f * 360f;
        sun.transform.rotation = Quaternion.Euler(sunAngle - 90f, 170f, 0f);
    }
}
