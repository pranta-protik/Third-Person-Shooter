using UnityEngine;
using UnityEngine.Animations.Rigging;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] private Transform _crossHairTarget;
    [SerializeField] private Rig _handIk;
    [SerializeField] private Transform _weaponParent;
    [SerializeField] private Transform _weaponLeftGrip;
    [SerializeField] private Transform _weaponRightGrip;
    
    private RaycastWeapon _raycastWeapon;
    private Animator _animator;
    private AnimatorOverrideController _animatorOverrideController;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animatorOverrideController = _animator.runtimeAnimatorController as AnimatorOverrideController;
        
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
        }
        else
        {
            _handIk.weight = 0f;
            _animator.SetLayerWeight(1, 0f);
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
        
        _handIk.weight = 1f;
        _animator.SetLayerWeight(1, 1f);

        Invoke(nameof(SetAnimationDelayed), 0.001f);
    }

    private void SetAnimationDelayed()
    {
        _animatorOverrideController["Weapon_Anim_Empty"] = _raycastWeapon.WeaponAnimation;
    }
    
#if UNITY_EDITOR
    [ContextMenu("Save Weapon Pose")]
    private void SaveWeaponPose()
    {
        var recorder = new GameObjectRecorder(gameObject);
        recorder.BindComponentsOfType<Transform>(_weaponParent.gameObject, false);
        recorder.BindComponentsOfType<Transform>(_weaponLeftGrip.gameObject, false);
        recorder.BindComponentsOfType<Transform>(_weaponRightGrip.gameObject, false);
        recorder.TakeSnapshot(0f);
        recorder.SaveToClip(_raycastWeapon.WeaponAnimation);
        AssetDatabase.SaveAssets();
    }
#endif
}
