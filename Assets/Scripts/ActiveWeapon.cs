using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    public enum WeaponSlot
    {
        Primary = 0,
        Secondary = 1
    }
    
    [SerializeField] private Transform _crossHairTarget;
    [SerializeField] private Transform[] _weaponSlots;
    [SerializeField] private Animator _rigController;

    private RaycastWeapon[] _equipped_weapons = new RaycastWeapon[2];
    private int _activeWeaponIndex;

    private void Start()
    {
        var existingWeapon = GetComponentInChildren<RaycastWeapon>();
        if (existingWeapon)
        {
            Equip(existingWeapon);
        }
    }

    private RaycastWeapon GetWeapon(int index)
    {
        if (index < 0 || index >= _equipped_weapons.Length) return null;
        return _equipped_weapons[index];
    }

    private void Update()
    {
        var weapon = GetWeapon(_activeWeaponIndex);
        if (weapon)
        {
            if (Input.GetButtonDown("Fire1")) weapon.StartFiring();
            if (weapon.IsFiring) weapon.UpdateFiring(Time.deltaTime);
            weapon.UpdateBullet(Time.deltaTime);
            if (Input.GetButtonUp("Fire1")) weapon.StopFiring();

            if (Input.GetKeyDown(KeyCode.X))
            {
                var isHolstered = _rigController.GetBool("Holster_Weapon");
                _rigController.SetBool("Holster_Weapon", !isHolstered);
            }
        }
    }

    public void Equip(RaycastWeapon newWeapon)
    {
        var weaponSlotIndex = (int)newWeapon.WeaponSlot;
        var weapon = GetWeapon(weaponSlotIndex);
        
        if (weapon)
        {
            Destroy(weapon.gameObject);
        }
        
        weapon = newWeapon;
        weapon.RayCastDestination = _crossHairTarget;
        weapon.transform.parent = _weaponSlots[weaponSlotIndex];
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        
        _rigController.Play($"Equip_{weapon.WeaponName}");

        _equipped_weapons[weaponSlotIndex] = weapon;
        _activeWeaponIndex = weaponSlotIndex;
    }
}
