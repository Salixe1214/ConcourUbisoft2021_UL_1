using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(randomCodePicker), typeof(Animation))]
public class DoorController : MonoBehaviour
{
    public enum Direction
    {
        Left,
        Right,
        Bottom,
        Up
    }

    [SerializeField] public int Id = 0;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;
    private List<Direction> _inputSequences;
    private List<Direction> _currentSequences = new List<Direction>();
    private Animation _animation = null;
    private PhotonView _photonView = null;

    public UnityEvent OnError;
    public UnityEvent OnSuccess;
    public UnityEvent OnEntry;

    public bool IsUnlock { get; private set; } = false;

    private void Awake()
    {
        _animation = GetComponent<Animation>();
        _photonView = GetComponent<PhotonView>();
        _inputSequences = GetComponent<randomCodePicker>().GetSequence();
    }

    public void TriggerLeft()
    {
        _photonView.RPC("TiggerLeftNetwork", RpcTarget.AllBuffered);
    }

    public void TriggerRight()
    {
        _photonView.RPC("TriggerRightNetwork", RpcTarget.AllBuffered);
    }

    public void TriggerBottom()
    {
        _photonView.RPC("TriggerBottomNetwork", RpcTarget.AllBuffered);
    }

    public void TriggerUp()
    {
        _photonView.RPC("TriggerUpNetwork", RpcTarget.AllBuffered);
    }

    public void CheckSequence()
    {
        _photonView.RPC("CheckSequenceNetwork", RpcTarget.AllBuffered);
    }

    public void Unlock()
    {
        Debug.Log("Play");
        IsUnlock = true;
        _animation.Play();
        audioSource.clip = audioClip;
        audioSource.volume = 2;
        audioSource.Play();
        OnSuccess?.Invoke();
    }

    [PunRPC]
    public void TiggerLeftNetwork()
    {
        _currentSequences.Add(Direction.Left);
        OnEntry.Invoke();
    }

    [PunRPC]
    public void TriggerRightNetwork()
    {
        _currentSequences.Add(Direction.Right);
        OnEntry.Invoke();
    }

    [PunRPC]
    public void TriggerBottomNetwork()
    {
        _currentSequences.Add(Direction.Bottom);
        OnEntry.Invoke();
    }

    [PunRPC]
    public void TriggerUpNetwork()
    {
        _currentSequences.Add(Direction.Up);
        OnEntry.Invoke();
    }

    [PunRPC]
    public void CheckSequenceNetwork()
    {
        if (!IsUnlock)
        {
            bool error = false;
            if (_inputSequences.Count == _currentSequences.Count)
            {
                for (int i = 0; i < _inputSequences.Count; ++i)
                {
                    Debug.Log(_currentSequences[i]);
                    if (_inputSequences[i] != _currentSequences[i])
                    {
                        error = true;
                        break;
                    }
                }
            }
            else
            {
                error = true;
            }

            if (error)
            {
                OnError?.Invoke();
            }
            else
            {
                Unlock();
            }
        }

        _currentSequences.Clear();
    }
}
