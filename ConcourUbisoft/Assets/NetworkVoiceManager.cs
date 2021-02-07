using Photon.Pun;
using Photon.Voice.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Photon.Voice.Unity.UtilityScripts;
using UnityEngine;

[RequireComponent(typeof(VoiceConnection))]
public class NetworkVoiceManager : MonoBehaviour
{
    private Recorder recorder;
    private ConnectAndJoin conAndJoin;
    private bool l = true;
    
    private void Awake()
    {
        recorder = GetComponent<Recorder>();
        recorder.IsRecording = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            recorder.IsRecording = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            recorder.IsRecording = false;
        }
    }
}
