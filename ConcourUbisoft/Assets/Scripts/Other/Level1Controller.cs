using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Controller : MonoBehaviour
{
    [SerializeField] private Color[] PossibleColors = null;
    public Color[] GetColors() => PossibleColors;

    [SerializeField] private FurnaceController FurnaceController = null;
    [SerializeField] private TransportableSpawner TransportableSpawner = null;

    public void Awake()
    {
        FurnaceController.enabled = false;
        TransportableSpawner.enabled = false;
    }

    public void Start()
    {
        StartLevel();
    }

    public void FinishLevel()
    {
        FurnaceController.enabled = false;
        TransportableSpawner.enabled = false;
    }

    public void StartLevel()
    {
        FurnaceController.enabled = true;
        TransportableSpawner.enabled = true;
    }
}
