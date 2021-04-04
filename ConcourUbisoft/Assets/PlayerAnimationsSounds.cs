using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationsSounds : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _walkClip;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Step(float pVolumeModif)
    {
        _audioSource.clip = _walkClip;
        _audioSource.volume = 0.5f;
        _audioSource.Play();
    }
}
