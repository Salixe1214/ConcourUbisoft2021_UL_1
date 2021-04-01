using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    [SerializeField] private GameObject creditPanel;
    [SerializeField] private Button openButtun;
    private SoundController _menuSoundController;
    
    // Start is called before the first frame update
    void Awake()
    {
        _menuSoundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        creditPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(creditPanel.activeSelf && Input.GetButton("Cancel"))
            creditPanel.SetActive(false);
    }

    public void Open()
    {
        _menuSoundController.PlayButtonSound();
        creditPanel.SetActive(true);
        openButtun.enabled = false;
    }

    public void Close()
    {
        _menuSoundController.PlayButtonSound();
        openButtun.enabled = true;
        creditPanel.SetActive(false);
    }
}
