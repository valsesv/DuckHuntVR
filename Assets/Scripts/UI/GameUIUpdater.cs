using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;

/// <summary>
/// Updates the game UI to display weapon information, ammo count, targets remaining, and provides a menu button.
/// Matches the UI design shown in the game screenshot.
/// </summary>
public class GameUIUpdater : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI ammoCountText;
    [SerializeField] private TextMeshProUGUI targetsLeftText;

    [Header("Settings")]
    [SerializeField] private float updateInterval = 0.1f;

    private WeaponBehaviour _currentWeapon;
    private float _lastUpdateTime;

    private void Start()
    {
        ScoreManager.Instance.onScoreChanged.AddListener(UpdateTargetsRemaining);

        // Subscribe to all weapon grab interactables in the scene
        SubscribeToWeaponEvents();
    }

    private void OnDestroy()
    {
        ScoreManager.Instance.onScoreChanged.RemoveListener(UpdateTargetsRemaining);
        UnsubscribeFromWeaponEvents();
    }

    private void Update()
    {
        // Update ammo display at intervals
        if (Time.time - _lastUpdateTime >= updateInterval)
        {
            UpdateAmmoCount();
            _lastUpdateTime = Time.time;
        }
    }

    /// <summary>
    /// Subscribes to select events for all weapons with XRGrabInteractable components.
    /// </summary>
    private void SubscribeToWeaponEvents()
    {
        var weapons = FindObjectsOfType<WeaponBehaviour>();
        foreach (var weapon in weapons)
        {
            var grabInteractable = weapon.GetComponent<XRGrabInteractable>();
            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.AddListener(OnWeaponSelected);
                grabInteractable.selectExited.AddListener(OnWeaponDeselected);
            }
        }
    }

    /// <summary>
    /// Unsubscribes from weapon events.
    /// </summary>
    private void UnsubscribeFromWeaponEvents()
    {
        var weapons = FindObjectsOfType<WeaponBehaviour>();
        foreach (var weapon in weapons)
        {
            var grabInteractable = weapon.GetComponent<XRGrabInteractable>();
            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.RemoveListener(OnWeaponSelected);
                grabInteractable.selectExited.RemoveListener(OnWeaponDeselected);
            }
        }
    }

    /// <summary>
    /// Called when a weapon is selected/grabbed.
    /// </summary>
    private void OnWeaponSelected(SelectEnterEventArgs args)
    {
        var weapon = args.interactableObject.transform.GetComponent<WeaponBehaviour>();
        if (weapon != null)
        {
            _currentWeapon = weapon;
            UpdateAmmoCount();
        }
    }

    /// <summary>
    /// Called when a weapon is deselected/released.
    /// </summary>
    private void OnWeaponDeselected(SelectExitEventArgs args)
    {
        // Only clear if this was the current weapon
        var weapon = args.interactableObject.transform.GetComponent<WeaponBehaviour>();
        if (weapon == _currentWeapon)
        {
            _currentWeapon = null;
            UpdateAmmoCount();
        }
    }

    /// <summary>
    /// Updates the ammo count display (current/max format).
    /// </summary>
    private void UpdateAmmoCount()
    {
        if (ammoCountText == null) return;

        if (_currentWeapon != null)
        {
            int currentAmmo = _currentWeapon.CurrentAmmo;
            int maxAmmo = GetMaxAmmo(_currentWeapon);
            ammoCountText.text = $"{currentAmmo}/{maxAmmo}";
        }
        else
        {
            ammoCountText.text = "--/--";
        }
    }

    /// <summary>
    /// Updates the targets remaining count.
    /// </summary>
    private void UpdateTargetsRemaining(int targetsLeft)
    {
        targetsLeftText.text = targetsLeft.ToString();
    }

    /// <summary>
    /// Gets the maximum ammo capacity for a weapon.
    /// </summary>
    private int GetMaxAmmo(WeaponBehaviour weapon)
    {
        return weapon.MagazineSize;
    }

    /// <summary>
    /// Sets the current weapon to track. Can be called externally when weapon changes.
    /// </summary>
    public void SetCurrentWeapon(WeaponBehaviour weapon)
    {
        _currentWeapon = weapon;
    }
}

