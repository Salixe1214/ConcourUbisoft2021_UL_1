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

    public void Step()
    {
        _audioSource.clip = _walkClip;
        _audioSource.Play();
    }
}
