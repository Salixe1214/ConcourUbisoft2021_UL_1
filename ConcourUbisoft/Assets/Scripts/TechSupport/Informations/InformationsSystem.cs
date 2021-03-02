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

        [Header("Accordion")] 
        private Accordion _accordion;
        [SerializeField] private List<InformationItem> items;
        [SerializeField] private Vector3 position;
        [SerializeField] private Vector2 size;
        [SerializeField] private Sprite background;
        [SerializeField] private Sprite front;

        private void Awake()
        {
            CreateList();
            CreateAccordion();
        }

        private void CreateList()
        {
            _imageLayout = new GameObject().AddComponent<ImageLayout>();
            
            _imageLayout.GetComponent<RectTransform>().SetParent(transform);
            _imageLayout.transform.position = new Vector3 (Screen.width * 0.5f, Screen.height * 0.9f, 0);
            _imageLayout.CreateLayout(imagesList);
        }

        private void CreateAccordion()
        {
            _accordion = new GameObject().AddComponent<Accordion>();

            _accordion.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
//            _accordion.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            _accordion.GetComponent<RectTransform>().SetParent(transform);
            _accordion.Setup(background, front);
            _accordion.transform.position = position;
            _accordion.CreateAccordion(items);
        }
    }
}
