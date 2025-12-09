using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using DG.Tweening;

/// <summary>
/// Basic VR weapon behaviour that instantiates a projectile, plays feedback
/// and sends haptics to the bound controller when fired.
/// </summary>
public class WeaponBehaviour : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform muzzleTransform;
    [SerializeField] private float projectileSpeed = 25f;
    [SerializeField] private float fireCooldown = 0.2f;

    [Header("Feedback")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private HapticImpulsePlayer hapticPlayer;
    [SerializeField, Range(0f, 1f)] private float hapticAmplitude = 0.35f;
    [SerializeField] private float hapticDuration = 0.1f;

    [Header("Return Animation")]
    [SerializeField] private float returnDuration = 1f;
    [SerializeField] private Ease returnEase = Ease.OutQuad;

    private float _nextAllowedFireTime;
    private XRGrabInteractable _grabInteractable;
    private Rigidbody _rigidbody;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Tween _returnTween;

    private void Awake()
    {
        _grabInteractable = GetComponent<XRGrabInteractable>();
        _rigidbody = GetComponent<Rigidbody>();

        // Store starting position and rotation
        _startPosition = transform.position;
        _startRotation = transform.rotation;

        // Subscribe to grab/detach events
        if (_grabInteractable != null)
        {
            _grabInteractable.selectEntered.AddListener((args) => OnWeaponGrabbed());
            _grabInteractable.selectExited.AddListener((args) => OnWeaponDetached());
        }
    }

    private void OnDestroy()
    {
        // Clean up event listeners
        if (_grabInteractable != null)
        {
            _grabInteractable.selectEntered.RemoveAllListeners();
            _grabInteractable.selectExited.RemoveAllListeners();
        }

        // Kill any active tweens
        _returnTween?.Kill();
    }

    private void OnWeaponGrabbed()
    {
        // If weapon is grabbed again during return animation, cancel the return
        if (_returnTween != null && _returnTween.IsActive())
        {
            _returnTween.Kill();

            // Re-enable physics immediately
            if (_rigidbody != null)
            {
                _rigidbody.isKinematic = false;
            }
        }
    }

    private void OnWeaponDetached()
    {
        // Kill any existing return tween
        _returnTween?.Kill();

        // Disable physics temporarily for smooth animation
        if (_rigidbody != null)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        // Animate back to start position and rotation using DOTween
        _returnTween = DOTween.Sequence()
            .Append(transform.DOMove(_startPosition, returnDuration).SetEase(returnEase))
            .Join(transform.DORotateQuaternion(_startRotation, returnDuration).SetEase(returnEase));
    }

    /// <summary>
    /// Attempts to fire the weapon. Will respect cooldown and configuration.
    /// </summary>
    public void Fire()
    {
        if (!CanFire())
        {
            return;
        }

        SpawnProjectile();
        PlayFeedback();

        _nextAllowedFireTime = Time.time + fireCooldown;
    }

    private bool CanFire()
    {
        if (Time.time < _nextAllowedFireTime)
        {
            return false;
        }

        if (projectilePrefab == null || muzzleTransform == null)
        {
            Debug.LogWarning($"{name}: WeaponBehaviour requires a projectile prefab and muzzle transform.");
            return false;
        }
        return true;
    }

    private void SpawnProjectile()
    {
        var projectile = Instantiate(projectilePrefab, muzzleTransform.position, muzzleTransform.rotation);

        // Attach projectile data component to carry weapon type information
        var projectileData = projectile.GetComponent<ProjectileData>();
        projectileData.SetSourceWeapon(gameObject);

        var rigidbody = projectile.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.velocity = muzzleTransform.forward * projectileSpeed;
        }
    }

    private void PlayFeedback()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        hapticPlayer?.SendHapticImpulse(hapticAmplitude, hapticDuration);
    }
}
