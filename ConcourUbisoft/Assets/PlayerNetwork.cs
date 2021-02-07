using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetwork : MonoBehaviourPun, IPunObservable
{
    public enum Role {
        A,
        B
    }

    public Role PlayerRole { get; set; }
    public string Name { set; get; }
    public string Id { get { return photonView.Owner.UserId; } }

    private PhotonView photonView = null;
    private NetworkController networkController = null;

    #region Unity Callbacks
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        Name = $"Player {(photonView.Owner.IsMasterClient ? "1" : "2")}";
    }
    private void Start()
    {
        networkController.InvokePlayerNetworkInstantiate();
    }
    #endregion
    #region Photon Callbacks
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext((int)PlayerRole);
            stream.SendNext(Name);
        }
        else
        {
            this.PlayerRole = (Role)(int)stream.ReceiveNext();
            this.Name = (string)stream.ReceiveNext();
        }
    }
    #endregion
    #region Public Functions
    public bool IsMasterClient()
    {
        return photonView.Owner.IsMasterClient;
    }
    public bool IsMine()
    {
        return photonView.IsMine;
    }
    #endregion
}
