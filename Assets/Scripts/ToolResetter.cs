using UnityEngine;

public class ToolResetter : MonoBehaviour
{
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    
    private void Awake()
    {
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
    }

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
        
        transform.position = _initialPosition;
        transform.rotation = _initialRotation;
    }
}
