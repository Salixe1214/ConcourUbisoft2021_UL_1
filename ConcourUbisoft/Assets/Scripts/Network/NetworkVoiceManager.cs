using Photon.Voice.Unity;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(VoiceConnection))]
public class NetworkVoiceManager : MonoBehaviour
{
    private Recorder recorder;
    
    private void Awake()
    {
        recorder = GetComponent<Recorder>();
        recorder.IsRecording = false;
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
