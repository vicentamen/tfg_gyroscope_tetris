using UnityEngine;
using UnityEngine.Events;

public class ScreenShake
{
    public static UnityEvent<Vector3> screenShakeEvent = new UnityEvent<Vector3>();
    public static UnityEvent<Vector3> screenBounceEvent = new UnityEvent<Vector3>();

    public static void DoShake(Vector3 direction)
    {
        screenShakeEvent.Invoke(direction);
    }

    public static void DoBounce(Vector3 direction)
    {
        screenBounceEvent.Invoke(direction);
    }
}
