using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoomElementController : MonoBehaviour
{
    public PlayerNetwork PlayerNetwork { get; set; }

    private NetworkController networkController = null;
    private Text PlayerName = null;
    private Button KickButton = null;
    private RawImage OwnerImage = null;
    private Dropdown DropdownRole = null;

    #region UI Actions
    public void OnRoleModify()
    {
        PlayerNetwork.PlayerRole = (PlayerNetwork.Role)gameObject.transform.Find("DropdownRole").GetComponent<Dropdown>().value;
    }
    #endregion
    #region Unity Callbacks
    private void Awake()
    {
        networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        PlayerName = gameObject.transform.Find("PlayerName").GetComponent<Text>();
        KickButton = gameObject.transform.Find("KickButton").GetComponent<Button>();
        OwnerImage = gameObject.transform.Find("OwnerImage").GetComponent<RawImage>();
        DropdownRole = gameObject.transform.Find("DropdownRole").GetComponent<Dropdown>();
    }
    private void Update()
    {
        if (PlayerNetwork != null)
        {
            PlayerName.text = PlayerNetwork.Name;

            if (PlayerNetwork.IsMasterClient() || !networkController.IsMasterClient())
            {
                KickButton.gameObject.SetActive(false);
            }
            else
            {
                KickButton.gameObject.SetActive(true);
            }

            if (!PlayerNetwork.IsMasterClient())
            {
                OwnerImage.gameObject.SetActive(false);
            }
            else
            {
                OwnerImage.gameObject.SetActive(true);
            }

            if(PlayerNetwork.IsMine())
            {
                DropdownRole.GetComponent<Dropdown>().interactable = true;
            }
            else
            {
                DropdownRole.GetComponent<Dropdown>().interactable = false;
            }

            DropdownRole.GetComponent<Dropdown>().value = (int)PlayerNetwork.PlayerRole;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
    #endregion
}
