using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Controller : MonoBehaviour
{
    [SerializeField] private Color[] PossibleColors = null;
    public Color[] GetColors() => PossibleColors;
    public Color GetNextColorInSequence() => FurnaceController.GetNextColor();
    public int GetCurrentSequenceLenght() => FurnaceController.GetCurrentSequenceLenght();

    [SerializeField] private FurnaceController FurnaceController = null;
    [SerializeField] private TransportableSpawner TransportableSpawner = null;
    [SerializeField] private DoorsScript Level1Door = null;

    public void Start()
    {
        FurnaceController.enabled = false;
        TransportableSpawner.enabled = false;
    }

    private void OnEnable()
    {
        Level1Door.OnDoorUnlockEvent += OnDoorUnlockEvent;
    }

    private void OnDisable()
    {
        Level1Door.OnDoorUnlockEvent -= OnDoorUnlockEvent;
    }

    private void OnDoorUnlockEvent(DoorsScript doorsScript)
    {
        StartLevel();
    }

    public void FinishLevel()
    {
        FurnaceController.enabled = false;
        TransportableSpawner.enabled = false;
        TransportableSpawner.gameObject.SetActive(false);
    }

    public void StartLevel()
    {
        FurnaceController.enabled = true;
        TransportableSpawner.enabled = true;
    }
}
