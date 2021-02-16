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

    [SerializeField] private AudioMixer MasterAudioMixer = null;

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
        optionController.OnOptionMasterVolumeUpdatedEvent += OnOptionMasterVolumeUpdatedEvent;
        optionController.OnOptionAmbientVolumeUpdatedEvent += OnOptionAmbientVolumeUpdatedEvent;
        optionController.OnOptionMusicVolumeUpdatedEvent += OnOptionMusicVolumeUpdatedEvent;
        optionController.OnOptionSoundEffectVolumeUpdatedEvent +=OnOptionSoundEffectVolumeUpdatedEvent;
    }
    private void OnDisable()
    {
        optionController.OnOptionMasterVolumeUpdatedEvent -= OnOptionMasterVolumeUpdatedEvent;
        optionController.OnOptionAmbientVolumeUpdatedEvent -= OnOptionAmbientVolumeUpdatedEvent;
        optionController.OnOptionMusicVolumeUpdatedEvent -= OnOptionMusicVolumeUpdatedEvent;
        optionController.OnOptionSoundEffectVolumeUpdatedEvent -= OnOptionSoundEffectVolumeUpdatedEvent;
    }
    #endregion

    #region Event Callbacks
    private void OnOptionMasterVolumeUpdatedEvent()
    {
        MasterAudioMixer.SetFloat("MasterVolume", MinSoundValue + ((MaxSoundValue - MinSoundValue) * optionController.MasterVolume.value));
    }

    private void OnOptionAmbientVolumeUpdatedEvent()
    {
        MasterAudioMixer.SetFloat("AmbientVolume", MinSoundValue + ((MaxSoundValue - MinSoundValue) * optionController.AmbientVolume.value));
    }

    private void OnOptionMusicVolumeUpdatedEvent()
    {
        MasterAudioMixer.SetFloat("MusicVolume", MinSoundValue + ((MaxSoundValue - MinSoundValue) * optionController.MusicVolume.value));
    }

    private void OnOptionSoundEffectVolumeUpdatedEvent()
    {
        MasterAudioMixer.SetFloat("SoundEffectVolume", MinSoundValue + ((MaxSoundValue - MinSoundValue) * optionController.SoundEffectVolume.value));
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
    #endregion
}
