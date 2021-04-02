using System;
using System.Collections;
using System.Collections.Generic;
using Other;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    // Start is called before the first frame update
    
    [SerializeField] private AudioClip TimeLeftSound;
    [SerializeField] private Text _timeTextField;
    [SerializeField] private Text _bonusTimeTextField;
    [SerializeField] private GameObject LevelControl;
    [SerializeField] private GameObject TimeImageObject;
    [SerializeField] private float BonusTimeShownDuration;
    
    private float _time;
    private LevelController _levelController;
    private float _timeLeft;
    private Color _timeTextColor;

    private void Awake()
    {
        _levelController = LevelControl.GetComponent<LevelController>();
    }

    void Start()
    {
        _bonusTimeTextField.text = "";
        _timeTextField.text = "";
        _timeTextColor = _timeTextField.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        _levelController.OnTimeChanged += SetTime;
        _levelController.OnBonusTime += ShowBonusTime;
    }

    private void OnDisable()
    {
        _levelController.OnTimeChanged -= SetTime;
        _levelController.OnBonusTime -= ShowBonusTime;
    }

    private void SetTime(float _timeValue)
    {
        if (!TimeImageObject.activeSelf)
        {
            TimeImageObject.SetActive(true);
        }
        _timeTextField.text = _timeValue.ToString("G");
    }

    private void ShowBonusTime(float _bonusTime)
    {
        _bonusTimeTextField.text = "+ " + _bonusTime.ToString("G");
        StartCoroutine(StartBonusTimeSequence());
    }

    private void EndTimer()
    {
        gameObject.SetActive(false);
    }

    IEnumerator StartBonusTimeSequence()
    {
        _timeTextField.color = _bonusTimeTextField.color;
        yield return new WaitForSeconds(BonusTimeShownDuration);
        _bonusTimeTextField.text ="";
        _timeTextField.color = _timeTextColor;
    }
    //public float RemaningTime { get { float timeElapsed = Time.time - startTime;  return timeElapsed > gameDuration ? 0 : gameDuration - timeElapsed; } }

}
