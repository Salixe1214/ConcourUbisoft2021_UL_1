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
    private int SucceedSequences = 0;

    [SerializeField] private Level1Controller Level1Controller = null;

    private void OnTriggerEnter(Collider other)
    {
        TransportableByConveyor transportableByConveyor = null;
        if (other.gameObject.TryGetComponent(out transportableByConveyor) && !transportableByConveyor.Consumed && transportableByConveyor.HasBeenPickUp)
        {
            Consume(transportableByConveyor);
        }
    }

    private void Consume(TransportableByConveyor transportableByConveyor)
    {
        transportableByConveyor.Consumed = true;
        Destroy(transportableByConveyor.gameObject);

        SequenceOfColor currentSequence = SequencesOfColor[SucceedSequences];
        if (currentSequence.ColorsSequence[currentSequence.SucceedColors] == transportableByConveyor.Color)
        {
            currentSequence.SucceedColors++;
            if(currentSequence.SucceedColors == currentSequence.ColorsSequence.Length)
            {
                SucceedSequences++;
                if(SucceedSequences == SequencesOfColor.Length)
                {
                    Level1Controller.FinishLevel();
                }
            }
        }
    }
}
