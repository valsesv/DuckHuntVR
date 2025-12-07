using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float minSpawnHeight = 1f;
    [SerializeField] private float maxSpawnHeight = 5f;

    [Header("Spawn Timing")]
    [SerializeField] private float initialSpawnInterval = 10f; // Every 10 seconds initially
    [SerializeField] private float advancedSpawnInterval = 30f; // Every 30 seconds after 5 targets
    [SerializeField] private int advancedThreshold = 5; // Switch to 30s interval after this many targets

    [Header("Bomb Spawn Chance")]
    [SerializeField][Range(0f, 1f)] private float bombSpawnChance = 0.2f;

    private int _requiredTargets = 0;
    private float _currentSpawnInterval;
    private float _nextLevelUpTime = 0f;
    private bool _isGameStarted = false;

    public void StartGame()
    {
        ClearAllTargets();

        _isGameStarted = true;
        _requiredTargets = 0;
        _currentSpawnInterval = initialSpawnInterval;
        _nextLevelUpTime = Time.time + _currentSpawnInterval;

        LevelUp();
    }

    public void StopGame()
    {
        _isGameStarted = false;
        ClearAllTargets();
    }

    /// <summary>
    /// Clears all spawned targets and bombs.
    /// </summary>
    private void ClearAllTargets()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void Update()
    {
        if (!_isGameStarted)
        {
            return;
        }

        SpawnUntilRequiredCount();

        if (Time.time >= _nextLevelUpTime)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        _requiredTargets++;
        if (_requiredTargets >= advancedThreshold)
        {
            _currentSpawnInterval = advancedSpawnInterval;
        }
        else
        {
            _currentSpawnInterval = initialSpawnInterval;
        }
        _nextLevelUpTime = Time.time + _currentSpawnInterval;
    }

    private void SpawnUntilRequiredCount()
    {
        int currentCount = transform.childCount;

        while (currentCount < _requiredTargets)
        {
            SpawnTarget();
            currentCount++;
        }
    }

    private void SpawnTarget()
    {
        bool spawnBomb = Random.value < bombSpawnChance;
        GameObject prefabToSpawn = spawnBomb ? bombPrefab : targetPrefab;
        Vector3 spawnPosition = GetRandomSpawnPosition();

        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity, transform);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        float randomHeight = Random.Range(minSpawnHeight, maxSpawnHeight);
        return transform.position + new Vector3(randomCircle.x, randomHeight, randomCircle.y);
    }
}
