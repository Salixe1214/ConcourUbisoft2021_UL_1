using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionController : MonoBehaviour
{
    [SerializeField] private Slider masterVolume = null;
    [SerializeField] private Slider ambientVolume = null;
    [SerializeField] private Slider musicVolume = null;
    [SerializeField] private Slider soundEffectVolume = null;

    public Slider MasterVolume { get => masterVolume; }
    public Slider AmbientVolume { get => ambientVolume; }
    public Slider MusicVolume { get => musicVolume; }
    public Slider SoundEffectVolume { get => soundEffectVolume; }

    #region Events 
    public delegate void OnOptionMasterVolumeUpdatedHandler();
    public event OnOptionMasterVolumeUpdatedHandler OnOptionMasterVolumeUpdatedEvent;

    public delegate void OnOptionAmbientVolumeUpdatedHandler();
    public event OnOptionAmbientVolumeUpdatedHandler OnOptionAmbientVolumeUpdatedEvent;

    public delegate void OnOptionSoundEffectVolumeUpdatedHandler();
    public event OnOptionSoundEffectVolumeUpdatedHandler OnOptionSoundEffectVolumeUpdatedEvent;

    public delegate void OnOptionMusicVolumeUpdatedHandler();
    public event OnOptionMusicVolumeUpdatedHandler OnOptionMusicVolumeUpdatedEvent;
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
