using System.Collections;
using System.Collections.Generic;
using Other;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoomElementController : MonoBehaviour
{
    public PlayerNetwork PlayerNetwork { get; set; }

    private NetworkController _networkController = null;
    private Text _playerName = null;
    private Button _kickButton = null;
    private RawImage _ownerImage = null;
    private RadioGroup _radioGroup = null;
    private SoundController _menuSoundController = null;

    #region UI Actions
    public void OnRoleModify()
    {
        PlayerNetwork.PlayerRole = (GameController.Role)gameObject.transform.Find("RadioRole").GetComponent<RadioGroup>().Value;
        _menuSoundController.PlayButtonSound();
    }
    #endregion
    #region Unity Callbacks
    private void Awake()
    {
        _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        _menuSoundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        _playerName = gameObject.transform.Find("PlayerName").GetComponent<Text>();
        _kickButton = gameObject.transform.Find("KickButton").GetComponent<Button>();
        _ownerImage = gameObject.transform.Find("OwnerImage").GetComponent<RawImage>();
        _radioGroup = gameObject.transform.Find("RadioRole").GetComponent<RadioGroup>();
    }
    private void Update()
    {
        if (PlayerNetwork != null)
        {
            _playerName.text = PlayerNetwork.Name;

            if (PlayerNetwork.IsMasterClient() || !_networkController.IsMasterClient())
            {
                _kickButton.gameObject.SetActive(false);
            }
            else
            {
                _kickButton.gameObject.SetActive(true);
            }

            if (!PlayerNetwork.IsMasterClient())
            {
                _ownerImage.gameObject.SetActive(false);
            }
            else
            {
                _ownerImage.gameObject.SetActive(true);
            }

            if(PlayerNetwork.IsMine())
            {
                _radioGroup.Interactable = true;
            }
            else
            {
                _radioGroup.Interactable = false;
            }

            _radioGroup.Set((int)PlayerNetwork.PlayerRole, false);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
    #endregion
}
