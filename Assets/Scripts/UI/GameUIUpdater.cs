using UnityEngine;
using TMPro;

/// <summary>
/// Updates the game UI to display weapon information, ammo count, targets remaining, and provides a menu button.
/// Matches the UI design shown in the game screenshot.
/// </summary>
public class GameUIUpdater : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _currentScoreText;

    private void Start()
    {
        ScoreManager.Instance.onScoreChanged.AddListener(UpdateCurrentScore);
    }

    private void OnDestroy()
    {
        ScoreManager.Instance.onScoreChanged.RemoveListener(UpdateCurrentScore);
    }

    private void UpdateCurrentScore(int currentScore)
    {
        _currentScoreText.text = currentScore.ToString();
    }
}

