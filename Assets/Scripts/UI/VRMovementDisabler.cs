using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using System.Reflection;

/// <summary>
/// Disables all player movement in VR, including locomotion providers and XR Device Simulator keyboard translation.
/// </summary>
public class VRMovementDisabler : MonoBehaviour
{
    /// <summary>
    /// Disables all locomotion providers (movement and teleportation) for the player.
    /// Also disables XR Device Simulator keyboard translation.
    /// </summary>
    public void Start()
    {
        DisableLocomotionProviders();
        DisableXRDeviceSimulatorMovement();
    }

    /// <summary>
    /// Disables all locomotion providers (movement and teleportation) for the player.
    /// </summary>
    private void DisableLocomotionProviders()
    {
        // Find all locomotion providers in the scene
        LocomotionProvider[] locomotionProviders = FindObjectsOfType<LocomotionProvider>(true);

        foreach (LocomotionProvider provider in locomotionProviders)
        {
            if (provider != null)
            {
                provider.enabled = false;
                Debug.Log($"Disabled locomotion provider: {provider.GetType().Name} on {provider.gameObject.name}");
            }
        }
    }

    /// <summary>
    /// Disables keyboard translation in XR Device Simulator by setting translation speeds to 0
    /// and disabling the keyboard translate input actions.
    /// </summary>
    private void DisableXRDeviceSimulatorMovement()
    {
        // Try to find XRDeviceSimulator component using reflection
        MonoBehaviour[] allBehaviours = FindObjectsOfType<MonoBehaviour>(true);

        foreach (MonoBehaviour behaviour in allBehaviours)
        {
            if (behaviour == null)
                continue;

            System.Type behaviourType = behaviour.GetType();

            // Check if this is XRDeviceSimulator (from UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation namespace)
            if (behaviourType.Name == "XRDeviceSimulator" &&
                behaviourType.Namespace == "UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation")
            {
                try
                {
                    // Use reflection to set keyboard translation speeds to 0
                    FieldInfo keyboardXSpeedField = behaviourType.GetField("m_KeyboardXTranslateSpeed",
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    FieldInfo keyboardYSpeedField = behaviourType.GetField("m_KeyboardYTranslateSpeed",
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    FieldInfo keyboardZSpeedField = behaviourType.GetField("m_KeyboardZTranslateSpeed",
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

                    if (keyboardXSpeedField != null)
                        keyboardXSpeedField.SetValue(behaviour, 0f);
                    if (keyboardYSpeedField != null)
                        keyboardYSpeedField.SetValue(behaviour, 0f);
                    if (keyboardZSpeedField != null)
                        keyboardZSpeedField.SetValue(behaviour, 0f);

                    // Also try to disable the input actions
                    PropertyInfo keyboardXActionProp = behaviourType.GetProperty("keyboardXTranslateAction",
                        BindingFlags.Public | BindingFlags.Instance);
                    PropertyInfo keyboardYActionProp = behaviourType.GetProperty("keyboardYTranslateAction",
                        BindingFlags.Public | BindingFlags.Instance);
                    PropertyInfo keyboardZActionProp = behaviourType.GetProperty("keyboardZTranslateAction",
                        BindingFlags.Public | BindingFlags.Instance);

                    // Try to get the actions and disable them
                    if (keyboardXActionProp != null)
                    {
                        var xAction = keyboardXActionProp.GetValue(behaviour);
                        if (xAction != null && xAction.GetType().Name == "InputActionReference")
                        {
                            var actionProperty = xAction.GetType().GetProperty("action");
                            if (actionProperty != null)
                            {
                                var action = actionProperty.GetValue(xAction);
                                if (action != null)
                                {
                                    var disableMethod = action.GetType().GetMethod("Disable");
                                    if (disableMethod != null)
                                        disableMethod.Invoke(action, null);
                                }
                            }
                        }
                    }

                    // Similar for Y and Z actions
                    if (keyboardYActionProp != null)
                    {
                        var yAction = keyboardYActionProp.GetValue(behaviour);
                        if (yAction != null && yAction.GetType().Name == "InputActionReference")
                        {
                            var actionProperty = yAction.GetType().GetProperty("action");
                            if (actionProperty != null)
                            {
                                var action = actionProperty.GetValue(yAction);
                                if (action != null)
                                {
                                    var disableMethod = action.GetType().GetMethod("Disable");
                                    if (disableMethod != null)
                                        disableMethod.Invoke(action, null);
                                }
                            }
                        }
                    }

                    if (keyboardZActionProp != null)
                    {
                        var zAction = keyboardZActionProp.GetValue(behaviour);
                        if (zAction != null && zAction.GetType().Name == "InputActionReference")
                        {
                            var actionProperty = zAction.GetType().GetProperty("action");
                            if (actionProperty != null)
                            {
                                var action = actionProperty.GetValue(zAction);
                                if (action != null)
                                {
                                    var disableMethod = action.GetType().GetMethod("Disable");
                                    if (disableMethod != null)
                                        disableMethod.Invoke(action, null);
                                }
                            }
                        }
                    }

                    Debug.Log($"Disabled XR Device Simulator keyboard translation on {behaviour.gameObject.name}");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Could not disable XR Device Simulator movement: {e.Message}");
                }
            }
        }
    }
}
