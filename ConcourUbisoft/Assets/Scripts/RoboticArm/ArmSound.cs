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
    public float StartTime { get; set; } = 0.0f;
    private float smoothSound;
    void Start()
	{
        guardAudio.loop = true;
        guardAudio.Play();
        techAudio.loop = true;
        techAudio.Play();

    }

    private void Update()
    {
        guardAudio.volume = Mathf.Lerp(0,1, Volume)*0.3f;
        techAudio.volume = Mathf.Lerp(0,1, Volume)*0.3f;
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