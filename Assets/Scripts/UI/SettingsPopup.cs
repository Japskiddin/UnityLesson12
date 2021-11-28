using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : MonoBehaviour
{
    [SerializeField] private AudioClip sound; // ссылка на звуковой клип
    [SerializeField] private Slider soundsSlider;
    [SerializeField] private Slider musicSlider;

    public void Refresh()
    {
        soundsSlider.value = Managers.Audio.soundVolume;
        musicSlider.value = Managers.Audio.musicVolume;
    }

    public void OnSoundToggle() { // кнопка переключает свойство mute диспетчера управления звуком
        Managers.Audio.soundMute = !Managers.Audio.soundMute;
        Managers.Audio.PlaySound(sound); // воспоизводится звуковой эффект при нажатии на кнопку
    }

    public void OnSoundValue(float volume) { // ползунок регулирует свойство volume диспетчера управления звуком
        Managers.Audio.soundVolume = volume;
    }

    public void OnMusicToggle() {
        Managers.Audio.musicMute = !Managers.Audio.musicMute;
        Managers.Audio.PlaySound(sound);
    }

    public void OnMusicValue(float volume) {
        Managers.Audio.musicVolume = volume;
    }
}
