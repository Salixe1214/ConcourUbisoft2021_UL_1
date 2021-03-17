using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Other;
using UnityEngine;

public class FurnaceController : MonoBehaviour
{
    [Serializable]
    public class SequenceOfColor
    {
        public Color[] ColorsSequence = null;
        public int SucceedColors = 0;
        public TransportableType[] types = null;
    }

    [SerializeField] private SequenceOfColor[] SequencesOfColor = null;
    [SerializeField] private TransportableType[] SequencesOfTransportableTypes = null;
    [SerializeField] private int nbColorSequences = 5;
    [SerializeField] private int minColorSequencelenght=3;
    [SerializeField] private int maxColorSequenceLenght=7;
    [SerializeField] private float TimeToConsume = 0.0f;
    [SerializeField] private bool HasBeenPickupNeeded = true;

    private SoundController soundController;

    public event Action WhenFurnaceConsumedAll;
    public event Action WhenFurnaceConsumeWrong;
    public event Action WhenFurnaceConsumeAWholeSequenceWithoutFinishing;

    public event Action CheckItemOffList;

    private int SucceedSequences = 0;

    System.Random _random = new System.Random(0);

    private void Awake()
    {
        SequencesOfColor = new SequenceOfColor[nbColorSequences];
        soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
    }


    private void OnTriggerEnter(Collider other)
    {
        TransportableByConveyor transportableByConveyor = null;
        if (other.gameObject.TryGetComponent(out transportableByConveyor) && (HasBeenPickupNeeded && transportableByConveyor.HasBeenPickUp || !HasBeenPickupNeeded))
        {
            Consume(transportableByConveyor);
        }
    }

    //TODO : Reset sequence when player commits an error. Generate a new color sequence. Maybe
    
    private void Consume(TransportableByConveyor transportableByConveyor)
    {
        transportableByConveyor.Consume();
        Destroy(transportableByConveyor.gameObject, TimeToConsume);

        SequenceOfColor currentSequence = SequencesOfColor[SucceedSequences];

        Color currentSequenceColor = currentSequence.ColorsSequence[currentSequence.SucceedColors];
        TransportableType currentType = currentSequence.types[currentSequence.SucceedColors];
        
        if (currentSequenceColor.r == ( transportableByConveyor.Color).r && currentSequenceColor.g == ( transportableByConveyor.Color).g &&currentSequenceColor.b == ( transportableByConveyor.Color).b && currentType == transportableByConveyor.GetType())
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
            sc.types = new TransportableType[currentSequenceLenght];
            for (int j = 0; j < currentSequenceLenght; j++)
            {
                int nextType = _random.Next(0, 2);
                int nextColor = _random.Next(0, allColors.Length);
                sc.ColorsSequence[j] = allColors[nextColor];
                sc.types[j] = (TransportableType)nextType;
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

    public TransportableType GetNextItemType()
    {
        SequenceOfColor currentSequence = GetCurrentSequence();
        return currentSequence.types[currentSequence.SucceedColors];
    }
}
