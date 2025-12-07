using UnityEngine;

/// <summary>
/// Singleton sound manager that controls the master volume of the game.
/// Provides methods to set and get the volume level.
/// </summary>
public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    private const string VOLUME_PREF_KEY = "MasterVolume";

    [Header("Audio Settings")]
    [SerializeField] private float _defaultVolume = 0.5f;

    private float _currentVolume;

    /// <summary>
    /// Singleton instance of the SoundManager.
    /// </summary>
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SoundManager>();

                if (_instance == null)
                {
                    GameObject soundManagerObject = new GameObject("SoundManager");
                    _instance = soundManagerObject.AddComponent<SoundManager>();
                    DontDestroyOnLoad(soundManagerObject);
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// Current volume level (0 to 1).
    /// </summary>
    public float Volume
    {
        get => _currentVolume;
        private set
        {
            _currentVolume = Mathf.Clamp01(value);
            ApplyVolume();
            SaveVolume();
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolume();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ApplyVolume();
    }

    /// <summary>
    /// Sets the master volume level.
    /// </summary>
    /// <param name="volume">Volume level between 0 and 1.</param>
    public void SetVolume(float volume)
    {
        Volume = volume;
    }

    private void ApplyVolume()
    {
        // Set the master volume using AudioListener
        AudioListener.volume = _currentVolume;
    }

    private void LoadVolume()
    {
        // Load saved volume from PlayerPrefs, or use default
        _currentVolume = PlayerPrefs.GetFloat(VOLUME_PREF_KEY, _defaultVolume);
    }

    private void SaveVolume()
    {
        // Save volume to PlayerPrefs for persistence
        PlayerPrefs.SetFloat(VOLUME_PREF_KEY, _currentVolume);
        PlayerPrefs.Save();
    }
}
