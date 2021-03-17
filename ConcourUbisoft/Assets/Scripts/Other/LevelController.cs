using UnityEngine;
namespace Other
{
    public interface LevelController
    {
        Color[] GetColors();
        Color GetNextColorInSequence();
        int GetCurrentSequenceLenght();
        TransportableType GetNextTypeInSequence();
    }
}
