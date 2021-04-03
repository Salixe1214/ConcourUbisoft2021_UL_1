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
    [SerializeField] private Text TimeTextField;
    [SerializeField] private Text BonusTimeTextField;
    [SerializeField] private Text WarningTextField;
    [SerializeField] private GameObject LevelControl;
    [SerializeField] private GameObject TimeImageObject;
    [SerializeField] private float BonusTimeShownDuration;
    [SerializeField] private float WarningDuration;
    
    private float _time;
    private LevelController _levelController;
    private float _timeLeft;
    private Color _timeTextColor;
    private AudioSource _timerAudioSource;

    private void Awake()
    {
        _levelController = LevelControl.GetComponent<LevelController>();
    }

    void Start()
    {
        BonusTimeTextField.text = "";
        TimeTextField.text = "";
        _timeTextColor = TimeTextField.color;
        _timerAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        _levelController.OnTimeChanged += SetTime;
        _levelController.OnBonusTime += ShowBonusTime;
        _levelController.OnWarning += ShowWarning;
    }

    private void OnDisable()
    {
        _levelController.OnTimeChanged -= SetTime;
        _levelController.OnBonusTime -= ShowBonusTime;
        _levelController.OnWarning -= ShowWarning;
    }

    private void SetTime(float timeValue)
    {
        if (!TimeImageObject.activeSelf)
        {
            TimeImageObject.SetActive(true);
        }
        TimeTextField.text = timeValue.ToString("G");
    }

    private void ShowBonusTime(float bonusTime)
    {
        BonusTimeTextField.text = "+ " + bonusTime.ToString("G");
        StartCoroutine(StartBonusTimeSequence());
    }

    private void ShowWarning(float remainingTime)
    {
        WarningTextField.text = remainingTime.ToString("G") + " Seconds Before Sequence Failure";
        _timerAudioSource.Stop();
        _timerAudioSource.clip = TimeLeftSound;
        _timerAudioSource.time = 0;
        StartCoroutine(StartWarning());
    }

    private void EndTimer()
    {
        gameObject.SetActive(false);
    }

    IEnumerator StartWarning()
    {
        WarningTextField.gameObject.SetActive(true);
        _timerAudioSource.Play();
        yield return new WaitForSeconds(WarningDuration);
        WarningTextField.gameObject.SetActive(false);
    }

    IEnumerator StartBonusTimeSequence()
    {
        TimeTextField.color = BonusTimeTextField.color;
        yield return new WaitForSeconds(BonusTimeShownDuration);
        BonusTimeTextField.text ="";
        TimeTextField.color = _timeTextColor;
    }
    //public float RemaningTime { get { float timeElapsed = Time.time - startTime;  return timeElapsed > gameDuration ? 0 : gameDuration - timeElapsed; } }

}
