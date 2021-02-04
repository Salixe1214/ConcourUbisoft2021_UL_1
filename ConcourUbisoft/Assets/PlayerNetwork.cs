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

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        Name = photonView.Owner.UserId;
    }

    private void Start()
    {
        networkController.InvokePlayerObjectCreate();
    }

    public bool IsMasterClient() {
        return photonView.Owner.IsMasterClient;
    }

    public bool IsMine()
    {
        return photonView.IsMine;
    }

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
}
