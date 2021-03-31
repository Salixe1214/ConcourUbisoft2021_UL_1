using Photon.Voice.Unity;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity.UtilityScripts;

[RequireComponent(typeof(VoiceConnection))]
public class NetworkVoiceManager : MonoBehaviour
{
    private Recorder recorder;
    private ConnectAndJoin cAJ;
    private VoiceConnection vc;
    private NetworkController _networkController;
    
    private void Start()
    {
        vc = gameObject.GetComponent<VoiceConnection>();
        recorder = GetComponent<Recorder>();
        recorder.IsRecording = false;
        recorder.Init(vc);

        
    }

    private void Update()
    {
        string[] joysticks = Input.GetJoystickNames();

        if (joysticks.Contains("Controller (Xbox One For Windows)"))
        {
            if (Input.GetAxis("VoiceXBO") >0.2)
            {
                recorder.IsRecording = true;
            }
            else
            {
                recorder.IsRecording = false;
            }
        }
        else if (joysticks.Contains("Wireless Controller"))
        {
            if (Input.GetAxis("VoicePS") > -0.9)
            {
                recorder.IsRecording = true;
            }
            else
            {
                recorder.IsRecording = false;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            recorder.IsRecording = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            recorder.IsRecording = false;
        }
    }
}
