using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterAiming : MonoBehaviour
{
    [SerializeField] private float _turnSpeed = 15f;
    [SerializeField] private float _aimDuration = 0.3f;
    [SerializeField] private Rig _aimLayer;
    
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
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
            // if (Input.GetButton("Fire2"))
            // {
            //     _aimLayer.weight += Time.deltaTime / _aimDuration;
            // }
            // else
            // {
            //     _aimLayer.weight -= Time.deltaTime / _aimDuration;
            // }
            _aimLayer.weight = 1f;
        }
    }
}
