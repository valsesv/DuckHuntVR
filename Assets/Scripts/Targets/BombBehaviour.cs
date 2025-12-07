using DG.Tweening;
using UnityEngine;

/// <summary>
/// Target behaviour that awards points when hit by projectiles.
/// Can be configured to only accept specific weapon types.
/// </summary>
public class BombBehaviour : MonoBehaviour, IShootable
{
    [Header("Feedback")]
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Color hitColor = Color.green;
    [SerializeField] private float hitColorDuration = 0.2f;
    [SerializeField] private GameObject explosionParticles;

    private bool _isActive = true;

    public void CheckHit(ProjectileData projectileData)
    {
        if (!_isActive)
        {
            return;
        }
        Debug.Log("CheckHit: " + projectileData.name + " has projectile data");

        OnValidHit();
    }

    private void OnValidHit()
    {
        _isActive = false;
        // Add score
        ScoreManager.Instance?.GetDamage();

        Instantiate(explosionParticles, transform.position, transform.rotation);

        if (_renderer == null)
        {
            Destroy(gameObject);
            return;
        }

        _renderer.material.DOColor(hitColor, hitColorDuration).OnComplete(() =>
        {
            _renderer.material.color = Color.white;
        }).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}

