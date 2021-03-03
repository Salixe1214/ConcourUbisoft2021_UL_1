using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreenMenuController : MonoBehaviour
{
    [SerializeField] private Text _loadingText = null;
    [SerializeField] private RawImage _loadingImage = null;
    [SerializeField] private float _rotationSpeed = 0.0f;

    #region Unity Callbacks
    private void Update()
    {
        _loadingImage.transform.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime);
    }
    #endregion
    #region Public Functions
    public void Show(string text)
    {
        _loadingImage.transform.rotation = Quaternion.identity;
        _loadingText.text = text;
        this.gameObject.SetActive(true);
    }
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
    #endregion
}
