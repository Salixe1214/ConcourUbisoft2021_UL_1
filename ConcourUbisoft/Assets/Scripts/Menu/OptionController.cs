using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionController : MonoBehaviour
{
    public enum SoundChannel
    {
        Master,
        Ambient,
        Music,
        SoundEffect,
        VoiceChat
    }

    [SerializeField] private Slider _masterVolume = null;
    [SerializeField] private Slider _ambientVolume = null;
    [SerializeField] private Slider _musicVolume = null;
    [SerializeField] private Slider _soundEffectVolume = null;
    [SerializeField] private Slider _voiceChatVolume = null;

    private Dictionary<SoundChannel, Slider> _sliders = null;

    #region Unity Callbacks
    private void Awake()
    {
        _sliders = new Dictionary<SoundChannel, Slider>() {
            { SoundChannel.Master, _masterVolume },
            { SoundChannel.Ambient, _ambientVolume },
            { SoundChannel.Music, _musicVolume },
            { SoundChannel.SoundEffect, _soundEffectVolume },
            { SoundChannel.VoiceChat, _voiceChatVolume },
        };
    }
    #endregion

    #region Events 
    public event Action<SoundChannel> OnOptionVolumeUpdatedEvent;
    #endregion
    #region Public Functions
    public float GetVolume(SoundChannel channel)
    {
        return _sliders[channel].value;
    }

    public void PublishVolumeChanged(int channel)
    {
        OnOptionVolumeUpdatedEvent?.Invoke((SoundChannel)channel);
    }
    #endregion
}
