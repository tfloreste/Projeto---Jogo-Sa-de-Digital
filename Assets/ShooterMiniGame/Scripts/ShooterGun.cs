using UnityEngine;

public class ShooterGun : MonoBehaviour
{
    [SerializeField] private GameObject firePoint;
    [SerializeField] private GameObject bulletPrefab;

    public void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.transform.position, Quaternion.identity);
    }
}
