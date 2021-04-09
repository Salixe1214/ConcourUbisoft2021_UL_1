using System;
using System.Collections.Generic;
using System.Linq;
using Doors;
using TechSupport.Informations.Items;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;


// TODO: <div>Icons made by <a href="https://www.freepik.com" title="Freepik">Freepik</a> from <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com</a></div>

namespace TechSupport.Informations
{
    public class InformationsSystem : MonoBehaviour
    {
        [Header("List")] 
        [SerializeField] private Vector2 sizeList;
        [SerializeField] private GameObject checkImagePrefabs;
        [SerializeField] private Material blurMaterial;
        [SerializeField] private Material fontMaterial;
        [SerializeField] private Sprite _questionSprite;

        [Header("TechBook")]
        [SerializeField] private Button button;
        [SerializeField] private GameObject informationPanel;
        [SerializeField] private Font font;

        private ImageLayout _imageLayout;
        private Animator _animator;
        private RectTransform _listRectTransform;

        private static readonly int Showed = Animator.StringToHash("Showed");

        public void Init()
        {
            _animator = informationPanel.GetComponent<Animator>();
            CreateList();
        }

        private void Update()
        {
            if (_imageLayout)
                _imageLayout.transform.position = new Vector3 (Screen.width * 0.5f, Screen.height * 0.95f, 0);
        }
        private static void SetSize(RectTransform rectTransform, Vector2 size)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        }
        private void CreateList()
        {
            _imageLayout = new GameObject().AddComponent<ImageLayout>();
            _imageLayout.Font = font;
            if (checkImagePrefabs != null)
                _imageLayout.SetCheckImage(checkImagePrefabs);
            if (blurMaterial != null)
                _imageLayout.SetBlurMaterial(blurMaterial);
            if (fontMaterial != null)
                _imageLayout.SetFontMaterial(fontMaterial);
            _imageLayout.SetQuestion(_questionSprite);
            _listRectTransform = _imageLayout.GetComponent<RectTransform>();
            _listRectTransform.SetParent(transform);
            SetSize(_listRectTransform, sizeList);
        }

        public ImageLayout GetList()
        {
            return _imageLayout;
        }

        public void ActivateInformation(bool activation)
        {
            if (_animator != null)
            {
                _animator.gameObject.SetActive(activation);
            }
            button.gameObject.SetActive(activation);
        }

        public void OpenInfo()
        {
            if (_animator != null)
            {
                bool isShowed = _animator.GetBool(Showed);
                
                button.gameObject.SetActive(isShowed);
                _animator.SetBool(Showed, !isShowed);
            }
        }
    }
}
