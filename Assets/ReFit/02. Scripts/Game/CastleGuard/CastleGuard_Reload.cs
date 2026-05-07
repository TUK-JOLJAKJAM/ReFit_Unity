using UnityEngine;

public class CastleGuard_Reload : MonoBehaviour
{
    double reloadTime = 3.0f;
    double currentTime = 0.0f;
    bool isReloading = false;
    public ReFit_G03_CastleGuard mainGameUI;

    private void Update()
    {
        if(isReloading)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= reloadTime)
            {
                Debug.Log("Reloaded!");
                mainGameUI.ReLoad();
                isReloading = false;
                currentTime = 0.0f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentTime = 0.0f;
        isReloading = true;
        Debug.Log("Reloading...");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isReloading = false;
        currentTime = 0.0f;
    }
}
