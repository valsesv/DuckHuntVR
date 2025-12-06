using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Singleton score manager that tracks and manages player score.
/// Provides events for score changes to allow UI updates.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    private static ScoreManager _instance;

    [Header("Events")]
    [HideInInspector] public UnityEvent<int> onScoreChanged;
    [HideInInspector] public UnityEvent onLevelComplete;

    private int _targetsLeft;
    private int _targetScore;

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


    /// <summary>
    /// Whether the level is complete (current score >= target score).
    /// </summary>
    public bool IsLevelComplete => _targetScore > 0 && _targetsLeft >= _targetScore;

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

    /// <summary>
    /// Adds points to the current score.
    /// </summary>
    /// <param name="points">Number of points to add (defaults to pointsPerHit if not specified)</param>
    public void AddPoints(int points = 1)
    {
        _targetsLeft -= points;
        onScoreChanged?.Invoke(_targetsLeft);

        // Check if level is complete
        if (IsLevelComplete)
        {
            onLevelComplete?.Invoke();
        }
    }

    /// <summary>
    /// Sets the target score (total targets needed to complete the level).
    /// </summary>
    /// <param name="targetScore">The target score to set</param>
    public void SetTargetScore(int targetScore)
    {
        SetScore(targetScore);
    }

    private void SetScore(int score)
    {
        _targetsLeft = score;
        onScoreChanged?.Invoke(_targetsLeft);
    }
}

