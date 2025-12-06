using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Target behaviour that awards points when hit by projectiles.
/// Can be configured to only accept specific weapon types.
/// </summary>
public class TargetBehaviour : MonoBehaviour
{
    [Header("Target Configuration")]
    [SerializeField] private WeaponType requiredWeaponType = WeaponType.Any;

    [Header("Feedback")]
    [SerializeField] private Color hitColor = Color.green;
    [SerializeField] private float hitColorDuration = 0.2f;

    [Header("Events")]
    [SerializeField] private UnityEvent onHit;

    private Renderer _renderer;
    private Color _originalColor;
    private bool _isActive = true;
    private float _colorResetTime;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            _originalColor = _renderer.material.color;
        }
    }

    private void Update()
    {
        // Reset hit color after duration
        if (_renderer != null && Time.time >= _colorResetTime && _renderer.material.color != _originalColor)
        {
            _renderer.material.color = _originalColor;
        }
    }

    public void CheckHit(ProjectileData projectileData)
    {
        Debug.Log("CheckHit: " + projectileData.name);
        if (!_isActive)
        {
            return;
        }
        Debug.Log("CheckHit: " + projectileData.name + " has projectile data");
        var projectileWeaponType = projectileData.GetWeaponType();
        if (requiredWeaponType != WeaponType.Any && projectileWeaponType != requiredWeaponType)
        {
            Debug.Log("CheckHit: " + projectileData.name + " has wrong weapon type");
            return;
        }
        Debug.Log("CheckHit: " + projectileData.name + " has valid weapon type");

        OnValidHit();
    }

    private void OnValidHit()
    {
        // Add score
        ScoreManager.Instance?.AddPoints();

        // Play feedback
        PlayHitFeedback();

        // Invoke events
        onHit?.Invoke();

        Destroy(gameObject);
    }

    private void PlayHitFeedback()
    {
        // Change color temporarily
        if (_renderer != null)
        {
            _renderer.material.color = hitColor;
            _colorResetTime = Time.time + hitColorDuration;
        }
    }
}

