using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Arm;
using Other;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class FurnaceController : MonoBehaviour
{
    [Serializable]
    public class SequenceOfColor
    {
        public Color[] ColorsSequence = null;
        public int SucceedColors = 0;
        public Other.PickableType[] types = null;
    }

    [SerializeField] private SequenceOfColor[] SequencesOfColor = null;
    [SerializeField] private Other.PickableType[] SequencesOfTransportableTypes = null;
    [SerializeField] private int nbColorSequences = 5;
    [SerializeField] private int minColorSequencelenght=3;
    [SerializeField] private int maxColorSequenceLenght=7;
    [SerializeField] private float TimeToConsume = 0.0f;
    [SerializeField] private GameController.Role _owner = GameController.Role.None;

    public UnityEvent WhenFurnaceConsumedAll;
    public UnityEvent WhenFurnaceConsumeWrong;
    public UnityEvent WhenFurnaceConsumeAWholeSequenceWithoutFinishing;
    public event Action CheckItemOffList;

    private SoundController soundController;
    private PhotonView _photonView = null;
    private NetworkController _networkController = null;

    private int SucceedSequences = 0;

    System.Random _random = new System.Random(0);

    private void Awake()
    {
        SequencesOfColor = new SequenceOfColor[nbColorSequences];
        soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        _networkController = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<NetworkController>();
        _photonView = GetComponent<PhotonView>();
    }


    private void OnTriggerEnter(Collider other)
    {
        Pickable pickable = null;
        if (other.gameObject.TryGetComponent(out pickable) && _owner == _networkController.GetLocalRole())
        {
            Consume(pickable);
        }
    }

    private void Consume(Pickable pickable)
    {
        object[] parameters = new object[] { (int)pickable.GetType(), pickable.Color.r, pickable.Color.g, pickable.Color.b, pickable.Color.a, };
        _photonView.RPC("Consumed", RpcTarget.All, parameters as object);

        PhotonNetwork.Destroy(pickable.gameObject);
    }

    [PunRPC]
    public void Consumed(object[] parameters)
    {
        ValidateConsumed((PickableType)parameters[0], new Color((float)parameters[1], (float)parameters[2], (float)parameters[3], (float)parameters[4]));
    }

    private void ValidateConsumed(PickableType type, Color color)
    {
        Debug.Log("Consume");
        SequenceOfColor currentSequence = SequencesOfColor[SucceedSequences];

        Color currentSequenceColor = currentSequence.ColorsSequence[currentSequence.SucceedColors];
        PickableType currentType = currentSequence.types[currentSequence.SucceedColors];

        if (currentSequenceColor.r == (color).r && currentSequenceColor.g == (color).g && currentSequenceColor.b == (color).b && currentType == type)
        {
            soundController.PlayLevelPartialSequenceSuccessSound();
            CheckItemOffList?.Invoke();
            currentSequence.SucceedColors++;
            if (currentSequence.SucceedColors == currentSequence.ColorsSequence.Length)
            {
                SucceedSequences++;
                if (SucceedSequences == SequencesOfColor.Length)
                {
                    WhenFurnaceConsumedAll?.Invoke();
                }
                else
                {
                    WhenFurnaceConsumeAWholeSequenceWithoutFinishing?.Invoke();
                }
            }
        }
        else
        {
            WhenFurnaceConsumeWrong?.Invoke();
        }
    }

    public void GenerateNewColorSequences(Color[] allColors)
    {
        int currentSequenceLenght = minColorSequencelenght;
        for (int i = 0; i < nbColorSequences; i++)
        {
            SequenceOfColor sc = new SequenceOfColor();
            sc.ColorsSequence = new Color[currentSequenceLenght];
            sc.types = new Other.PickableType[currentSequenceLenght];
            for (int j = 0; j < currentSequenceLenght; j++)
            {
                int nextType = _random.Next(0, 5);
                int nextColor = _random.Next(0, allColors.Length);
                sc.ColorsSequence[j] = allColors[nextColor];
                sc.types[j] = (Other.PickableType)nextType;
            }
            SequencesOfColor[i] = sc;
            if (currentSequenceLenght < maxColorSequenceLenght)
            {
                currentSequenceLenght += 1;
            }
        }
    }

    public int GetCurrentSequenceLenght()
    {
        return SequencesOfColor[SucceedSequences].ColorsSequence.Length;
    }

    public SequenceOfColor GetCurrentSequence()
    {
        return SequencesOfColor[SucceedSequences];
    }

    public Color GetNextColor()
    {
        SequenceOfColor currentSequence = GetCurrentSequence();
        return currentSequence.ColorsSequence[currentSequence.SucceedColors];
    }

    public Other.PickableType GetNextItemType()
    {
        SequenceOfColor currentSequence = GetCurrentSequence();
        return currentSequence.types[currentSequence.SucceedColors];
    }

    public PickableType[] GetAllNextItemTypes()
    {
        return GetCurrentSequence().types;
    }

    public int GetCurrentSequenceIndex()
    {
        return GetCurrentSequence().SucceedColors;
    }

    public SequenceOfColor[] GetAllSequences()
    {
        return SequencesOfColor;
    }

    public Color[] GetAllNextItemColors()
    {
        return GetCurrentSequence().ColorsSequence;
    }

    public int GetItemCount()
    {
        int total = 0;
        foreach (var sequence in SequencesOfColor)
        {
            total += sequence.ColorsSequence.Length;
        }

        return total;
    }
}
