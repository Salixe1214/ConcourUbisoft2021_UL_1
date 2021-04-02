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

    [SerializeField] private Sprite BonusTimeSprite;
    [SerializeField] private AudioClip TimeLeftSound;
    [SerializeField] private Text _timeTextField;
    [SerializeField] private Text _bonusTimeTextField;
    [SerializeField] private GameObject LevelControl;
    [SerializeField] private float BonusTimeShownDuration;

    private float _time;
    private LevelController _levelController;
    private Image _bonusTimeImage;
    private float _timeLeft;

    private void Awake()
    {
        _levelController = LevelControl.GetComponent<LevelController>();
    }

    void Start()
    {
        _bonusTimeTextField.text = "";
        _timeTextField.text = "";
        _bonusTimeImage = GetComponentInChildren<Image>();
        _bonusTimeImage.sprite = BonusTimeSprite;
        _bonusTimeTextField.gameObject.SetActive(false);
        _bonusTimeImage.gameObject.SetActive(false);
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
        if (!_timeTextField.gameObject.activeSelf)
        {
            _timeTextField.gameObject.SetActive(true);
        }
        _timeTextField.text = _timeValue.ToString("N");
    }

    private void ShowBonusTime(float _bonusTime)
    {
        _bonusTimeTextField.text = "+ " + _bonusTime.ToString("N");
        StartCoroutine(StartBonusTimeSequence());
    }

    private void EndTimer()
    {
        gameObject.SetActive(false);
    }

    IEnumerator StartBonusTimeSequence()
    {
        _bonusTimeTextField.gameObject.SetActive(true);
        _bonusTimeImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(BonusTimeShownDuration);
        _bonusTimeTextField.gameObject.SetActive(false);
        _bonusTimeImage.gameObject.SetActive(false);
    }
    //public float RemaningTime { get { float timeElapsed = Time.time - startTime;  return timeElapsed > gameDuration ? 0 : gameDuration - timeElapsed; } }

}
