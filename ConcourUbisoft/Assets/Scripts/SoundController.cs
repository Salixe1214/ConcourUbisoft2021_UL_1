using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioClip ButtonSound = null;
    [SerializeField] private AudioSource AudioSource = null;

    [SerializeField] private AudioClip MenuSong = null;
    [SerializeField] private AudioSource MenuSongSource = null;

    [SerializeField] private AudioClip AmbientSound = null;
    [SerializeField] private AudioSource AmbientSource = null;

    [SerializeField] private AudioSource EffectSource = null;

    [SerializeField] private AudioClip LevelOneErrorSound;
    [SerializeField] private AudioClip LevelOneSuccesSound;

    [SerializeField] private AudioClip LevelSuccessSound;

    [SerializeField] private AudioMixer MasterAudioMixer = null;
    [SerializeField] private AudioMixer _ambientAudioMixer = null;
    [SerializeField] private AudioMixer _musicAudioMixer = null;
    [SerializeField] private AudioMixer _soundEffectAudioMixer = null;
    

    [SerializeField] private float MaxSoundValue = 0;
    [SerializeField] private float MinSoundValue = -10;


    private OptionController optionController = null;
    private GameController gameController = null;

    #region Unity Callbacks
    private void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        optionController = gameController.OptionController;
    }
    private void OnEnable()
    {
        optionController.OnOptionVolumeUpdatedEvent += OnOptionVolumeUpdatedEvent;
    }
    private void OnDisable()
    {
        optionController.OnOptionVolumeUpdatedEvent -= OnOptionVolumeUpdatedEvent;
    }
    #endregion
    #region Event Callbacks
    private void OnOptionVolumeUpdatedEvent(OptionController.SoundChannel channel)
    {
        switch (channel)
        {
            case OptionController.SoundChannel.Master:
                MasterAudioMixer.SetFloat("MasterVolume", MinSoundValue + ((MaxSoundValue - MinSoundValue) * optionController.GetVolume(channel)));
                break;
            case OptionController.SoundChannel.Ambient:
                MasterAudioMixer.SetFloat("AmbientVolume", MinSoundValue + ((MaxSoundValue - MinSoundValue) * optionController.GetVolume(channel)));
                break;
            case OptionController.SoundChannel.Music:
                MasterAudioMixer.SetFloat("MusicVolume", MinSoundValue + ((MaxSoundValue - MinSoundValue) * optionController.GetVolume(channel)));
                break;
            case OptionController.SoundChannel.SoundEffect:
                MasterAudioMixer.SetFloat("SoundEffectVolume", MinSoundValue + ((MaxSoundValue - MinSoundValue) * optionController.GetVolume(channel)));
                break;
            case OptionController.SoundChannel.VoiceChat:
                MasterAudioMixer.SetFloat("VoiceChatVolume", MinSoundValue + ((MaxSoundValue - MinSoundValue) * optionController.GetVolume(channel)));
                break;
        }
        
    }
    #endregion
    #region Public Functions
    public void PlayButtonSound()
    {
        AudioSource.PlayOneShot(ButtonSound);
    }
    public void PlayMenuSong()
    {
        MenuSongSource.clip = MenuSong;
        MenuSongSource.time = 0;
        MenuSongSource.Play();
    }
    public void StopMenuSong()
    {
        MenuSongSource.Stop();
    }
    public void PlayAmbientSound()
    {
        AmbientSource.clip = AmbientSound;
        AmbientSource.time = 0;
        AmbientSource.Play();
    }
    public void StopAmbientSound()
    {
        AmbientSource.Stop();
    }

    public void PlayLevelOneErrorSound()
    {
        EffectSource.clip = LevelOneErrorSound;
        EffectSource.time = 0;
        EffectSource.Play();
    }
    
    public void PlayLevelOneSuccessSound()
    {
        EffectSource.clip = LevelOneSuccesSound;
        EffectSource.time = 0;
        EffectSource.Play();
    }
    
    public void PlayLevelClearSuccessSound()
    {
        EffectSource.clip = LevelSuccessSound;
        EffectSource.time = 0;
        EffectSource.Play();
    }

    public void SetSound(GameController.Role role)
    {
        if(role == GameController.Role.SecurityGuard)
        {
            _ambientAudioMixer.SetFloat("TechVolume", -80.0f);
            _musicAudioMixer.SetFloat("TechVolume", -80.0f);
            _soundEffectAudioMixer.SetFloat("TechVolume", -80.0f);

            _ambientAudioMixer.SetFloat("GuardVolume", 0);
            _musicAudioMixer.SetFloat("GuardVolume", 0);
            _soundEffectAudioMixer.SetFloat("GuardVolume", 0);
        }
        else if(role == GameController.Role.Technician)
        {
            _ambientAudioMixer.SetFloat("TechVolume", 0);
            _musicAudioMixer.SetFloat("TechVolume", 0);
            _soundEffectAudioMixer.SetFloat("TechVolume", 0);

            _ambientAudioMixer.SetFloat("GuardVolume", -80.0f);
            _musicAudioMixer.SetFloat("GuardVolume", -80.0f);
            _soundEffectAudioMixer.SetFloat("GuardVolume", -80.0f);
        }
    }
    #endregion
}
