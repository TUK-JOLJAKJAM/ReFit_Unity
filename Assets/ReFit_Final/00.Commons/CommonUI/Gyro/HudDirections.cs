using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HudDirections : MonoBehaviour
{
    public GyroHud gyroHud;
    public GyroHud.GyroDirection gyroDirection;
    public Image image;

    float inputTime = 0.0f;

    Coroutine inputCoroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        image.enabled = true;
        inputCoroutine = StartCoroutine(InputCoroutine());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        image.enabled = false;
        inputTime = 0.0f;
        
        StopCoroutine(inputCoroutine);
        inputCoroutine = null;
    }

    IEnumerator InputCoroutine()
    {
        while (true)
        {
            if (image.enabled)
            {
                inputTime += Time.deltaTime;
                if (inputTime >= gyroHud.inputTriggerTime)
                {
                    ReFitLogger.Info($"[GyroHud] {gyroDirection} ют╥б");
                    inputTime = 0.0f;
                    gyroHud.GyroInputEnter(gyroDirection);
                }
            }
            else
            {
                inputTime = 0.0f;
            }
            yield return null;
        }
    }
}
