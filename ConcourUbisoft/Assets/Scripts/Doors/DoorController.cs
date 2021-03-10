using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorController : MonoBehaviour
{
    public enum Direction
    {
        Left,
        Right,
        Bottom,
        Up
    }

    [SerializeField] private List<Direction> _inputSequences = null;
    [SerializeField] public int Id = 0;
    private List<Direction> _currentSequences = new List<Direction>();

    public UnityEvent OnError;
    public UnityEvent OnSuccess;

    public bool IsUnlock { get; private set; } = false;

    public void TriggerLeft()
    {
        _currentSequences.Add(Direction.Left);
    }

    public void TriggerRight()
    {
        _currentSequences.Add(Direction.Right);
    }

    public void TriggerBottom()
    {
        _currentSequences.Add(Direction.Bottom);
    }

    public void TriggerUp()
    {
        _currentSequences.Add(Direction.Up);
    }

    public void CheckSequence()
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

    public void Unlock()
    {
        IsUnlock = true;
        OnSuccess?.Invoke();
    }
}
