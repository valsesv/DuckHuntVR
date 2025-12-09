using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles controller input for firing the weapon.
/// Listens for Trigger button on controllers (maps to left mouse click in XR Device Simulator).
/// </summary>
public class WeaponFireInputHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponBehaviour _weaponBehaviour;

    private InputAction _fireAction;

    private void Awake()
    {
        // Create input action for fire (Trigger button)
        _fireAction = new InputAction(
            name: "Fire",
            type: InputActionType.Button
        );

        // Bind to controller triggers (works with real controllers and XR Device Simulator)
        // In XR Device Simulator, left mouse click maps to trigger
        _fireAction.AddBinding("<XRController>{RightHand}/{Trigger}");

        _fireAction.performed += OnFirePressed;
    }

    private void OnEnable()
    {
        _fireAction?.Enable();
    }

    private void OnDisable()
    {
        _fireAction?.Disable();
    }

    private void OnDestroy()
    {
        _fireAction?.Dispose();
    }

    private void OnFirePressed(InputAction.CallbackContext context)
    {
        Debug.Log("OnFirePressed");
        if (_weaponBehaviour != null)
        {
            _weaponBehaviour.Fire();
        }
    }
}
