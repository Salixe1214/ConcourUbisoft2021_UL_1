using System;
using Arm;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(MagnetController))]
[RequireComponent(typeof(AudioSource))]
public class MagnetSound : MonoBehaviour, IPunObservable
{
    [SerializeField] private float volumeMultiplier = 0.3f;

    private MagnetController magnetController;
    private AudioSource audioSource;

    public bool IsOn { get; set; } = false;

    private void Start()
    {
        magnetController = GetComponent<MagnetController>();
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    private void Update()
    {
        if (IsOn && !magnetController.Grabbed)
        {
            audioSource.volume = 1 * volumeMultiplier;
        }
        else
        {
            audioSource.volume = 0;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(IsOn);
        }
        else
        {
            IsOn = (bool)stream.ReceiveNext();
        }
    }
}