using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inputs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomMenu : MonoBehaviour
{
    [SerializeField] private GameObject _content = null;
    [SerializeField] private GameObject _roomMenuElementPrefab = null;
    [SerializeField] private GameObject _waitingForAnotherPlayer = null;
    [SerializeField] private GameObject _startButton = null;
    [SerializeField] private Toggle _colorBlindToggle = null;
    [SerializeField] private Text _errorText = null;
    [SerializeField] private GameObject _speaking = null;
    [SerializeField] private GameObject _lobbyPanelCreateButton;
    [SerializeField] private GameObject _lobbyPanelRoomNameInputField;
    [SerializeField] private GameObject _lobbyPanelBackButton;
    [SerializeField] private GameObject _lobbyListHeader;

    private NetworkController _networkController = null;
    private GameController _gameController = null;
    private SoundController _menuSoundController = null;

    #region UI Action

    public void LeaveRoom()
    {
        _menuSoundController.PlayButtonSound();
        _networkController.LeaveRoom();
        Debug.Log("Try to leave room");
        _lobbyPanelCreateButton.SetActive(true);
        _lobbyPanelRoomNameInputField.SetActive(true);
        _lobbyPanelBackButton.SetActive(true);
        _lobbyListHeader.SetActive(true);
    }
    public void StartGame()
    {
        IEnumerable<PlayerNetwork> playerNetworks = GameObject.FindGameObjectsWithTag("PlayerNetwork").Select(x => x.GetComponent<PlayerNetwork>());
        
        if (_gameController.ForceTwoPlayers)
        {
            if (playerNetworks.Count() != 2)
            {
                _errorText.text = "You must have two players to proceed.";
                return;
            }
            
            IEnumerable<PlayerNetwork> playerNetworksDistinctRole = playerNetworks.GroupBy(x => x.PlayerRole).Select(x => x.First());

            if (playerNetworksDistinctRole.Count() != playerNetworks.Count())
            {
                _errorText.text = "You cannot have players with the same role.";
                return;
            } 
        }

        EventSystem.current.SetSelectedGameObject(null);
        _menuSoundController.PlayButtonSound();
        _errorText.text = "";
        _gameController.InitiateStartGame(_networkController.GetLocalRole(), _colorBlindToggle.isOn);
    }
    #endregion
    #region Unity Callbacks
    private void Awake()
    {
        _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        _menuSoundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
    }
    private void OnEnable()
    {
        _speaking.SetActive(true);
        _networkController.OnPlayerObjectCreate += RefreshRoomInterface;
        _networkController.OnJoinedRoomEvent += OnJoinedRoomEvent;
        _networkController.OnPlayerLeftEvent += RefreshRoomInterface;
    }
    private void OnDisable()
    {
        _speaking.SetActive(false);
        _networkController.OnPlayerObjectCreate -= RefreshRoomInterface;
        _networkController.OnJoinedRoomEvent -= OnJoinedRoomEvent;
        _networkController.OnPlayerLeftEvent -= RefreshRoomInterface;
    }
    private void Update()
    {
        _startButton.SetActive(_networkController.IsMasterClient());
        _colorBlindToggle.gameObject.SetActive(_networkController.IsMasterClient());
    }
    #endregion
    #region Event Callbacks

    private void RefreshRoomInterface()
    {
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < _content.transform.childCount; ++i)
        {
            Transform child = _content.transform.GetChild(i);
            if (child.GetComponent<RoomElementController>() == null)
            {
                Destroy(child.gameObject);
            }
            else
            {
                children.Add(child);
            }
        }

        IEnumerable<PlayerNetwork> elements = children.Select(x => x.GetComponent<RoomElementController>().PlayerNetwork);
        IEnumerable<PlayerNetwork> playerNetworksNotFoundInScene = GameObject.FindGameObjectsWithTag("PlayerNetwork").Select(x => x.GetComponent<PlayerNetwork>()).Where(y => elements.Count(z => z == y) == 0).OrderBy(x => !x.IsMasterClient());

        foreach (PlayerNetwork playerNetwork in playerNetworksNotFoundInScene)
        {
            GameObject roomElement = Instantiate(_roomMenuElementPrefab, _content.transform);
            roomElement.GetComponent<RoomElementController>().PlayerNetwork = playerNetwork;
            roomElement.transform.Find("KickButton").GetComponent<Button>().onClick.AddListener(new UnityAction(() => { _menuSoundController.PlayButtonSound(); OnKickClicked(); _networkController.KickPlayer(playerNetwork.Id); }));
        }

        if (_networkController.GetNumberOfPlayer() != 2)
        {
            Instantiate(_waitingForAnotherPlayer, _content.transform);
        }
    }

    private void OnKickClicked()
    {
        if (InputManager.GetController() != Controller.Other)
        {
            EventSystem.current.SetSelectedGameObject(_startButton);
        }
    }

    private void OnJoinedRoomEvent()
    {
        _errorText.text = "";
    }
    #endregion
}
