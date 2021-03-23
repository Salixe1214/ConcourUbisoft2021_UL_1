using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoomMenu : MonoBehaviour
{
    [SerializeField] private GameObject _content = null;
    [SerializeField] private GameObject _roomMenuElementPrefab = null;
    [SerializeField] private GameObject _waitingForAnotherPlayer = null;
    [SerializeField] private Button _startButton = null;
    [SerializeField] private Text _errorText = null;
    [SerializeField] private GameObject _createButton;
    [SerializeField] private GameObject _directButton;

    private NetworkController _networkController = null;
    private GameController _gameController = null;
    private SoundController _menuSoundController = null;

    #region UI Action
    public void LeaveRoom()
    {
        _menuSoundController.PlayButtonSound();
        _networkController.LeaveRoom();
        Debug.Log("Try to leave room");
        //_directButton.SetActive(true);
        //_createButton.SetActive(true);
    }
    public void StartGame()
    {
        _menuSoundController.PlayButtonSound();
        IEnumerable<PlayerNetwork> playerNetworks = GameObject.FindGameObjectsWithTag("Player").Select(x => x.GetComponent<PlayerNetwork>());

        //if (playerNetworks.Count() != 2)
        //{
        //    errorText.text = "You must have two players to proceed.";
        //    return;
        //}

        //IEnumerable<PlayerNetwork> playerNetworksDistinctRole = playerNetworks.GroupBy(x => x.PlayerRole).Select(x => x.First());
        //if (playerNetworksDistinctRole.Count() != playerNetworks.Count())
        //{
        //    errorText.text = "You cannot have players with the same role.";
        //    return;
        //}

        _errorText.text = "";

        _gameController.StartGame(_networkController.GetLocalRole());
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
        _networkController.OnPlayerObjectCreate += RefreshRoomInterface;
        _networkController.OnJoinedRoomEvent += OnJoinedRoomEvent;
        _networkController.OnPlayerLeftEvent += RefreshRoomInterface;
    }
    private void OnDisable()
    {
        _networkController.OnPlayerObjectCreate -= RefreshRoomInterface;
        _networkController.OnJoinedRoomEvent -= OnJoinedRoomEvent;
        _networkController.OnPlayerLeftEvent -= RefreshRoomInterface;
    }
    private void Update()
    {
        if (_networkController.IsMasterClient())
        {
            _startButton.interactable = true;
        }
        else
        {
            _startButton.interactable = false;
        }
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
            roomElement.transform.Find("KickButton").GetComponent<Button>().onClick.AddListener(new UnityAction(() => { _menuSoundController.PlayButtonSound(); _networkController.KickPlayer(playerNetwork.Id); }));
        }

        if (_networkController.GetNumberOfPlayer() != 2)
        {
            Instantiate(_waitingForAnotherPlayer, _content.transform);
        }
    }
    private void OnJoinedRoomEvent()
    {
        _errorText.text = "";
    }
    #endregion
}
