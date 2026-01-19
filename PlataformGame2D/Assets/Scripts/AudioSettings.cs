using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    private const string MusicVolumeKey = "MusicVolume";

    [SerializeField] private Slider musicSlider;

    private void Start()
    {
        float saved = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        ApplyVolume(saved);

        if (musicSlider != null)
        {
            musicSlider.value = saved;
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        }
    }

    private void OnDestroy()
    {
        if (musicSlider != null)
            musicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
    }

    public void OnMusicSliderChanged(float value)
    {
        ApplyVolume(value);
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
        PlayerPrefs.Save();
    }

    private void ApplyVolume(float value)
    {
        // Volumen global simple (afecta a todo)
        AudioListener.volume = Mathf.Clamp01(value);
    }
}
