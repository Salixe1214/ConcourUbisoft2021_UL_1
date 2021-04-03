using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySpeaking : MonoBehaviour
{
    [SerializeField] private GameObject _playerOneSound = null;
    [SerializeField] private GameObject _playerTwoSound = null;
    [SerializeField] private RawImage _playerOneImage = null;
    [SerializeField] private RawImage _playerTwoImage = null;
    [SerializeField] private Sprite _guardSprite = null;
    [SerializeField] private Sprite _techSprite = null;


    private NetworkController _networkController = null;
    private PhotonView _photonView = null;

    private void Awake()
    {
        _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        _photonView = GetComponent<PhotonView>();
    }

    private void OnEnable()
    {
        _networkController.OnPlayerJoin += OnPlayerJoin;
        _playerOneSound.SetActive(false);
        _playerTwoSound.SetActive(false);
    }

    private void OnDisable()
    {
        _networkController.OnPlayerJoin -= OnPlayerJoin;
    }

    private void OnPlayerJoin()
    {
        _playerOneSound.SetActive(false);
        _playerTwoSound.SetActive(false);
    }

    private void OnDestroy()
    {
        
    }

    private void Update()
    {
        if(PhotonNetwork.CurrentRoom != null)
        {
            _playerTwoImage.gameObject.SetActive(_networkController.GetNumberOfPlayer() == 2);
        }

        if(PhotonNetwork.IsMasterClient)
        {
            _playerOneImage.texture = _networkController.GetLocalRole() == GameController.Role.SecurityGuard ? _guardSprite.texture : _techSprite.texture;
            _playerTwoImage.texture = _networkController.GetDistantRole() == GameController.Role.SecurityGuard ? _guardSprite.texture : _techSprite.texture;
        }
        else
        {
            _playerOneImage.texture = _networkController.GetDistantRole() == GameController.Role.SecurityGuard ? _guardSprite.texture : _techSprite.texture;
            _playerTwoImage.texture = _networkController.GetLocalRole() == GameController.Role.SecurityGuard ? _guardSprite.texture : _techSprite.texture;
        }

        string[] joysticks = Input.GetJoystickNames();

        if (joysticks.Contains("Controller (Xbox One For Windows)"))
        {
            if (Input.GetAxis("VoiceXBO") > 0.2)
            {
                object[] parameters = new object[] { true, PhotonNetwork.IsMasterClient };
                _photonView.RPC("SetIsSpeaking", RpcTarget.All, parameters as object);
            }
            else
            {
                object[] parameters = new object[] { false, PhotonNetwork.IsMasterClient };
                _photonView.RPC("SetIsSpeaking", RpcTarget.All, parameters as object);
            }
        }
        else if (joysticks.Contains("Wireless Controller"))
        {
            if (Input.GetAxis("VoicePS") > -0.9)
            {
                object[] parameters = new object[] { true, PhotonNetwork.IsMasterClient };
                _photonView.RPC("SetIsSpeaking", RpcTarget.All, parameters as object);
            }
            else
            {
                object[] parameters = new object[] {false, PhotonNetwork.IsMasterClient };
                _photonView.RPC("SetIsSpeaking", RpcTarget.All, parameters as object);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            object[] parameters = new object[] { true, PhotonNetwork.IsMasterClient };
            _photonView.RPC("SetIsSpeaking", RpcTarget.All, parameters as object);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            object[] parameters = new object[] { false, PhotonNetwork.IsMasterClient };
            _photonView.RPC("SetIsSpeaking", RpcTarget.All, parameters as object);
        }
    }

    [PunRPC]
    private void SetIsSpeaking(object[] paramaters)
    {
        bool value = (bool)paramaters[0];
        bool isMasterClientCaller = (bool)paramaters[1];

        if(isMasterClientCaller)
        {
            _playerOneSound.SetActive(value);
        }
        else
        {
            _playerTwoSound.SetActive(value);
        }
    }
}
