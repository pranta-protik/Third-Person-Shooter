using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] private Transform _crossHairTarget;
    [SerializeField] private Transform _weaponParent;
    [SerializeField] private Transform _weaponLeftGrip;
    [SerializeField] private Transform _weaponRightGrip;
    [SerializeField] private Animator _rigController;
    
    private RaycastWeapon _raycastWeapon;

    private void Start()
    {
        var existingWeapon = GetComponentInChildren<RaycastWeapon>();
        if (existingWeapon)
        {
            Equip(existingWeapon);
        }
    }

    private void Update()
    {
        if (_raycastWeapon)
        {
            if (Input.GetButtonDown("Fire1")) _raycastWeapon.StartFiring();
            if (_raycastWeapon.IsFiring) _raycastWeapon.UpdateFiring(Time.deltaTime);
            _raycastWeapon.UpdateBullet(Time.deltaTime);
            if (Input.GetButtonUp("Fire1")) _raycastWeapon.StopFiring();

            if (Input.GetKeyDown(KeyCode.X))
            {
                var isHolstered = _rigController.GetBool("Holster_Weapon");
                _rigController.SetBool("Holster_Weapon", !isHolstered);
            }
        }
    }

    public void Equip(RaycastWeapon newWeapon)
    {
        if (_raycastWeapon)
        {
            Destroy(_raycastWeapon.gameObject);
        }
        
        _raycastWeapon = newWeapon;
        _raycastWeapon.RayCastDestination = _crossHairTarget;
        _raycastWeapon.transform.parent = _weaponParent;
        _raycastWeapon.transform.localPosition = Vector3.zero;
        _raycastWeapon.transform.localRotation = Quaternion.identity;
        
        _rigController.Play($"Equip_{_raycastWeapon.WeaponName}");
    }
}
