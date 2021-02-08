using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreenMenuController : MonoBehaviour
{
    [SerializeField] private Text LoadingText = null;
    [SerializeField] private RawImage LoadingImage = null;
    [SerializeField] private float RotationSpeed = 0.0f;

    #region Unity Callbacks
    private void Update()
    {
        LoadingImage.transform.Rotate(Vector3.forward, RotationSpeed * Time.deltaTime);
    }
    #endregion
    #region Public Functions
    public void Show(string text)
    {
        LoadingImage.transform.rotation = Quaternion.identity;
        LoadingText.text = text;
        this.gameObject.SetActive(true);
    }
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
    #endregion
}
