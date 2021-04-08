using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioClip ButtonSound = null;
    [SerializeField] private AudioSource AudioSourceSoundEffect = null;

    [SerializeField] private AudioClip MenuSong = null;
    [SerializeField] private AudioSource MenuSongSource = null;

    [SerializeField] private AudioClip AmbientSound = null;
    [SerializeField] private AudioSource AmbientSource = null;

    [SerializeField] private AudioSource EffectSource = null;

    [SerializeField] private AudioClip LevelOneErrorSound;
    [SerializeField] private AudioClip LevelOneSuccesSound;

    [SerializeField] private AudioClip LevelSuccessSound;

    [SerializeField] private AudioClip Area2Track;
    [SerializeField] private AudioClip Area1Track;

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

    private void Start()
    {
        MasterAudioMixer.SetFloat("MasterVolume", Mathf.Log10(0.33f)*20);
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
                MasterAudioMixer.SetFloat("MasterVolume", Mathf.Log10(optionController.GetVolume(channel))*20);
                Debug.LogWarning(Mathf.Log10(optionController.GetVolume(channel))*20);
                break;
            case OptionController.SoundChannel.Ambient:
                MasterAudioMixer.SetFloat("AmbientVolume", Mathf.Log10(optionController.GetVolume(channel))*20);
                break;
            case OptionController.SoundChannel.Music:
                MasterAudioMixer.SetFloat("MusicVolume", Mathf.Log10(optionController.GetVolume(channel))*20);
                break;
            case OptionController.SoundChannel.SoundEffect:
                MasterAudioMixer.SetFloat("SoundEffectVolume", Mathf.Log10(optionController.GetVolume(channel))*20);
                break;
            case OptionController.SoundChannel.VoiceChat:
                MasterAudioMixer.SetFloat("VoiceChatVolume", Mathf.Log10(optionController.GetVolume(channel))*20);
                break;
        }
        
    }
    #endregion
    #region Public Functions
    public void PlayButtonSound()
    {
        AudioSourceSoundEffect.PlayOneShot(ButtonSound);
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
    
    public void PlayLevelPartialSequenceSuccessSound()
    {
        EffectSource.clip = LevelOneSuccesSound;
        EffectSource.time = 0;
        EffectSource.Play();
    }
    
    public void PlayLevelSequenceClearedSuccessSound()
    {
        EffectSource.clip = LevelSuccessSound;
        EffectSource.time = 0;
        EffectSource.Play();
    }

    public void PlayArea2Music()
    {
        MenuSongSource.clip = Area2Track;
        MenuSongSource.time = 0;
        MenuSongSource.loop = true;
        MenuSongSource.Play();
    }

    public void StopAreaMusic()
    {
        MenuSongSource.Stop();
    }

    public void MuteAmbient()
    {
        
    }

    public void PlayArea1Music()
    {
        MenuSongSource.clip = Area1Track;
        MenuSongSource.time = 0;
        MenuSongSource.loop = true;
        MenuSongSource.Play();
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
