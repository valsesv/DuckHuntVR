using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles controller input for pausing the game.
/// Listens for 'N' key press (maps to Secondary Button on controllers).
/// </summary>
public class PauseInputHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameController _gameController;

    private InputAction _pauseAction;

    private void Awake()
    {
        // Create input action for pause (N key / Secondary Button)
        _pauseAction = new InputAction(
            name: "Pause",
            type: InputActionType.Button
        );

        // Also bind to controller secondary buttons (works with real controllers)
        _pauseAction.AddBinding("<XRController>{LeftHand}/{SecondaryButton}");
        _pauseAction.AddBinding("<XRController>{RightHand}/{SecondaryButton}");

        _pauseAction.performed += OnPausePressed;
    }

    private void OnEnable()
    {
        _pauseAction?.Enable();
    }

    private void OnDisable()
    {
        _pauseAction?.Disable();
    }

    private void OnDestroy()
    {
        _pauseAction?.Dispose();
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        _gameController.TogglePause();
    }
}
