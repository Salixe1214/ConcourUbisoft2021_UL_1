using System;
using System.Collections;
using System.Collections.Generic;
using Other;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TimerController : MonoBehaviour, IPunObservable
{
    // Start is called before the first frame update
    
    [SerializeField] private AudioClip TimeLeftSound;
    [SerializeField] private Text TimeTextField;
    [SerializeField] private Text TimeTextFieldGuard;
    [SerializeField] private Text BonusTimeTextField;
    [SerializeField] private Text BonusTimeTextFieldGuard;
    [SerializeField] private Text WarningTextField;
    [SerializeField] private GameObject LevelControl;
    [SerializeField] private GameObject TimeImageObject;
    [SerializeField] private GameObject TimeImageObjectGuard;
    [SerializeField] private float BonusTimeShownDuration;
    [SerializeField] private float WarningDuration;
    
    private float _time;
    private LevelController _levelController;
    private float _timeLeft;
    private Color _timeTextColor;
    private AudioSource _timerAudioSource;
    private PhotonView _photonView;
    private NetworkController _networkController;

    private void Awake()
    {
        _levelController = LevelControl.GetComponent<LevelController>();
        _photonView = GetComponent<PhotonView>();
        _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
    }

    void Start()
    {
        if (_networkController.GetLocalRole() == GameController.Role.Technician && !_photonView.IsMine)
        {
            _photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        }

        if (_networkController.GetLocalRole() == GameController.Role.Technician)
        {
            BonusTimeTextField.text = "";
            TimeTextField.text = "";
        }
        else
        {
            BonusTimeTextFieldGuard.text = "";
            TimeTextFieldGuard.text = "";
        }

        _timeTextColor = TimeTextField.color;
        _timerAudioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _levelController.OnTimeChanged += SetTime;
        _levelController.OnBonusTime += ShowBonusTime;
        _levelController.OnWarning += ShowWarning;
    }

    private void OnDisable()
    {
        _levelController.OnTimeChanged -= SetTime;
        _levelController.OnBonusTime -= ShowBonusTime;
        _levelController.OnWarning -= ShowWarning;
    }

    private void SetTime(float timeValue)
    {
        if(_photonView.IsMine)
        {
            if (!TimeImageObject.activeSelf)
            {
                _photonView.RPC("TimerImageChange", RpcTarget.All, new object[]{ true } as object);
                //TimeImageObject.SetActive(true);
            }
            TimeTextField.text = timeValue.ToString("G");
        }
    }

    private void ShowBonusTime(float bonusTime)
    {
        if (_photonView.IsMine)
        {
            BonusTimeTextField.text = "+ " + bonusTime.ToString("G");
            StartCoroutine(StartBonusTimeSequence());
        }
    }

    private void ShowWarning(float remainingTime)
    {
        if(_photonView.IsMine)
        {
            WarningTextField.text = remainingTime.ToString("G") + " Seconds Before Sequence Failure";
            _timerAudioSource.Stop();
            _timerAudioSource.clip = TimeLeftSound;
            _timerAudioSource.time = 0;
            StartCoroutine(StartWarning());
        }
    }

    private void EndTimer()
    {
        if (_photonView.IsMine)
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator StartWarning()
    {
        WarningTextField.gameObject.SetActive(true);
        _timerAudioSource.Play();
        yield return new WaitForSeconds(WarningDuration);
        WarningTextField.gameObject.SetActive(false);
    }

    IEnumerator StartBonusTimeSequence()
    {
        //TimeTextField.color = BonusTimeTextField.color;
        _photonView.RPC("ChangeColor", RpcTarget.All, new object[] { BonusTimeTextField.color.r, BonusTimeTextField.color.g, BonusTimeTextField.color.b } as object);
        yield return new WaitForSeconds(BonusTimeShownDuration);
        BonusTimeTextField.text ="";
        //TimeTextField.color = _timeTextColor;
        _photonView.RPC("ChangeColor", RpcTarget.All, new object[] { _timeTextColor.r, _timeTextColor.g, _timeTextColor.b } as object);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(TimeTextField.text);
            stream.SendNext(BonusTimeTextField.text);
        }
        else
        {
            TimeTextFieldGuard.text = (string)stream.ReceiveNext();
            BonusTimeTextFieldGuard.text = (string)stream.ReceiveNext();
        }
    }

    [PunRPC]
    private void TimerImageChange(object[] parameters)
    {
        if(_networkController.GetLocalRole() == GameController.Role.Technician)
        {
            TimeImageObject.SetActive((bool)parameters[0]);
        }
        else
        {
            TimeImageObjectGuard.SetActive((bool)parameters[0]);
        }
    }

    [PunRPC]
    private void ChangeColor(object[] parameters)
    {
        if (_networkController.GetLocalRole() == GameController.Role.Technician)
        {
            TimeTextField.color = new Color((float)parameters[0], (float)parameters[1], (float)parameters[2]);
        }
        else
        {
            TimeTextFieldGuard.color = new Color((float)parameters[0], (float)parameters[1], (float)parameters[2]);
        }
    }
    //public float RemaningTime { get { float timeElapsed = Time.time - startTime;  return timeElapsed > gameDuration ? 0 : gameDuration - timeElapsed; } }

}
