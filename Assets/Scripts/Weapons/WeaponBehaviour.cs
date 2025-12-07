using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

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

    private float _nextAllowedFireTime;

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
