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

namespace TechSupport.Informations
{
    public class InformationsSystem : MonoBehaviour
    {
        enum DevMode
        {
            Test,
            Game
        }
        
        [Header("Général")] [SerializeField] private DevMode mode = DevMode.Game;

        [Header("List")] 
        [SerializeField] private List<Sprite> imagesList;
        [SerializeField] private Color[] colors;
        [SerializeField] private Vector2 sizeList;
        private ImageLayout _imageLayout;

        [Header("Accordion")]
        [SerializeField] private Sprite background;
        [SerializeField] private Sprite front;
        private Accordion _accordion;
        private List<InformationItem> _items;
        
        [Header("Tech Book")] 
        [SerializeField] private SerializableDictionary<DoorCode.Symbol, Sprite> symbols =
            new SerializableDictionary<DoorCode.Symbol, Sprite>(
                new Dictionary<DoorCode.Symbol, Sprite>
                {
                    {DoorCode.Symbol.One, null},
                    {DoorCode.Symbol.Two, null},
                    {DoorCode.Symbol.Three, null}
                });

        [SerializeField] private Sprite arrow;

        private RectTransform _listRectTransform;
        private RectTransform _accordionRectTransform;

        private void Awake()
        {
            _items = new List<InformationItem>()
            {
                new TechnicienBook(symbols, arrow)
            };

            CreateList();
            CreateAccordion();
        }

        private void Update()
        {
            if (_imageLayout)
                _imageLayout.transform.position = new Vector3 (Screen.width * 0.5f, Screen.height * 0.95f, 0);
            if (_accordion)
            {
                _accordion.transform.position = new Vector3(Screen.width * 0.80f, Screen.height * 0.5f);
                _accordionRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width * 0.20f);
            }
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
            if (mode == DevMode.Test)
            {
                _imageLayout.CreateLayout(imagesList, colors);
            }
        } 

        private void CreateAccordion()
        {
            _accordion = new GameObject().AddComponent<Accordion>();
            _accordionRectTransform = _accordion.GetComponent<RectTransform>();
            _accordionRectTransform.SetParent(transform);
            _accordion.Setup(background, front);
            _accordion.CreateAccordion(_items);
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
