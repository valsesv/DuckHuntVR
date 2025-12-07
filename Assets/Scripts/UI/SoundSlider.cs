using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI slider component that controls the master volume through the SoundManager.
/// Updates the volume when the slider value changes.
/// </summary>
public class SoundSlider : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    private void Start()
    {
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
        _slider.value = SoundManager.Instance.Volume;
    }

    private void OnDestroy()
    {
        _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetVolume(value);
        }
    }
}
