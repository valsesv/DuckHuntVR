using UnityEngine;

/// <summary>
/// Controls all game state including menu, pause, and lose functionality.
/// Manages time scale and panel visibility.
/// </summary>
public class GameController : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _losePanel;

    [Header("Game References")]
    [SerializeField] private TargetSpawner _targetSpawner;

    private bool _isPaused = false;
    private bool _isGameOver = false;

    private void Start()
    {
        ScoreManager.Instance.onLevelLost.AddListener(OnLevelLost);

        ResumeGame();
    }

    private void OnDestroy()
    {
        ScoreManager.Instance.onLevelLost.RemoveListener(OnLevelLost);
    }

    public void PauseGame()
    {
        if (_isGameOver)
        {
            return;
        }

        _isPaused = true;
        Time.timeScale = 0f;
        _pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        _pausePanel.SetActive(false);
    }

    public void TogglePause()
    {
        if (_gamePanel.activeSelf == false && _pausePanel.activeSelf == false)
        {
            return;
        }

        if (_isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void OnLevelLost(int lives)
    {
        if (_isGameOver)
        {
            return;
        }

        _isGameOver = true;

        _targetSpawner.StopGame();
        _gamePanel.SetActive(false);
        _losePanel.SetActive(true);
    }

    public void OnStartButtonClicked()
    {
        Time.timeScale = 1f;
        _isPaused = false;
        _isGameOver = false;

        _menuPanel.SetActive(false);
        _gamePanel.SetActive(true);
        ScoreManager.Instance.Reset();
        _targetSpawner.StartGame();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        _isPaused = false;
        _isGameOver = false;

        _targetSpawner.StopGame();
        _gamePanel.SetActive(false);
        _pausePanel.SetActive(false);
        _losePanel.SetActive(false);
        _menuPanel.SetActive(true);

        ScoreManager.Instance.Reset();
    }
}
