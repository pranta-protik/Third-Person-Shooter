using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterAiming : MonoBehaviour
{
    [SerializeField] private float _turnSpeed = 15f;
    [SerializeField] private float _aimDuration = 0.3f;
    [SerializeField] private Rig _aimLayer;
    
    private Camera _mainCamera;
    private RaycastWeapon _raycastWeapon;

    private void Start()
    {
        _mainCamera = Camera.main;
        _raycastWeapon = GetComponentInChildren<RaycastWeapon>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        var yawCamera = _mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, yawCamera, 0f), _turnSpeed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (_aimLayer)
        {
            if (Input.GetButton("Fire2"))
            {
                _aimLayer.weight += Time.deltaTime / _aimDuration;
            }
            else
            {
                _aimLayer.weight -= Time.deltaTime / _aimDuration;
            }
        }

        if (Input.GetButtonDown("Fire1")) _raycastWeapon.StartFiring();
        if (_raycastWeapon.IsFiring) _raycastWeapon.UpdateFiring(Time.deltaTime);
        _raycastWeapon.UpdateBullet(Time.deltaTime);
        if (Input.GetButtonUp("Fire1")) _raycastWeapon.StopFiring();
    }
}
