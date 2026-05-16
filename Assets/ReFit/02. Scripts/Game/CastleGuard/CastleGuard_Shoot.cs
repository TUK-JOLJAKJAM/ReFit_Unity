using System.Collections;
using UnityEngine;

public class CastleGuard_Shoot : MonoBehaviour
{
    public CastleGuard_GameManager gameManager;
    public GameObject bulletPrefab;
    public Transform bulletParent;
    public CastleGuard_AmmoManager ammoManager;
    public ReFit_G03_CastleGuard mainGameUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ShootCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Shoot()
    {
        if (gameManager._currentState == CastleGuard_GameManager.GameState.Playing)
        {
            if (ammoManager.ammoCount <= 0) return;
            Instantiate(bulletPrefab, transform.position + new Vector3(-0.17f, 0, -0.1f), Quaternion.identity, bulletParent);
            mainGameUI.UseAmmo();
            ammoManager.UseAmmo();
        }
    }

    IEnumerator ShootCoroutine()
    {
        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
