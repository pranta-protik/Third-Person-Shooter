using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private RaycastWeapon _weaponPrefab;

    private void OnTriggerEnter(Collider other)
    {
        var activeWeapon = other.GetComponent<ActiveWeapon>();

        if (activeWeapon)
        {
            var newWeapon = Instantiate(_weaponPrefab);
            activeWeapon.Equip(newWeapon);
        }
    }
}
