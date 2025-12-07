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
    [HideInInspector] public UnityEvent<int> onLivesChanged;
    [HideInInspector] public UnityEvent<int> onLevelLost;

    private int _currentScore;
    private int _maxScore;
    private int _lives;

    public int maxScore => _maxScore;

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
        Reset();
    }

    public void Reset()
    {
        _currentScore = 0;
        _lives = _lifeCount;
        onScoreChanged?.Invoke(_currentScore);
        onLivesChanged?.Invoke(_lives);
    }

    [ContextMenu("Add Points")]
    public void AddPoints(int points = 1)
    {
        _currentScore += points;
        onScoreChanged?.Invoke(_currentScore);
        if (_currentScore >= _maxScore)
        {
            _maxScore = _currentScore;
        }
    }

    [ContextMenu("Get Damage")]
    public void GetDamage(int damage = 1)
    {
        _lives = Mathf.Max(0, _lives - damage);
        onLivesChanged?.Invoke(_lives);
        if (_lives <= 0)
        {
            onLevelLost?.Invoke(_lives);
        }
    }
}

