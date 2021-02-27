using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionController : MonoBehaviour
{
    [SerializeField] private Slider _masterVolume = null;
    [SerializeField] private Slider _ambientVolume = null;
    [SerializeField] private Slider _musicVolume = null;
    [SerializeField] private Slider _soundEffectVolume = null;

    public Slider MasterVolume { get => _masterVolume; }
    public Slider AmbientVolume { get => _ambientVolume; }
    public Slider MusicVolume { get => _musicVolume; }
    public Slider SoundEffectVolume { get => _soundEffectVolume; }

    #region Events 
    public event Action OnOptionMasterVolumeUpdatedEvent;
    public event Action OnOptionAmbientVolumeUpdatedEvent;
    public event Action OnOptionSoundEffectVolumeUpdatedEvent;
    public event Action OnOptionMusicVolumeUpdatedEvent;
    #endregion
    #region Public Functions
    public void PublishOptionMasterVolumeUpdated()
    {
        OnOptionMasterVolumeUpdatedEvent?.Invoke();
    }
    public void PublishOptionAmbientVolumeUpdated()
    {
        OnOptionAmbientVolumeUpdatedEvent?.Invoke();
    }
    public void PublishOptionSoundEffectVolumeUpdated()
    {
        OnOptionSoundEffectVolumeUpdatedEvent?.Invoke();
    }
    public void PublishOptionMusicVolumeUpdated()
    {
        OnOptionMusicVolumeUpdatedEvent?.Invoke();
    }
    #endregion
}
