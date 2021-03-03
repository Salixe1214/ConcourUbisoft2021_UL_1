using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TechSupport.Informations
{
    public class InformationsSystem : MonoBehaviour
    {
        [Serializable]
        public struct InformationItem
        {
            public string title;
            [TextArea]
            public string content;
            public List<Sprite> images;
        }

        [Header("List")] 
        private ImageLayout _imageLayout;
        [SerializeField] private List<Sprite> imagesList;
        [SerializeField] private Vector2 sizeList;

        [Header("Accordion")] 
        private Accordion _accordion;
        [SerializeField] private List<InformationItem> items;
        [SerializeField] private Sprite background;
        [SerializeField] private Sprite front;

        private RectTransform _listRectTransform;
        private RectTransform _accordionRectTransform;

        private void Awake()
        {
            CreateList();
            CreateAccordion();
        }

        private void Update()
        {
            _imageLayout.transform.position = new Vector3 (Screen.width * 0.5f, Screen.height * 0.95f, 0);
            _accordion.transform.position = new Vector3(Screen.width * 0.85f, Screen.height * 0.5f);
            _accordionRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width * 0.15f);
        }

        private void SetSize(RectTransform rectTransform, Vector2 size)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        }
        private void CreateList()
        {
            _imageLayout = new GameObject().AddComponent<ImageLayout>();
            _listRectTransform = _imageLayout.GetComponent<RectTransform>();
            _listRectTransform.SetParent(transform);
            SetSize(_listRectTransform, sizeList);
           // _imageLayout.CreateLayout(imagesList);
        } 

        private void CreateAccordion()
        {
            _accordion = new GameObject().AddComponent<Accordion>();
            _accordionRectTransform = _accordion.GetComponent<RectTransform>();
            _accordionRectTransform.SetParent(transform);
            _accordion.Setup(background, front);
            _accordion.CreateAccordion(items);
        }

        public ImageLayout GetList()
        {
            return _imageLayout;
        }

        public Accordion GetInformationDisplay()
        {
            return _accordion;
        }
    }
}
