using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SpawnableObject
{
    public GameObject prefab;
    [Range(0f, 1f)]
    public float spawnChance = 0.5f;
}

public class TargetSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<SpawnableObject> spawnableObjects = new List<SpawnableObject>();
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float minSpawnHeight = 1f;
    [SerializeField] private float maxSpawnHeight = 5f;

    [Header("Spawn Timing")]
    [SerializeField] private float initialSpawnInterval = 10f; // Every 10 seconds initially
    [SerializeField] private float advancedSpawnInterval = 30f; // Every 30 seconds after 5 targets
    [SerializeField] private int advancedThreshold = 5; // Switch to 30s interval after this many targets

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
        GameObject prefabToSpawn = GetRandomPrefab();
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("No valid prefab to spawn. Check spawnableObjects list.");
            return;
        }

        Vector3 spawnPosition = GetRandomSpawnPosition();
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity, transform);
    }

    private GameObject GetRandomPrefab()
    {
        if (spawnableObjects == null || spawnableObjects.Count == 0)
        {
            return null;
        }

        // Filter out null prefabs and calculate total weight
        var validObjects = spawnableObjects.Where(obj => obj != null && obj.prefab != null && obj.spawnChance > 0f).ToList();

        if (validObjects.Count == 0)
        {
            return null;
        }

        // Calculate total weight
        float totalWeight = validObjects.Sum(obj => obj.spawnChance);

        if (totalWeight <= 0f)
        {
            // If all weights are 0, return first valid prefab
            return validObjects[0].prefab;
        }

        // Generate random value between 0 and totalWeight
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        // Select prefab based on weighted random
        foreach (var spawnable in validObjects)
        {
            currentWeight += spawnable.spawnChance;
            if (randomValue <= currentWeight)
            {
                return spawnable.prefab;
            }
        }

        // Fallback to last item (shouldn't happen, but safety check)
        return validObjects[validObjects.Count - 1].prefab;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        float randomHeight = Random.Range(minSpawnHeight, maxSpawnHeight);
        return transform.position + new Vector3(randomCircle.x, randomHeight, randomCircle.y);
    }

    private void OnDrawGizmos()
    {
        // Draw spawn area gizmos
        Gizmos.color = Color.cyan;

        Vector3 center = transform.position;

        // Draw circle at min height
        DrawCircle(center + Vector3.up * minSpawnHeight, spawnRadius, Vector3.up);

        // Draw circle at max height
        DrawCircle(center + Vector3.up * maxSpawnHeight, spawnRadius, Vector3.up);

        // Draw vertical lines connecting the circles (4 cardinal directions)
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };
        foreach (Vector3 dir in directions)
        {
            Vector3 offset = dir * spawnRadius;
            Gizmos.DrawLine(
                center + Vector3.up * minSpawnHeight + offset,
                center + Vector3.up * maxSpawnHeight + offset
            );
        }

        // Draw center line
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            center + Vector3.up * minSpawnHeight,
            center + Vector3.up * maxSpawnHeight
        );
    }

    private void DrawCircle(Vector3 center, float radius, Vector3 normal)
    {
        int segments = 32;
        float angleStep = 360f / segments;

        // Find two perpendicular vectors to the normal
        Vector3 forward = normal == Vector3.up ? Vector3.forward : Vector3.up;
        Vector3 right = Vector3.Cross(normal, forward).normalized;
        forward = Vector3.Cross(right, normal).normalized;

        Vector3 prevPoint = center + right * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 point = center + (right * Mathf.Cos(angle) + forward * Mathf.Sin(angle)) * radius;
            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }
    }
}
