using System.Collections;
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
        weapon.transform.SetParent(_weaponSlots[weaponSlotIndex], false);
        _equipped_weapons[weaponSlotIndex] = weapon;

        SetActiveWeapon(weaponSlotIndex);
    }

    private void SetActiveWeapon(int weaponSlotIndex)
    {
        var holsterIndex = _activeWeaponIndex;
        var activateIndex = weaponSlotIndex;

        StartCoroutine(SwitchWeapon(holsterIndex, activateIndex));
    }

    private IEnumerator SwitchWeapon(int holsterIndex, int activateIndex)
    {
        yield return StartCoroutine(HolsterWeapon(holsterIndex));
        yield return StartCoroutine(ActivateWeapon(activateIndex));
        _activeWeaponIndex = activateIndex;
    }

    private IEnumerator HolsterWeapon(int index)
    {
        var weapon = GetWeapon(index);
        if (weapon)
        {
            _rigController.SetBool("Holster_Weapon", true);
            do
            {
                yield return new WaitForEndOfFrame();
            } while (_rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);
        }
    }

    private IEnumerator ActivateWeapon(int index)
    {
        var weapon = GetWeapon(index);
        if (weapon)
        {
            _rigController.SetBool("Holster_Weapon", false);
            _rigController.Play($"Equip_{weapon.WeaponName}");
            do
            {
                yield return new WaitForEndOfFrame();
            } while (_rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);
        }
    }
    
}
