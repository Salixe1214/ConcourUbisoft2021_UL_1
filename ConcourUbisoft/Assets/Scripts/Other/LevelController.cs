using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Other
{
    public interface LevelController
    {
        Color[] GetColors();
        Color GetNextColorInSequence();
        int GetCurrentSequenceLenght();
        PickableType GetNextTypeInSequence();
        PickableType[] GetAllNextItemTypes();

        Color[] GetAllNextItemColors();
        public int GetIndexInCurrentSequence();

        public int GetCurrentRequiredItemIndex();
        public event Action<float> OnTimeChanged;
        public event Action<float> OnBonusTime;
        public event Action<float> OnWarning;

    }
}
