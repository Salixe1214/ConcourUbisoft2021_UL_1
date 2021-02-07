using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoomMenu : MonoBehaviour
{
    [SerializeField] private GameObject Content = null;
    [SerializeField] private GameObject RoomMenuElementPrefab = null;
    [SerializeField] private GameObject StartButton = null;
    [SerializeField] private GameObject ErrorText = null;

    private NetworkController networkController = null;
    private Button startButton = null;
    private Text errorText = null;

    #region UI Action
    public void LeaveRoom()
    {
        networkController.LeaveRoom();
    }
    public void StartGame()
    {
        IEnumerable<PlayerNetwork> playerNetworks = GameObject.FindGameObjectsWithTag("Player").Select(x => x.GetComponent<PlayerNetwork>());

        if (playerNetworks.Count() != 2)
        {
            errorText.text = "You must have two players to proceed.";
            return;
        }

        IEnumerable<PlayerNetwork> playerNetworksDistinctRole = playerNetworks.GroupBy(x => x.PlayerRole).Select(x => x.First());
        if (playerNetworksDistinctRole.Count() != playerNetworks.Count())
        {
            errorText.text = "You cannot have players with the same role.";
            return;
        }

        errorText.text = "";
    }
    #endregion
    #region Unity Callbacks
    private void Awake()
    {
        networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        startButton = StartButton.GetComponent<Button>();
        errorText = ErrorText.GetComponent<Text>();
    }
    private void OnEnable()
    {
        networkController.OnPlayerObjectCreate += RefreshRoomInterface;
        networkController.OnJoinedRoomEvent += OnJoinedRoomEvent;
    }
    private void OnDisable()
    {
        networkController.OnPlayerObjectCreate -= RefreshRoomInterface;
        networkController.OnJoinedRoomEvent -= OnJoinedRoomEvent;
    }
    private void Update()
    {
        if (networkController.IsMasterClient())
        {
            startButton.interactable = true;
        }
        else
        {
            startButton.interactable = false;
        }
    }
    #endregion
    #region Event Callbacks
    private void RefreshRoomInterface()
    {
        List<Transform> children = new List<Transform>();
        for(int i = 0; i < Content.transform.childCount; ++i)
        {
            children.Add(Content.transform.GetChild(i));
        }

        IEnumerable<PlayerNetwork> elements = children.Select(x => x.GetComponent<RoomElementController>().PlayerNetwork );
        IEnumerable<PlayerNetwork> playerNetworksNotFoundInScene = GameObject.FindGameObjectsWithTag("Player").Select(x => x.GetComponent<PlayerNetwork>()).Where(y => elements.Count(z => z == y) == 0).OrderBy(x => !x.IsMasterClient());

        foreach (PlayerNetwork playerNetwork in playerNetworksNotFoundInScene)
        {
            GameObject roomElement = Instantiate(RoomMenuElementPrefab, Content.transform);
            roomElement.GetComponent<RoomElementController>().PlayerNetwork = playerNetwork;
            roomElement.transform.Find("KickButton").GetComponent<Button>().onClick.AddListener(new UnityAction(() => networkController.KickPlayer(playerNetwork.Id)));
        }
    }
    private void OnJoinedRoomEvent()
    {
        errorText.text = "";
    }
    #endregion
}
