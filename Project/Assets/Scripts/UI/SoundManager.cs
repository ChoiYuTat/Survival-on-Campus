using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{

    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private Slider musicSlider, soundSlider;

    private void Start()
    {
        SetMusicVolume();
        SetSoundVolume();
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSoundVolume()
    {
        float volume = soundSlider.value;
        audioMixer.SetFloat("SoundVolume", Mathf.Log10(volume) * 20);
    }
}
