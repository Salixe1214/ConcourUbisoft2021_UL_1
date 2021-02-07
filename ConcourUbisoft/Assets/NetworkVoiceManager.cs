/*
 * Source: https://github.com/silidragos/TCG/blob/master/Assets/Scripts/Networking/NetworkVoiceManager.cs
 */

using Photon.Pun;
using Photon.Voice.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(VoiceConnection))]
public class NetworkVoiceManager : MonoBehaviour
{
    public Transform RemoteVoiceParent;

    private VoiceConnection voiceConnection;
    
    void Awake()
    {
        voiceConnection = GetComponent<VoiceConnection>();
    }

    private void OnEnable()
    {
        voiceConnection.SpeakerLinked += this.OnSpeakerCreated;
    }
    
    private void OnDisable()
    {
        voiceConnection.SpeakerLinked -= this.OnSpeakerCreated;
    }

    private void OnSpeakerCreated(Speaker pSpeaker)
    {
        pSpeaker.transform.SetParent(RemoteVoiceParent);
        pSpeaker.OnRemoteVoiceRemoveAction += OnRemoteVoiceRemove;
    }

    private void OnRemoteVoiceRemove(Speaker pSpeaker)
    {
        if(pSpeaker != null)
            Destroy(pSpeaker.gameObject);
    }
}
