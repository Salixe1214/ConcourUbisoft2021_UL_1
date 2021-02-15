using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioClip ButtonSound = null;
    [SerializeField] private AudioSource AudioSource = null;

    [SerializeField] private AudioClip MenuSong = null;
    [SerializeField] private AudioSource MenuSongSource = null;

    [SerializeField] private AudioClip AmbientSound = null;
    [SerializeField] private AudioSource AmbientSource = null;


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
