using UnityEngine;

public class CastleGuard_AmmoManager : MonoBehaviour
{
    public int ammoCount = 11;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ammoCount = 11;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UseAmmo()
    {
        if (ammoCount > 0) ammoCount--;
    }
    public void ReLoad()
    {
        ammoCount = 11;
    }
}
