using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Singleton score manager that tracks and manages player score.
/// Provides events for score changes to allow UI updates.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int _lifeCount = 3;
    private static ScoreManager _instance;

    [Header("Events")]
    [HideInInspector] public UnityEvent<int> onScoreChanged;
    [HideInInspector] public UnityEvent<int> onLevelLost;

    private int _currentScore;
    private int _maxScore;
    private int _lives;

    /// <summary>
    /// Singleton instance of the ScoreManager.
    /// </summary>
    public static ScoreManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ScoreManager>();

                if (_instance == null)
                {
                    GameObject scoreManagerObject = new GameObject("ScoreManager");
                    _instance = scoreManagerObject.AddComponent<ScoreManager>();
                    DontDestroyOnLoad(scoreManagerObject);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Reset()
    {
        _currentScore = 0;
        _lives = _lifeCount;
        onScoreChanged?.Invoke(_currentScore);
    }

    /// <summary>
    /// Adds points to the current score.
    /// </summary>
    /// <param name="points">Number of points to add (defaults to pointsPerHit if not specified)</param>
    public void AddPoints(int points = 1)
    {
        _currentScore += points;
        onScoreChanged?.Invoke(_currentScore);
        if (_currentScore >= _maxScore)
        {
            _maxScore = _currentScore;
        }
    }

    public void GetDamage(int damage)
    {
        _lives -= damage;
        if (_lives <= 0)
        {
            onLevelLost?.Invoke(_lives);
        }
    }
}

