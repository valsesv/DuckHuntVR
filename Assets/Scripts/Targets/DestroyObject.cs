using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Destroys the object after a timer expires.
/// Can display a countdown UI and optionally damage the player when the timer finishes.
/// </summary>
public class DestroyObject : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float destroyTimer = 5f;

    [Header("Player Damage")]
    [SerializeField] private bool damagePlayerOnDestroy = false;

    [Header("UI Display")]
    [SerializeField] private Slider timerSlider;

    private float _currentTimer = 0f;
    private bool _isTimerActive = false;

    private void Start()
    {
        _isTimerActive = true;
    }

    private void Update()
    {
        if (!_isTimerActive)
        {
            return;
        }
        _currentTimer += Time.deltaTime;

        if (timerSlider != null)
        {
            UpdateTimerDisplay();
        }

        if (_currentTimer >= destroyTimer)
        {
            OnTimerFinished();
        }
    }

    private void UpdateTimerDisplay()
    {
        float normalizedTime = _currentTimer / destroyTimer;
        timerSlider.value = normalizedTime;
    }

    private void OnTimerFinished()
    {
        _isTimerActive = false;
        if (damagePlayerOnDestroy)
        {
            ScoreManager.Instance?.GetDamage();
        }

        // Destroy the object
        Destroy(gameObject);
    }
}
