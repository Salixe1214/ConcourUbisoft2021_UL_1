using System.Collections;
using System.Collections.Generic;
using Arm;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ArmSound : MonoBehaviour, IPunObservable
{
    [SerializeField] private ArmController armController;
    [SerializeField] private AudioSource guardAudio;
    [SerializeField] private AudioSource techAudio;

    public float Volume { get; set; } = 0.0f;
    private float smoothSound;
    void Start()
	{
        guardAudio = GetComponent<AudioSource>();
        guardAudio.loop = true;
        guardAudio.Play();
        techAudio = GetComponent<AudioSource>();
        techAudio.loop = true;
        techAudio.Play();

    }

    private void Update()
    {
        guardAudio.volume = Volume*0.3f;
        techAudio.volume = Volume*0.3f;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(Volume);
        }
        else
        {
            Volume = (float)stream.ReceiveNext();
        }
    }
}