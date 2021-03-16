using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(randomCodePicker), typeof(Animation))]
public class DoorController : MonoBehaviour
{
    public enum Direction
    {
        Left,
        Right,
        Bottom,
        Up
    }

    [SerializeField] public int Id = 0;
    private List<Direction> _inputSequences;
    private List<Direction> _currentSequences = new List<Direction>();
    private Animation _animation = null;

    public UnityEvent OnError;
    public UnityEvent OnSuccess;
    public UnityEvent OnEntry;

    public bool IsUnlock { get; private set; } = false;

    private void Awake()
    {
        _animation = GetComponent<Animation>();

        _inputSequences = GetComponent<randomCodePicker>().GetSequence();
    }

    public void TriggerLeft()
    {
        _currentSequences.Add(Direction.Left);
        OnEntry.Invoke();
    }

    public void TriggerRight()
    {
        _currentSequences.Add(Direction.Right);
        OnEntry.Invoke();
    }

    public void TriggerBottom()
    {
        _currentSequences.Add(Direction.Bottom);
        OnEntry.Invoke();
    }

    public void TriggerUp()
    {
        _currentSequences.Add(Direction.Up);
        OnEntry.Invoke();
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
        Debug.Log("Play");
        IsUnlock = true;
        _animation.Play();
        OnSuccess?.Invoke();
    }
}
