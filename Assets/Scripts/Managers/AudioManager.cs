using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, IGameManager
{
    [SerializeField] private AudioSource music1Source;
    [SerializeField] private AudioSource music2Source;
    [SerializeField] private AudioSource soundSource; // €чейка переменной на панели дл€ ссылки на новый источник звука
    [SerializeField] private string introBGMusic; // строки указывают имена музыкальных клипов
    [SerializeField] private string levelBGMusic;
    private AudioSource _activeMusic;
    private AudioSource _inactiveMusic; // следим за тем, какой из источников активен, а какой нет

    public float crossFadeRate = 1.5f;
    private bool _crossFading; // переключатель, позвол€ющий избежать ошибок в процессе перехода

    public ManagerStatus status { get; private set; }
    private NetworkService _network;
    private float _musicVolume; // непосредственный доступ к закрытой переменной невозможен, только через функцию задани€ свойства
    public float musicVolume
    {
        get
        {
            return _musicVolume;
        }
        set
        {
            _musicVolume = value;

            if (music1Source != null && !_crossFading)
            { // напр€мую регулируем громкость источника звука
                music1Source.volume = _musicVolume;
                music2Source.volume = _musicVolume; // регулировка громкости обоих источников звука
            }
        }
    }

    public bool musicMute
    {
        get
        {
            if (music1Source != null)
            {
                return music1Source.mute;
            }
            return false; // значение по умолчанию если AudioSource отсутствует
        }
        set
        {
            if (music1Source != null)
            {
                music1Source.mute = value;
                music2Source.mute = value;
            }
        }
    }

    public float soundVolume
    { // свойство дл€ громкости с функцией чтени€ и функцией доступа
        get { return AudioListener.volume; } // реализуем функции чтени€/доступа с помощью AudioListener
        set { AudioListener.volume = value; }
    }

    public bool soundMute
    { // добавл€ем аналогичное свойство дл€ выключени€ звука
        get { return AudioListener.pause; }
        set { AudioListener.pause = value; }
    }

    public void Startup(NetworkService service)
    {
        Debug.Log("Audio manager starting...");
        _network = service;

        music1Source.ignoreListenerVolume = true; // свойства заставл€ют AudioSource игнорировать громкость компонента AudioListener
        music1Source.ignoreListenerPause = true;
        music2Source.ignoreListenerVolume = true;
        music2Source.ignoreListenerPause = true;

        musicVolume = 1;
        soundVolume = 1f; // 1 - полна€ громкость

        _activeMusic = music1Source; // инициализируем один из источников как активный
        _inactiveMusic = music2Source;

        status = ManagerStatus.Started;
    }

    public void PlaySound(AudioClip clip)
    { // воспроизводим звуки, не имеющие другого источника
        soundSource.PlayOneShot(clip);
    }

    public void PlayIntroMusic()
    { // загрузка музыки intro из папки Resources
        PlayMusic(Resources.Load("Music/" + introBGMusic) as AudioClip);
    }

    public void PlayLevelMusic()
    { // загрузка основной музыки из папки Resources
        PlayMusic(Resources.Load("Music/" + levelBGMusic) as AudioClip);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (_crossFading) return;
        StartCoroutine(CrossFadeMusic(clip)); // при изменении музыкальной композиции вызываем сопрограмму
    }

    private IEnumerator CrossFadeMusic(AudioClip clip)
    {
        _crossFading = true;

        _inactiveMusic.clip = clip;
        _inactiveMusic.volume = 0;
        _inactiveMusic.Play();

        float scaledRate = crossFadeRate * _musicVolume;
        while (_activeMusic.volume > 0)
        {
            _activeMusic.volume -= scaledRate * Time.deltaTime;
            _inactiveMusic.volume += scaledRate * Time.deltaTime;

            yield return null; // оператор yield останавливает операции на один кадр
        }

        AudioSource temp = _activeMusic; // временна€ переменна€, используема€ когда мы мен€ем местами _active и _inactive

        _activeMusic = _inactiveMusic;
        _activeMusic.volume = _musicVolume;

        _inactiveMusic = temp;
        _inactiveMusic.Stop();

        _crossFading = false;
    }

    public void StopMusic()
    {
        _activeMusic.Stop();
        _inactiveMusic.Stop();
    }
}
