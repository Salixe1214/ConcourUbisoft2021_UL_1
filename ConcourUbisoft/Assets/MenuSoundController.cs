using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSoundController : MonoBehaviour
{
    [SerializeField] private AudioClip ButtonSound = null;
    [SerializeField] private AudioSource AudioSource = null;

    [SerializeField] private AudioClip MenuSong = null;
    [SerializeField] private AudioSource MenuSongSource = null;

    #region Public Functions
    public void PlayButtonSound()
    {
        AudioSource.PlayOneShot(ButtonSound);
    }
    public void PlayMenuSong()
    {
        MenuSongSource.clip = MenuSong;
        MenuSongSource.Play();
    }
    public void StopMenuSong()
    {
        MenuSongSource.Stop();
    }
    
    #endregion
}
