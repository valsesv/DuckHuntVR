using UnityEngine;

/// <summary>
/// Spawns targets and bombs within a 3D spawn area.
/// Manages target count based on time intervals.
/// </summary>
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
    [SerializeField][Range(0f, 1f)] private float bombSpawnChance = 0.2f; // 20% chance to spawn bomb

    private int _requiredTargets = 0;
    private float _currentSpawnInterval;
    private float _nextLevelUpTime = 0f;

    private void Update()
    {
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
