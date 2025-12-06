using UnityEngine;

/// <summary>
/// Spawns a level from LevelListData based on the current level index on Start.
/// </summary>
public class LevelSpawner : MonoBehaviour
{
    [SerializeField] private LevelListData levelListData;
    [SerializeField] private Transform spawnParent;

    private void Awake()
    {
        int levelIndex = levelListData.currentLevelIndex;

        if (levelIndex < 0 || levelIndex >= levelListData.levels.Length)
        {
            Debug.LogError($"LevelSpawner: Invalid level index {levelIndex}!");
            return;
        }

        var levelData = levelListData.levels[levelIndex];

        if (levelData.levelPrefab == null)
        {
            Debug.LogError($"LevelSpawner: Level prefab is null at index {levelIndex}!");
            return;
        }

        Instantiate(levelData.levelPrefab, spawnParent);

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SetTargetScore(levelData.targetScore);
        }
    }
}
