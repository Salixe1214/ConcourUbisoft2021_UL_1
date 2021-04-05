using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSuccesFailSounds : MonoBehaviour
{
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip failSound;
    [SerializeField] private DoorController door;
    private AudioSource _audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        door.OnSuccess.AddListener(OnSuccess);
    }

    public void OnSuccess()
    {
        _audioSource.clip = successSound;
        _audioSource.Play();
    }

    public void OnFail()
    {
        _audioSource.clip = failSound;
        _audioSource.Play();
    }
}
