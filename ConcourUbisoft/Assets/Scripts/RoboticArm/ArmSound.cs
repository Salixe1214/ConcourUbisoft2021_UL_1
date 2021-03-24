using System.Collections;
using System.Collections.Generic;
using Arm;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ArmSound : MonoBehaviour, IPunObservable
{
    [SerializeField] private ArmController armController;
	private AudioSource audioSource;

    public float Volume { get; set; } = 0.0f;
    private float smoothSound;
    void Start()
	{
        audioSource = GetComponent<AudioSource>();
		audioSource.loop = true;
        audioSource.Play();

    }

    private void Update()
    {
        audioSource.volume = Volume*0.3f;
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