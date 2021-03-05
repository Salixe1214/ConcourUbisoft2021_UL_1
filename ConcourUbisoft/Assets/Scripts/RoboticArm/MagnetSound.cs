using System;
using Arm;
using UnityEngine;

[RequireComponent(typeof(MagnetController))]
[RequireComponent(typeof(AudioSource))]
public class MagnetSound : MonoBehaviour
{
    [SerializeField] private float volumeMultiplier = 0.3f;
    private MagnetController magnetController;
    private AudioSource audioSource;

    private void Start()
    {
        magnetController = GetComponent<MagnetController>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (magnetController.MagnetActive)
        {
            audioSource.volume = 1 * volumeMultiplier;
        }
        else
        {
            audioSource.volume = 0;
        }
    }
}