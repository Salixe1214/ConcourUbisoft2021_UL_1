using UnityEngine;
namespace Other
{
    public interface LevelController
    {
        public Color[] GetColors();
        public Color GetNextColorInSequence();
        public int GetCurrentSequenceLenght();
        public TransportableType GetNextTypeInSequence();
    }
}
