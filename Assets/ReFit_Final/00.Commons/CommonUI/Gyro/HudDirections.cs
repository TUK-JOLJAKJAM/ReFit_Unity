using UnityEngine;

public class HudDirections : MonoBehaviour
{
    public GyroHud gyroHud;
    public GyroHud.GyroDirection gyroDirection;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        gyroHud.GyroInputEnter(gyroDirection);
    }
}
