using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Singleton;

    [SerializeField] private AudioSource _musicSource, _effectsSource;
    [SerializeField] private bool _toggleMusic, _toggleEffects;

    private void Awake() {
        if(Singleton == null){
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip){
        _effectsSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip){
        _musicSource.clip = clip;
        _musicSource.Play();
    }

    public void ChangeMasterVolume(float value){
        AudioListener.volume = value;
    }

    public void ToggleEffects(){
        _effectsSource.mute = !_effectsSource.mute;
    }
    
    public void ToggleMusic(){
        _musicSource.mute = !_musicSource.mute;
    }
}
