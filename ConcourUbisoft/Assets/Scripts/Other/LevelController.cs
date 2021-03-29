using UnityEngine;
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
        public int GetCurrentSequenceIndex();

        public int GetCurrentRequiredItemIndex();

    }
}
