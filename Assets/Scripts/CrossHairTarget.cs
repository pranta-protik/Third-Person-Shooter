using UnityEngine;

public class CrossHairTarget : MonoBehaviour
{
    [SerializeField] private Transform _player;
    
    private Camera _mainCamera;
    private Ray _ray;
    private RaycastHit _hitInfo;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        var distanceOffset = Vector3.Distance(_mainCamera.transform.position, _player.transform.position);
        
        _ray.origin = _mainCamera.transform.position + _mainCamera.transform.forward * distanceOffset;
        _ray.direction = _mainCamera.transform.forward;

        transform.position = Physics.Raycast(_ray, out _hitInfo) ? _hitInfo.point : _ray.GetPoint(1000f);
    }
}
