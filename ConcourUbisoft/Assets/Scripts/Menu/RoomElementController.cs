using System.Collections;
using System.Collections.Generic;
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
    private Dropdown _dropdownRole = null;
    private SoundController _menuSoundController = null;

    #region UI Actions
    public void OnRoleModify()
    {
        PlayerNetwork.PlayerRole = (GameController.Role)gameObject.transform.Find("DropdownRole").GetComponent<Dropdown>().value;
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
        _dropdownRole = gameObject.transform.Find("DropdownRole").GetComponent<Dropdown>();
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
                _dropdownRole.GetComponent<Dropdown>().interactable = true;
            }
            else
            {
                _dropdownRole.GetComponent<Dropdown>().interactable = false;
            }

            _dropdownRole.GetComponent<Dropdown>().value = (int)PlayerNetwork.PlayerRole;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
    #endregion
}
