using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Singleton;

    [SerializeField] private AudioSource _musicSource, _effectsSource, _musicDarkSource;
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

    //Volume 0-1
    //At the start of the game -> the gameplay level, we put the normal music on PlayMusic and at same time at PlayDarkMusic
    //When we enter or leave the mode? We just change the volume to 0 based on the mode!
    public void PlayDarkMusic(AudioClip clip, float volume){
        _musicDarkSource.clip = clip;
        _musicDarkSource.volume = volume;
    }

    public void ChangeMasterVolume(float value){
        AudioListener.volume = value;
    }

    public void ToggleEffects(){
        _effectsSource.mute = !_effectsSource.mute;
    }
    
    public void ToggleMusic(){
        _musicSource.mute = !_musicSource.mute;
        _musicDarkSource.mute = !_musicSource.mute;
    }
}
