using UnityEngine;

/// <summary>
/// ScriptableObject that holds data for a level, including the level prefab and configuration.
/// Create instances of this asset in the Unity Editor to define different levels.
/// </summary>
[CreateAssetMenu(fileName = "New Level Data", menuName = "VR Project/Level Data", order = 1)]
public class LevelListData : ScriptableObject
{
    [System.Serializable]
    public class LevelData
    {
        public GameObject levelPrefab;
        public int targetScore;
    }

    public LevelData[] levels;

    public int currentLevelIndex;
}

