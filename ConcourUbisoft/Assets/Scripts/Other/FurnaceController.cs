using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceController : MonoBehaviour
{
    [Serializable]
    public class SequenceOfColor
    {
        public Color[] ColorsSequence = null;
        public int SucceedColors = 0;
    }

    [SerializeField] private SequenceOfColor[] SequencesOfColor = null;
    [SerializeField] private Level1Controller Level1Controller = null;
    [SerializeField] private float TimeToConsume = 0.0f;
    private int SucceedSequences = 0;

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
        if (currentSequence.ColorsSequence[currentSequence.SucceedColors] == transportableByConveyor.Color)
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
}
