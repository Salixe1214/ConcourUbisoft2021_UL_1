using System.Collections;
using System.Collections.Generic;
using Arm;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ArmSound : MonoBehaviour, IPunObservable
{
	[SerializeField] private Transform target;
	[SerializeField] private ArmController armController;
	private Vector3 lastHeadPosition;
	private AudioSource audioSource;

    public float Volume { get; set; } = 0.0f;

    void Start()
	{
		lastHeadPosition = armController.Head.position;

		audioSource = GetComponent<AudioSource>();
		audioSource.loop = true;
        audioSource.Play();

    }

    private void Update()
    {
        audioSource.volume = Volume;
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