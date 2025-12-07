using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Updates the game UI to display weapon information, ammo count, targets remaining, and provides a menu button.
/// Matches the UI design shown in the game screenshot.
/// </summary>
public class GameUIUpdater : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _currentScoreText;
    [SerializeField] private Transform _livesContainer;
    [SerializeField] private float _lifeFadeDuration = 0.5f;

    private Image[] _livesImages;

    private void Start()
    {
        _livesImages = _livesContainer.GetComponentsInChildren<Image>();
        ScoreManager.Instance.onScoreChanged.AddListener(UpdateCurrentScore);
        ScoreManager.Instance.onLivesChanged.AddListener(UpdateLives);
    }

    private void OnDestroy()
    {
        ScoreManager.Instance.onScoreChanged.RemoveListener(UpdateCurrentScore);
        ScoreManager.Instance.onLivesChanged.RemoveListener(UpdateLives);
    }

    private void UpdateCurrentScore(int currentScore)
    {
        _currentScoreText.text = currentScore.ToString();
    }

    private void UpdateLives(int lives)
    {
        for (int i = lives - 1; i < _livesImages.Length; i++)
        {
            _livesImages[i].DOFade(0, _lifeFadeDuration);
        }
    }
}

