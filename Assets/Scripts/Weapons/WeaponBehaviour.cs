using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

/// <summary>
/// Weapon type enum for categorizing different weapon types.
/// </summary>
public enum WeaponType
{
    Pistol,
    Rifle,
    Shotgun,
    Any
}

/// <summary>
/// Basic VR weapon behaviour that instantiates a projectile, plays feedback
/// and sends haptics to the bound controller when fired.
/// </summary>
public class WeaponBehaviour : MonoBehaviour
{
    [Header("Weapon Type")]
    [SerializeField] private WeaponType weaponType = WeaponType.Pistol;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform muzzleTransform;
    [SerializeField] private float projectileSpeed = 25f;
    [SerializeField] private float fireCooldown = 0.2f;

    public WeaponType Type => weaponType;

    [Header("Ammo")]
    [SerializeField] private int magazineSize = 12;
    [SerializeField] private int ammoInReserve = -1;
    [SerializeField] private float reloadDuration = 1.2f;
    [SerializeField] private bool autoReloadOnEmpty = true;

    [Header("Feedback")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private HapticImpulsePlayer hapticPlayer;
    [SerializeField, Range(0f, 1f)] private float hapticAmplitude = 0.35f;
    [SerializeField] private float hapticDuration = 0.1f;

    private float _nextAllowedFireTime;
    private int _currentAmmo;
    private bool _isReloading;
    private Coroutine _reloadRoutine;

    public int CurrentAmmo => _currentAmmo;
    public int ReserveAmmo => ammoInReserve;
    public int MagazineSize => magazineSize;
    public bool IsReloading => _isReloading;

    private void Awake()
    {
        _currentAmmo = Mathf.Clamp(magazineSize, 0, magazineSize);
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
        ConsumeAmmo();

        _nextAllowedFireTime = Time.time + fireCooldown;
    }

    private bool CanFire()
    {
        if (_isReloading)
        {
            return false;
        }

        if (Time.time < _nextAllowedFireTime)
        {
            return false;
        }

        if (projectilePrefab == null || muzzleTransform == null)
        {
            Debug.LogWarning($"{name}: WeaponBehaviour requires a projectile prefab and muzzle transform.");
            return false;
        }

        if (_currentAmmo <= 0)
        {
            if (autoReloadOnEmpty)
            {
                Reload();
            }

            return false;
        }

        return true;
    }

    private void SpawnProjectile()
    {
        var projectile = Instantiate(projectilePrefab, muzzleTransform.position, muzzleTransform.rotation);

        // Attach projectile data component to carry weapon type information
        var projectileData = projectile.GetComponent<ProjectileData>();
        projectileData.SetWeaponType(weaponType);
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

    public void Reload()
    {
        if (_isReloading || _currentAmmo >= magazineSize)
        {
            return;
        }

        if (ammoInReserve == 0)
        {
            return;
        }

        if (_reloadRoutine != null)
        {
            StopCoroutine(_reloadRoutine);
        }

        _reloadRoutine = StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        _isReloading = true;

        if (reloadDuration > 0f)
        {
            yield return new WaitForSeconds(reloadDuration);
        }
        else
        {
            yield return null;
        }

        var missingAmmo = magazineSize - _currentAmmo;
        int ammoToLoad;

        if (ammoInReserve < 0)
        {
            ammoToLoad = missingAmmo;
        }
        else
        {
            ammoToLoad = Mathf.Min(missingAmmo, ammoInReserve);
            ammoInReserve -= ammoToLoad;
        }

        _currentAmmo += ammoToLoad;
        _currentAmmo = Mathf.Clamp(_currentAmmo, 0, magazineSize);

        _isReloading = false;
        _reloadRoutine = null;
        _nextAllowedFireTime = Time.time + fireCooldown;
    }

    private void ConsumeAmmo()
    {
        if (_currentAmmo <= 0)
        {
            return;
        }

        _currentAmmo--;

        if (_currentAmmo <= 0 && autoReloadOnEmpty)
        {
            Reload();
        }
    }
}
