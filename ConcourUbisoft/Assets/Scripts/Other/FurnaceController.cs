using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Random = System.Random;

public class FurnaceController : MonoBehaviour
{
    [Serializable]
    public class SequenceOfColor
    {
        public Color[] ColorsSequence = null;
        public int SucceedColors = 0;
    }
    
    [SerializeField] private SequenceOfColor[] SequencesOfColor = null;
    [SerializeField] private int nbColorSequences = 5;
    [SerializeField] private int minColorSequencelenght=3;
    [SerializeField] private int maxColorSequenceLenght=7;
    [SerializeField] private Level1Controller Level1Controller = null;
    [SerializeField] private float TimeToConsume = 0.0f;
    private int SucceedSequences = 0;
    private Random colorPicker;
    private Color[] allColors;

    private void Start()
    {
        colorPicker = new Random();
        allColors = Level1Controller.GetColors();
        SequencesOfColor = new SequenceOfColor[nbColorSequences];
        generateNewColorSequences();
    }


    private void OnTriggerEnter(Collider other)
    {
        TransportableByConveyor transportableByConveyor = null;
        if (other.gameObject.TryGetComponent(out transportableByConveyor) && transportableByConveyor.HasBeenPickUp)
        {
            Consume(transportableByConveyor);
        }
    }

    private void Consume(TransportableByConveyor transportableByConveyor)
    {
        transportableByConveyor.Consume();
        Destroy(transportableByConveyor.gameObject, TimeToConsume);

        SequenceOfColor currentSequence = SequencesOfColor[SucceedSequences];

        Color currentSequenceColor = currentSequence.ColorsSequence[currentSequence.SucceedColors];
        
        if (currentSequenceColor.r == ( transportableByConveyor.Color).r && currentSequenceColor.g == ( transportableByConveyor.Color).g &&currentSequenceColor.b == ( transportableByConveyor.Color).b)
        {
            currentSequence.SucceedColors++;
            if (currentSequence.SucceedColors == currentSequence.ColorsSequence.Length)
            {
                SucceedSequences++;
                if (SucceedSequences == SequencesOfColor.Length)
                {
                    Level1Controller.FinishLevel();
                }
            }
        }
    }

    public void generateNewColorSequences()
    {
        int currentSequenceLenght = minColorSequencelenght;
        for (int i = 0; i < nbColorSequences; i++)
        {
            SequenceOfColor sc = new SequenceOfColor();
            sc.ColorsSequence = new Color[currentSequenceLenght];
            for (int j = 0; j < currentSequenceLenght; j++)
            {
                int nextColor =colorPicker.Next(0, allColors.Length);
                sc.ColorsSequence[j] = allColors[nextColor];
            }
            SequencesOfColor[i] = sc;
            if (currentSequenceLenght < maxColorSequenceLenght)
            {
                currentSequenceLenght += 1;
            }
        }
    }
}
