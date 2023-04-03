using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    class Bullet
    {
        public float time;
        public Vector3 initialPosition;
        public Vector3 initialVelocity;
        public TrailRenderer tracer;
    }
    
    [SerializeField] private int _fireRate = 25;
    [SerializeField] private float _bulletSpeed = 1000f;
    [SerializeField] private float _bulletDrop;
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private ParticleSystem _hitEffect;
    [SerializeField] private TrailRenderer _tracerEffect;
    [SerializeField] private string _weaponName;
    [SerializeField] private Transform _raycastOrigin;
    [SerializeField] private Transform _rayCastDestination;
    
    private Ray _ray;
    private RaycastHit _hitInfo;
    private float _accumulatedTime;
    private List<Bullet> _bullets = new();
    private float _maxLifeTime = 3f;
    private bool _isFiring;

    public bool IsFiring => _isFiring;
    public Transform RayCastDestination
    {
        get => _rayCastDestination;
        set => _rayCastDestination = value;
    }

    public string WeaponName => _weaponName;

    private Vector3 GetPosition(Bullet bullet)
    {
        // p + v*t + 0.5*g*t*t

        var gravity = Vector3.down * _bulletDrop;
        return bullet.initialPosition + bullet.initialVelocity * bullet.time + 0.5f * gravity * bullet.time * bullet.time;
    }

    private Bullet CreateBullet(Vector3 position, Vector3 velocity)
    {
        var bullet = new Bullet
        {
            initialPosition = position,
            initialVelocity = velocity,
            time = 0f,
            tracer = Instantiate(_tracerEffect, position, Quaternion.identity)
        };
        
        bullet.tracer.AddPosition(position);
        
        return bullet;
    }
    
    public void StartFiring()
    {
        _isFiring = true;
        _accumulatedTime = 0f;
        FireBullet();
    }

    public void UpdateFiring(float deltaTime)
    {
        _accumulatedTime += deltaTime;
        var fireInterval = 1f / _fireRate;
        while (_accumulatedTime >= 0f)
        {
            FireBullet();
            _accumulatedTime -= fireInterval;
        }
    }

    public void UpdateBullet(float deltaTime)
    {
        SimulateBullets(deltaTime);
        DestroyBullets();
    }

    private void SimulateBullets(float deltaTime)
    {
        _bullets.ForEach(bullet =>
        {
            var p0 = GetPosition(bullet);
            bullet.time += deltaTime;
            var p1 = GetPosition(bullet);
            RaycastSegment(p0, p1, bullet);
        });
    }

    private void DestroyBullets()
    {
        _bullets.RemoveAll(bullet => bullet.time >= _maxLifeTime);
    }
    
    private void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        var direction = end - start;
        var distance = direction.magnitude;
        _ray.origin = start;
        _ray.direction = direction;
        
        if (Physics.Raycast(_ray, out _hitInfo, distance))
        {
            // Debug.DrawLine(_raycastOrigin.position, _hitInfo.point, Color.red, 1f);
            _hitEffect.transform.position = _hitInfo.point;
            _hitEffect.transform.forward = _hitInfo.normal;
            _hitEffect.Emit(1);
        
            bullet.tracer.transform.position = _hitInfo.point;
            bullet.time = _maxLifeTime;
        }
        else
        {
            bullet.tracer.transform.position = end;
        }
    }
    
    private void FireBullet()
    {
        _muzzleFlash.Emit(1);

        var velocity = (_rayCastDestination.position - _raycastOrigin.position).normalized * _bulletSpeed;
        var bullet = CreateBullet(_raycastOrigin.position, velocity);
        _bullets.Add(bullet);
        
        // _ray.origin = _raycastOrigin.position;
        // _ray.direction = _rayCastDestination.position - _raycastOrigin.position;
        //
        // var tracer = Instantiate(_tracerEffect, _ray.origin, Quaternion.identity);
        // tracer.AddPosition(_ray.origin);
        //
        // if (Physics.Raycast(_ray, out _hitInfo))
        // {
        //     // Debug.DrawLine(_raycastOrigin.position, _hitInfo.point, Color.red, 1f);
        //     _hitEffect.transform.position = _hitInfo.point;
        //     _hitEffect.transform.forward = _hitInfo.normal;
        //     _hitEffect.Emit(1);
        //
        //     tracer.transform.position = _hitInfo.point;
        // }
    }

    public void StopFiring() => _isFiring = false;
}
