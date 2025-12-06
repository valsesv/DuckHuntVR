using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Component attached to projectiles to carry weapon type information.
/// Allows targets to identify which weapon type fired the projectile.
/// Handles collision detection and spawns hit effects.
/// </summary>
public class ProjectileData : MonoBehaviour
{
    [SerializeField] private float lifetime = -1f;
    [SerializeField] private Rigidbody _rigidbody;
    [Header("Hit Effect")]
    [SerializeField] private ParticleSystem hitEffectParticle;

    private WeaponType _weaponType;
    private bool _hasHit = false;
    private GameObject _sourceWeapon;

    private void Awake()
    {
        Assert.IsNotNull(_rigidbody, "Rigidbody is required");
        Assert.IsNotNull(hitEffectParticle, "Hit effect particle is required");
    }

    /// <summary>
    /// Sets the weapon type for this projectile.
    /// </summary>
    public void SetWeaponType(WeaponType weaponType)
    {
        _weaponType = weaponType;
    }

    /// <summary>
    /// Gets the weapon type that fired this projectile.
    /// </summary>
    public WeaponType GetWeaponType()
    {
        return _weaponType;
    }

    /// <summary>
    /// Sets the weapon that fired this projectile. Used to ignore collisions with the weapon.
    /// </summary>
    public void SetSourceWeapon(GameObject weapon)
    {
        _sourceWeapon = weapon;

        // Ignore collisions between this projectile and the weapon
        if (weapon != null)
        {
            var projectileColliders = GetComponentsInChildren<Collider>();
            var weaponColliders = weapon.GetComponentsInChildren<Collider>();

            foreach (var projCollider in projectileColliders)
            {
                foreach (var weaponCollider in weaponColliders)
                {
                    Physics.IgnoreCollision(projCollider, weaponCollider, true);
                }
            }
        }
    }

    private void Start()
    {
        if (lifetime > 0f)
        {
            Destroy(gameObject, lifetime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ignore collisions with the weapon that fired this projectile
        if (_sourceWeapon != null && (collision.gameObject == _sourceWeapon || collision.transform.IsChildOf(_sourceWeapon.transform)))
        {
            return;
        }

        Debug.Log($"[ProjectileData] OnCollisionEnter: Hit {collision.gameObject.name} at position {collision.contacts[0].point}");
        if (_hasHit)
        {
            Debug.Log("[ProjectileData] OnCollisionEnter: Already hit, ignoring");
            return;
        }

        if (collision.gameObject.TryGetComponent<TargetBehaviour>(out var targetBehaviour))
        {
            targetBehaviour.CheckHit(this);
        }

        ContactPoint contact = collision.contacts[0];
        HandleHit(contact.point, Quaternion.LookRotation(contact.normal));
    }

    void OnDestroy()
    {
        Debug.Log("Projectile destroyed");
    }

    private void HandleHit(Vector3 hitPosition, Quaternion hitRotation)
    {
        if (_hasHit)
        {
            return;
        }
        _hasHit = true;

        var effect = Instantiate(hitEffectParticle, hitPosition, hitRotation);
        Destroy(gameObject);
    }
}

